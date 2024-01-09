using System.Collections.Generic;
using UnityEngine;
using System;

namespace Modules.Server.NeuroNavigation.External
{
    public class NeuroAgent
    {
        // Pathfinding: Data structures
        readonly Dictionary<Vector2Int, Vector2Int> cameFrom;
        readonly Dictionary<Vector2Int, float> gScore;
        readonly HashSet<Vector2Int> closedSet;
        readonly List<Vector2Int> openSet;

        // Perceptron: Agent-specific properties
        readonly List<float> weights;
        readonly List<Vector2Int> edgeNormals;

        public NeuroAgent(List<float> initialWeights, List<Vector2Int> edgeNormals)
        {
            cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            gScore = new Dictionary<Vector2Int, float>();
            closedSet = new HashSet<Vector2Int>();
            openSet = new List<Vector2Int>();
            weights = initialWeights;
            this.edgeNormals = edgeNormals;
        }

        // Pathfinding: Main method to find a path using the A* algorithm
        public List<Vector2Int> FindPath(Vector2Int startPosition, Vector2Int goalPosition, Func<Vector2Int, float[]> costsAtCoords)
        {
            InitializePathfinding(startPosition);

            while (openSet.Count > 0)
            {
                var currentNode = GetLowestFScoreNode(costsAtCoords);
                if (currentNode == goalPosition)
                    break;

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                ProcessNeighbors(currentNode, goalPosition, costsAtCoords);
            }

            UpdateWeightsBasedOnPerformance(goalPosition);
            return ReconstructPath(goalPosition);
        }

        // Pathfinding: Reconstructs the path from the goal position to the start
        List<Vector2Int> ReconstructPath(Vector2Int goalPosition)
        {
            var path = new List<Vector2Int>();
            var current = goalPosition;
            while (cameFrom.ContainsKey(current))
            {
                path.Add(current);
                current = cameFrom[current];
            }
            path.Reverse();
            return path;
        }

        // Pathfinding: Initializes or resets pathfinding variables
        void InitializePathfinding(Vector2Int startPosition)
        {
            openSet.Clear();
            closedSet.Clear();
            cameFrom.Clear();
            gScore.Clear();

            openSet.Add(startPosition);
            gScore[startPosition] = 0;
        }

        // Pathfinding: Process neighbors of the current node
        void ProcessNeighbors(Vector2Int currentNode, Vector2Int goalPosition, Func<Vector2Int, float[]> costsAtCoords)
        {
            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                var tentativeGScore = gScore[currentNode] + Distance(currentNode, neighbor);
                var inputs = new List<float> { tentativeGScore, HeuristicCostEstimate(neighbor, goalPosition) };
                inputs.AddRange(costsAtCoords(neighbor));

                var costWithInputs = CalculateNodeCost(inputs.ToArray());
                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                    gScore[neighbor] = float.MaxValue;
                }

                // Update gScore based on perceptron-calculated cost
                if (costWithInputs < gScore[neighbor])
                {
                    cameFrom[neighbor] = currentNode;
                    gScore[neighbor] = costWithInputs;
                }
            }
        }

        // Perceptron: Calculates the cost of a node based on inputs and weights
        float CalculateNodeCost(float[] inputs)
        {
            var cost = 0f;
            for (int i = 0; i < inputs.Length; i++)
            {
                cost += inputs[i] * weights[i];
            }

            return cost;
        }

        // Pathfinding: Finds the node with the lowest F score
        Vector2Int GetLowestFScoreNode(Func<Vector2Int, float[]> costsAtCoords)
        {
            var lowestNode = openSet[0];
            var lowestCost = float.MaxValue;

            foreach (var node in openSet)
            {
                var inputs = costsAtCoords(node);
                var cost = CalculateNodeCost(inputs);
                if (cost < lowestCost)
                {
                    lowestCost = cost;
                    lowestNode = node;
                }
            }

            return lowestNode;
        }

        // Perceptron: Updates weights based on the performance of the found path
        void UpdateWeightsBasedOnPerformance(Vector2Int goalPosition)
        {
            var pathCost = gScore[goalPosition];
            var initialEstimate = HeuristicCostEstimate(openSet[0], goalPosition);
            var weightChange = pathCost > initialEstimate * 1.5f ? 0.1f : -0.1f;
            AdjustAllWeights(weightChange);
        }

        // Perceptron: Adjusts all weights by a specified amount
        void AdjustAllWeights(float amount)
        {
            for (var i = 0; i < weights.Count; i++)
                weights[i] += amount;
        }

        // Pathfinding: Generates neighboring positions based on edge normals
        IEnumerable<Vector2Int> GetNeighbors(Vector2Int node)
        {
            var neighbors = new List<Vector2Int>();
            foreach (var edgeNormal in edgeNormals)
            {
                neighbors.Add(node + edgeNormal);
            }

            return neighbors;
        }

        // Utility methods for distance and heuristic estimation
        float Distance(Vector2Int a, Vector2Int b)
            => Vector2Int.Distance(a, b);
        float HeuristicCostEstimate(Vector2Int a, Vector2Int b)
            => Vector2Int.Distance(a, b);
    }
}