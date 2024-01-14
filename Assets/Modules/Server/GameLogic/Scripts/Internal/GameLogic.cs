using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine;
using System;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Server.GameLogic.External;
using Modules.Shared.Neese.AStarNav.External;
using Modules.Shared.Neese.HexTwo.External.Schema;

namespace Modules.Server.GameLogic.Internal
{
    [UsedImplicitly]
    public class GameLogic : IGameLogic
    {
        AStarAgent agent;
        Hex2 playerPos;
        Guid team1Id;

        public GameState Init(List<User> users, int radius, string seed, float minWalkHeight, float amplitude, float noiseScale, int noiseOffsetX, int noiseOffsetY)
        {
            var result = new GameState(
                new HexGrid(radius),
                Hex2.Zero,
                radius,
                seed,
                minWalkHeight,
                amplitude,
                noiseScale,
                noiseOffsetX,
                noiseOffsetY,
                users
            );

            team1Id = users.First().Team.TeamId;
            agent = new AStarAgent();

            return result;
        }

        public void SetPlayerPos(Hex2 newPos)
            => playerPos = newPos.Round();

        public GameState Next(GameState state)
        {
            state.SetPlayerPos(playerPos);

            var actors = state.Users.SelectMany(user => user?.Team?.Actors ?? new List<Actor>());
            foreach (var actor in actors)
            {
                if (actor == null) continue;
                if (actor.IsDead) continue;

                Hex2 newCoords;
                if (actor.OwnedByTeamId == team1Id)
                {
                    var tuple = PathTowardsTarget(actor, playerPos, state);
                    actor.SetNavPath(tuple.navPath);
                    newCoords = tuple.coords;
                }
                else
                {
                    newCoords = MoveTowardsTarget(actor, FindClosestActor(actor, state)?.Coords ?? Hex2.Zero, state);
                }

                actor.SetCoords(newCoords);
            }

            ProcessKills(state);

            var winner = state.GetWinner();
            if (winner != null)
            {
                state.WinningTeamName = winner.Team.TeamName;
            }

            return state;
        }

        Actor FindClosestActor(Actor currentActor, GameState state)
        {
            var targetTeam = state.GetTeamById(team1Id);
            var closestDistance = double.MaxValue;
            Actor closestActor = null;

            foreach (var actor in targetTeam.Actors)
            {
                if (actor.IsDead) continue;
                if (currentActor.OwnedByTeamId == actor.OwnedByTeamId) continue;

                var distance = Hex2.Distance(currentActor.Coords, actor.Coords);
                if (distance >= closestDistance) continue;

                closestDistance = distance;
                closestActor = actor;
            }

            return closestActor;
        }

        (Hex2 coords, List<Vector3> navPath) PathTowardsTarget(Actor actor, Hex2 target, GameState state)
        {
            var navPath = agent.BuildPath(
                actor.Coords.ToVector3(),
                target.ToVector3(),
                Hex2.GetWorldNeighbors,
                pos => CostAtPos(pos, state),
                20
            );

            if (navPath.Count > 0)
            {
                var newCoords = navPath[0].ToHex2();
                return (newCoords, navPath);
            }

            return (actor.Coords, null);
        }

        List<float> CostAtPos (Vector3 pos, GameState state)
        {
            var result = new List<float>();
            var coords = pos.ToHex2();

            if (!Hex2.WithinRadius(coords, state.Radius))
            {
                result.Add(float.MaxValue);
            }

            var actorsAtCoord = state.ActorsAtCoord(coords);
            if (actorsAtCoord.Count > 0)
            {
                result.Add(actorsAtCoord.All(actor => actor.IsDead) ? 1 : 10);
            }
            else
            {
                result.Add(0);
            }

            return result;
        }

        void ProcessKills (GameState state)
            => state.Users.SelectMany(user => user.Team.Actors)
                .Where(attacker => !attacker.IsDead)
                .Select(attacker => (attacker, victims: GetNeighbouringOpponents(attacker, state)))
                .ToList()
                .ForEach(tuple =>
                {
                    tuple.victims.ForEach(victim =>
                    {
                        victim.DecrementHealth(tuple.attacker.HitPoints);
                    });
                });
        List<Actor> GetNeighbouringOpponents (Actor actor, GameState state)
            => Hex2.GetNeighbors(actor.Coords)
                .SelectMany(state.ActorsAtCoord)
                .Where(otherActor => otherActor != null)
                .Where(otherActor => otherActor.OwnedByTeamId != actor.OwnedByTeamId)
                .Where(otherActor => !otherActor.IsDead)
                .ToList();
        Hex2 MoveTowardsTarget(Actor actor, Hex2 target, GameState state)
            => Hex2.GetNeighbors(actor.Coords)
                .Where(neighbour => Hex2.WithinRadius(neighbour, state.Radius))
                .Where(neighbour => HeightAtCoords(neighbour, state) >= state.MinWalkHeight)
                .Where(neighbour =>
                {
                    var actorsAtCoords =  state.ActorsAtCoord(neighbour);
                    return actorsAtCoords.Count < 1 || actorsAtCoords.All(otherActor => otherActor.OwnedByTeamId != actor.OwnedByTeamId || otherActor.IsDead);
                })
                .OrderBy(neighbor => Hex2.Distance(neighbor, target))
                .FirstOrDefault();
        float HeightAtCoords(Hex2 coords, GameState state)
            => coords.PerlinHeight(state.SeedAsFloat, state.NoiseScale, state.Amplitude, state.NoiseOffsetX, state.NoiseOffsetY);
    }
}