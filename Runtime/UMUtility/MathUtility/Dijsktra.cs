using System;
using System.Collections.Generic;
using System.Linq;
using UM.Runtime.UMUtility.CollectionUtility.CustomCollections;

namespace UM.Runtime.UMUtility.MathUtility
{
    public class Dijsktra<T>
    {
        private readonly Func<IEnumerable<T>> _getNodes;
        private readonly Func<T, T, float> _getWeight;
        private readonly Func<T, IEnumerable<T>> _getChildren;

        public Dijsktra(Func<IEnumerable<T>> getNodes, Func<T, IEnumerable<T>> getChildren, Func<T, T, float> getWeight)
        {
            _getNodes = getNodes;
            _getWeight = getWeight;
            _getChildren = getChildren;
        }
        
        public List<T> GetShortestPath(T start, T end)
        {
            Dict<T, DijsktraData> data = _getNodes().ToDict(x => x, x => new DijsktraData());
            DijkstraSearch(start, end, data);
            var shortestPath = new List<T>();
            shortestPath.Add(end);
            BuildShortestPath(shortestPath, data, end);
            shortestPath.Reverse();
            return shortestPath;
        }
        
        private void BuildShortestPath(List<T> list, Dict<T, DijsktraData> data, T node)
        {
            var nodeData = data[node];
            if (nodeData.nearestToStart == null)
                return;
            list.Add(nodeData.nearestToStart);
            BuildShortestPath(list, data, nodeData.nearestToStart);
        }
        private class DijsktraData
        {
            public bool visited;
            public float minCostToStart = -1;
            public T nearestToStart;
        }
        private void DijkstraSearch(T start, T end, Dict<T, DijsktraData> data)
        {
            var prioQueue = new List<T>();
            prioQueue.Add(start);
            do {
                prioQueue = prioQueue.OrderBy(x => data[x].minCostToStart).ToList();
                var node = prioQueue.First();
                prioQueue.Remove(node);
                var nodeData = data[node];
                foreach (var childNode in _getChildren(node).OrderBy(x => _getWeight(node, x)))
                {
                    var cost = _getWeight(node, childNode);
                    var childData = data[childNode];
                    if (childData.visited)
                        continue;
                    if (childData.minCostToStart < 0 ||
                        nodeData.minCostToStart + cost < childData.minCostToStart)
                    {
                        childData.minCostToStart = nodeData.minCostToStart + cost;
                        childData.nearestToStart = node;
                        if (!prioQueue.Contains(childNode))
                            prioQueue.Add(childNode);
                    }
                }
                nodeData.visited = true;
                if (node.Equals(end))
                    return;
            } while (prioQueue.Any());
        }
    }
}