using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq;
using System;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Shared.HexMap.External.Schema;
using Modules.Server.GameLogic.External;


namespace Modules.Server.GameLogic.Internal
{
    [UsedImplicitly]
    public class GameLogic : IGameLogic
    {
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
            return result;
        }

        public void SetPlayerPos(Hex2 newPos)
            => playerPos = newPos.Round();

        public GameState Next(GameState state)
        {
            state.SetPlayerPos(playerPos);

            if (state.Users != null)
            {
                foreach (var actor in state.Users.SelectMany(user => user?.Team?.Actors ?? new List<Actor>()))
                {
                    if (actor == null) continue;
                    if (actor.IsDead) continue;

                    var newCoords = actor.OwnedByTeamId == team1Id
                        ? MoveTowardsTarget(actor, playerPos, state)
                        : MoveTowardsTarget(actor, FindClosestActor(actor, state)?.Coords ?? Hex2.Zero, state);

                        actor.SetCoords(newCoords);
                }
            }

            ProcessKills(state);

            var winner = state.GetWinner();
            if (winner != null)
            {
                state.WinningTeamName = winner.Team.TeamName;
            }

            return state;
        }

        void ProcessKills (GameState state)
            => state.Users.SelectMany(user => user.Team.Actors)
                .Where(attacker => !attacker.IsDead)
                .Select(attacker => (attacker, victim:
                    GetNeighbouringOpponents(attacker, state)
                        .FirstOrDefault()))
                .Where(tuple => tuple.victim != null)
                .ToList().ForEach(tuple => tuple.victim
                    .DecrementHealth(tuple.attacker.HitPoints));

        List<Actor> GetNeighbouringOpponents (Actor actor, GameState state)
            => Hex2.GetNeighbors(actor.Coords)
                .Select(state.ActorAtCoord)
                .Where(neighbor => neighbor != null)
                .Where(neighbor => neighbor.OwnedByTeamId != actor.OwnedByTeamId)
                .Where(neighbor => !neighbor.IsDead)
                .ToList();

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

        Hex2 MoveTowardsTarget(Actor actor, Hex2 target, GameState state)
            => Hex2.GetNeighbors(actor.Coords)
                .Where(neighbour => !Hex2.OutOfBounds(neighbour, state.Radius))
                .Where(neighbour => HeightAtCoords(neighbour, state) >= state.MinWalkHeight)
                .Where(neighbour => state.ActorAtCoord(neighbour) == null
                    || (state.ActorAtCoord(neighbour).IsDead
                        && state.ActorAtCoord(neighbour).OwnedByTeamId == actor.OwnedByTeamId))
                .OrderBy(neighbor => Hex2.Distance(neighbor, target))
                .FirstOrDefault();

        float HeightAtCoords(Hex2 coords, GameState state)
            => coords.PerlinHeight(state.SeedAsFloat, state.NoiseScale, state.Amplitude, state.NoiseOffsetX, state.NoiseOffsetY);
    }
}