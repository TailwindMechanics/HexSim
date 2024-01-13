using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace Modules.Server.NeuroNavigation.External
{
    public class NeuroAgent
    {
        readonly List<Vector3> closedSet = new();
        readonly List<NeuroNode> path = new();
        const float maxCostThreshold = 9f;

        public List<Vector3> BuildPath(Vector3 origin, Vector3 destination, Func<Vector3, List<Vector3>> getNeighbours, Func<Vector3, List<float>> costsAtPos, int maxCellSteps)
        {
            if (VectorsAreEqual(origin, destination))
            {
                // Debug.Log("<color=orange><b>>>> Origin and destination are the same</b></color>");
                return new List<Vector3> { origin };
            }

            closedSet.Clear();
            path.Clear();

            closedSet.Add(origin);
            ComputeNodes(origin, origin, destination, getNeighbours, costsAtPos, maxCellSteps);

            return path
                .Where(node => node.B.Sum() <= maxCostThreshold)
                .Select(node => node.Pos)
                .ToList();
        }

        void ComputeNodes(Vector3 current, Vector3 origin, Vector3 destination, Func<Vector3, List<Vector3>> getNeighbours, Func<Vector3, List<float>> costsAtPos, int maxCellSteps)
        {
            for (var currentStep = 0; currentStep <= maxCellSteps; currentStep++)
            {
                var neighbours = getNeighbours(current).Where(item => !ClosedContains(item)).ToList();
                if (neighbours.Count < 1)
                {
                    // Debug.Log("<color=orange><b>>>> No neighbours found</b></color>");
                    break;
                }

                var result = new NeuroNode(
                    currentStep,
                    neighbours[0],
                    origin,
                    destination,
                    costsAtPos
                );

                foreach (var pos in neighbours)
                {
                    var node = new NeuroNode(
                        currentStep,
                        pos,
                        origin,
                        destination,
                        costsAtPos
                    );

                    if (currentStep >= maxCellSteps)
                    {
                        path.Add(node);
                        continue;
                    }

                    if (node.F < result.F)
                    {
                        result = node;
                    }
                    else if (Math.Abs(node.F - result.F) < 0.01f && node.H < result.H)
                    {
                        result = node;
                    }
                }

                if (currentStep >= maxCellSteps)
                {
                    // Debug.Log("<color=orange><b>>>> Max range reached</b></color>");
                    break;
                }

                closedSet.AddRange(neighbours);
                path.Add(result);

                if (VectorsAreEqual(result.Pos, destination))
                {
                    // Debug.Log("<color=green><b>>>> Path found</b></color>");
                    break;
                }

                current = result.Pos;
            }
        }

        bool VectorsAreEqual(Vector3 v1, Vector3 v2, float tolerance = 0.01f)
            => (v1 - v2).sqrMagnitude < tolerance * tolerance;
        bool ClosedContains (Vector3 v1)
            => closedSet.Any(v2 => VectorsAreEqual(v1, v2));
    }
}