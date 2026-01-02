using System.Collections.Generic;
using UnityEngine;

namespace Codebase.Mechanics.PathfinderSystem
{
    public class GraphNode : MonoBehaviour
    {
        [SerializeField] private GraphNode[] _reachableGraphs;
        public int GraphCount => _reachableGraphs.Length;
        public GraphNode[] ReachableNodes => _reachableGraphs;
        public Transform GetNodePosition => transform;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (GraphNode graph in _reachableGraphs)
            {
                Gizmos.DrawLine(transform.position, graph.transform.position);
                Gizmos.DrawSphere(graph.transform.position, 0.5f);
            }
        }
    }
}