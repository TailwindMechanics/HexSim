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

        public List<Vector3> BuildPath(Vector3 origin, Vector3 destination, Func<Vector3, List<Vector3>> getNeighbours, Func<Vector3, float> heightAtPos, int maxRange)
        {
            if (VectorsAreEqual(origin, destination))
            {
                // Debug.Log("<color=orange><b>>>> Origin and destination are the same</b></color>");
                return new List<Vector3> { origin };
            }

            closedSet.Clear();
            path.Clear();

            closedSet.Add(origin);
            ComputeNodes(origin, origin, destination, getNeighbours, maxRange);

            return path.Select(node => node.Pos).ToList();
        }

        void ComputeNodes(Vector3 current, Vector3 origin, Vector3 destination, Func<Vector3, List<Vector3>> getNeighbours, int maxSteps)
        {
            for (var currentStep = 0; currentStep <= maxSteps; currentStep++)
            {
                var neighbours = getNeighbours(current).Where(item => !ClosedContains(item)).ToList();
                var result = new NeuroNode(currentStep, neighbours[0], origin, destination);

                foreach (var pos in neighbours)
                {
                    var node = new NeuroNode(currentStep, pos, origin, destination);
                    if (currentStep >= maxSteps)
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

                if (currentStep >= maxSteps)
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