using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Codebase.Mechanics.PathfinderSystem
{
    public class SearchOnWidth
    {
        public static List<GraphNode> Search(Transform target, GraphNode startNode)
        {
            Queue<GraphNode> nextNodes = new Queue<GraphNode>();
            List<GraphNode> visitedNodes = new List<GraphNode>();
            List<float> distanceOnNodeToTarget = new List<float>();
            Dictionary<GraphNode, GraphNode> cameFrom = new Dictionary<GraphNode, GraphNode>();
            
            InitializeNode(startNode, target, ref visitedNodes, ref distanceOnNodeToTarget, ref nextNodes, ref cameFrom);
            
            while (nextNodes.Count > 0)
            {
                var node = nextNodes.Dequeue();
                if (!visitedNodes.Contains(node))
                {
                    InitializeNode(node, target, ref visitedNodes, ref distanceOnNodeToTarget, ref nextNodes, ref cameFrom);
                }
            }

            int minIndex = 0;
            for (int i = 0; i < distanceOnNodeToTarget.Count; i++)
            {
                if(distanceOnNodeToTarget[minIndex] > distanceOnNodeToTarget[i])
                    minIndex = i;
            }
            
            GraphNode targetNode = visitedNodes[minIndex];
            
            return ReconstructPath(cameFrom, startNode, targetNode);
        }
        
        private static void InitializeNode(GraphNode node, Transform target, ref List<GraphNode> visitedNodes,
            ref List<float> distanceOnNodeToTarget, ref Queue<GraphNode> nextNodes, ref Dictionary<GraphNode, GraphNode> cameFrom)
        {
            visitedNodes.Add(node);
            distanceOnNodeToTarget.Add(Vector3.Distance(target.position, node.transform.position));
            
            Debug.Log($"Node {node.name} has been visited. Distance to target: {Vector3.Distance(target.position, node.transform.position)}");
            
            foreach (var neighbor in node.ReachableNodes.ToList())
            {
                if (!cameFrom.ContainsKey(neighbor))
                {
                    cameFrom[neighbor] = node;
                }
                nextNodes.Enqueue(neighbor);
            }
        }
        
        private static List<GraphNode> ReconstructPath(Dictionary<GraphNode, GraphNode> cameFrom, GraphNode startNode, GraphNode targetNode)
        {
            List<GraphNode> path = new List<GraphNode>();
            GraphNode current = targetNode;
            
            while (current != startNode && current != null)
            {
                path.Add(current);
                if (cameFrom.ContainsKey(current))
                {
                    current = cameFrom[current];
                }
                else
                {
                    break;
                }
            }
            
            path.Add(startNode);
            path.Reverse();
            
            Debug.Log($"Path reconstructed with {path.Count} nodes");
            return path;
        }
    }
}