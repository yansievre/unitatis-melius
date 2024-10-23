using System;
using System.Collections.Generic;
using System.Linq;
using UM.Runtime.UMUtility.MathUtility;
using Unity.Burst;

namespace UM.Runtime.UMUtility.CollectionUtility.CustomCollections
{
    /// <summary>
    /// Connections are unordered
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeightedGraph<T>
    {
        private SimpleGraph<T> _simpleGraph;
        private Dictionary<UnorderedPair<T>, float> _connectionWeights;
        
        public WeightedGraph()
        {
            _simpleGraph = new SimpleGraph<T>();
        }
        
        public UnorderedPair<T>[] GetConnections()
        {
            return _simpleGraph.GetConnections();
        }

        public void AddConnection(T from, T to, float weight)
        {
            _simpleGraph.AddConnection(from, to);
            _connectionWeights[new UnorderedPair<T>(from, to)] = weight;
        }

        public void RemoveNode(T nodeValue)
        {
            _simpleGraph.RemoveNode(nodeValue);
            _connectionWeights = _connectionWeights.Where(x => !x.Key.Contains(nodeValue)).ToDictionary(x => x.Key, x => x.Value);
        }

        public void RemoveConnection(T from, T to)
        {
            _simpleGraph.RemoveConnection(from, to);
            _connectionWeights.Remove(new UnorderedPair<T>(from, to));
        }

        public IEnumerable<T> GetNodes()
        {
            return _simpleGraph.GetNodes();
        }
        
        public float GetWeight(T from, T to)
        {
            return _connectionWeights[new UnorderedPair<T>(from, to)];
        }

        
        public IEnumerable<T> GetConnections(T value)
        {
            return _simpleGraph.GetConnections(value);
        }
        
        public List<T> GetShortestPathDijkstra(T start, T end)
        {
            var dijsktra = new Dijsktra<T>(GetNodes,  GetConnections, GetWeight);
            return dijsktra.GetShortestPath(start, end);
        }
    }
}