using System.Collections.Generic;
using Random = System.Random;
using JetBrains.Annotations;
using UnityEngine;
using System.Linq;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Shared.HexMap.External.Schema;
using Modules.Server.GameLogic.External;


namespace Modules.Server.GameLogic.Internal
{
    [UsedImplicitly]
    public class GameLogic : IGameLogic
    {
        public GameState Init(List<User> users, int radius, string seed, float minWalkHeight, float amplitude, float noiseScale, int noiseOffsetX, int noiseOffsetY)
            => new(
                new HexGrid(radius),
                radius,
                seed,
                minWalkHeight,
                amplitude,
                noiseScale,
                noiseOffsetX,
                noiseOffsetY,
                users
            );

        public GameState Next(GameState state)
        {
            var team1 = state.Users[0].Team.TeamId;
            foreach (var actor in state.Users.SelectMany(user => user.Team.Actors))
            {
                Hex2 newCoords;

                if (actor.OwnedByTeamId == team1)
                {
                    var shouldEvade = CheckForEnemiesInRange(actor, state, 3);
                    newCoords = shouldEvade ? EvadeOpponent(actor, state) : MoveToFriendly(actor, state);
                }
                else
                {
                    newCoords = Team2Behaviour(actor, state);
                }

                actor.SetCoords(newCoords);
            }

            return state;
        }
        public bool CheckForEnemiesInRange(Actor actor, GameState state, int checkRadius)
        {
            var enemyTeamId = state.Users[0].Team.TeamId == actor.OwnedByTeamId ? state.Users[1].Team.TeamId : state.Users[0].Team.TeamId;
            var enemyTeam = state.GetTeamById(enemyTeamId);

            return enemyTeam.Actors.Any(enemyActor => Hex2.Distance(actor.Coords, enemyActor.Coords) <= checkRadius);
        }

        Hex2 Team2Behaviour (Actor actor, GameState state)
        {
            var opCoords = MoveToOpponent(actor, state);
            var valid = !Hex2.OutOfBounds(opCoords, state.Radius);
            var result = !Hex2.OutOfBounds(opCoords, state.Radius)
                ? opCoords
                : Wander(actor.Coords, state.Radius);

            Debug.Log($"<color=yellow><b>>>> actor: {actor.Coords}, opp: {opCoords}, radius: {state.Radius}</b></color>");

            return result;
        }

        Hex2 MoveToOpponent(Actor actor, GameState state)
        {
            var enemyTeam = state.Users[0].Team.TeamId == actor.OwnedByTeamId ? state.Users[1].Team : state.Users[0].Team;
            var closestEnemy = FindClosestActor(actor, enemyTeam);
            return MoveTowardsTarget(actor, closestEnemy.Coords, state);
        }

        Hex2 EvadeOpponent(Actor actor, GameState state)
        {
            var enemyTeam = state.Users[0].Team.TeamId == actor.OwnedByTeamId ? state.Users[1].Team : state.Users[0].Team;
            var closestEnemy = FindClosestActor(actor, enemyTeam);

            if (closestEnemy == null)
            {
                return actor.Coords;
            }

            return MoveAwayFromTarget(actor, closestEnemy.Coords, state);
        }

        Hex2 MoveAwayFromTarget(Actor actor, Hex2 targetCoords, GameState state)
        {
            var currentCoords = actor.Coords;
            var neighbors = new List<Hex2>
            {
                new (currentCoords.ne + 1, currentCoords.se),
                new (currentCoords.ne, currentCoords.se + 1),
                new (currentCoords.e + 1, currentCoords.e + 1),
                new (currentCoords.ne - 1, currentCoords.se),
                new (currentCoords.ne, currentCoords.se - 1),
                new (currentCoords.e - 1, currentCoords.e - 1)
            };

            var validNeighbors = neighbors.Where(neighbor => !Hex2.OutOfBounds(neighbor, state.Radius)).ToList();
            if (!validNeighbors.Any()) return currentCoords;

            var bestNeighbor = currentCoords;
            var furthestDistance = double.MinValue;

            foreach (var neighbor in validNeighbors)
            {
                var distance = Hex2.Distance(neighbor, targetCoords);
                var height = neighbor.PerlinHeight(state.SeedAsFloat, state.Amplitude, state.NoiseScale, state.NoiseOffsetX, state.NoiseOffsetY);

                if (distance > furthestDistance && height >= state.MinWalkHeight)
                {
                    furthestDistance = distance;
                    bestNeighbor = neighbor;
                }
            }

            return bestNeighbor;
        }


        Hex2 MoveToFriendly(Actor actor, GameState state)
        {
            var team = state.GetTeamById(actor.OwnedByTeamId);
            var closestFriendly = FindClosestActor(actor, team);
            return MoveTowardsTarget(actor, closestFriendly.Coords, state);
        }

        Actor FindClosestActor(Actor currentActor, Team targetTeam)
        {
            Actor closestActor = null;
            var closestDistance = double.MaxValue;

            foreach (var actor in targetTeam.Actors)
            {
                if (actor == currentActor) continue;

                var distance = Hex2.Distance(currentActor.Coords, actor.Coords);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestActor = actor;
                }
            }

            return closestActor;
        }

        Hex2 MoveTowardsCenter(Actor actor, GameState state)
            => MoveTowardsTarget(actor, Hex2.Zero, state);

        Hex2 MoveTowardsTarget(Actor actor, Hex2 targetCoords, GameState state)
        {
            var currentCoords = actor.Coords;
            var neighbors = new List<Hex2>
            {
                new (currentCoords.ne + 1, currentCoords.se),
                new (currentCoords.ne, currentCoords.se + 1),
                new (currentCoords.e + 1, currentCoords.e + 1),
                new (currentCoords.ne - 1, currentCoords.se),
                new (currentCoords.ne, currentCoords.se - 1),
                new (currentCoords.e - 1, currentCoords.e - 1)
            };

            var validNeighbors = neighbors.Where(neighbor => !Hex2.OutOfBounds(neighbor, state.Radius)).ToList();

            if (!validNeighbors.Any()) return currentCoords;

            var bestNeighbor = currentCoords;
            var closestDistance = double.MaxValue;

            foreach (var neighbor in validNeighbors)
            {
                var distance = Hex2.Distance(neighbor, targetCoords);
                var height = actor.Coords.PerlinHeight(state.SeedAsFloat, state.Amplitude, state.NoiseScale, state.NoiseOffsetX, state.NoiseOffsetY);
                if (distance < closestDistance && height >= state.MinWalkHeight)
                {
                    closestDistance = distance;
                    bestNeighbor = neighbor;
                }
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestNeighbor = neighbor;
                }
            }

            return bestNeighbor;
        }

        Hex2 Wander(Hex2 currentCoords, int radius)
        {
            var random = new Random();
            var neighbors = new List<Hex2>
            {
                new (currentCoords.ne + 1, currentCoords.se),
                new (currentCoords.ne, currentCoords.se + 1),
                new (currentCoords.e + 1, currentCoords.e + 1),
                new (currentCoords.ne - 1, currentCoords.se),
                new (currentCoords.ne, currentCoords.se - 1),
                new (currentCoords.e - 1, currentCoords.e - 1)
            };

            var validNeighbors = neighbors.Where(neighbor => !Hex2.OutOfBounds(neighbor, radius)).ToList();
            return validNeighbors.Any()
                ? validNeighbors[random.Next(validNeighbors.Count)]
                : currentCoords;
        }
    }
}