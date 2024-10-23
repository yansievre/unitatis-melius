using System;
using System.Collections.Generic;
using System.Linq;
using UM.Runtime.UMUtility.MathUtility;
using Unity.Burst;

namespace UM.Runtime.UMUtility.CollectionUtility.CustomCollections
{
    public class SimpleGraph<T>
    {
        private const int MaxLoops = 500;
        private class Node
        {
            public T Value;
            public List<Node> Connections = new List<Node>();

            public Node(T value)
            {
                Value = value;
            }
        }

        private string _initialConnections = "";
        
        private Dict<T, Node> _nodes = new Dict<T, Node>();
        
        private Node GetNode(T value)
        {
            return _nodes.TryGetValue(value, out var node) ? node : null;
        }

        public IEnumerable<T> GetConnections(T value)
        {
            var node = GetNode(value);
            return node?.Connections.Select(x => x.Value);
        }
        
        public IEnumerable<T> GetNodes()
        {
            return _nodes.Keys;
        }
        
        public void AddConnection(T from, T to)
        {
            var fromNode = GetNode(from);
            var toNode = GetNode(to);
            if(fromNode==null) _nodes.Add(from, fromNode=new Node(from));
            if (toNode == null)
            {
                if (from.Equals(to))
                {
                    toNode = fromNode;
                }
                else
                {
                    _nodes.Add(to, toNode=new Node(to));
                }
            }
            fromNode.Connections.Add(toNode);
            toNode.Connections.Add(fromNode);
            _initialConnections += $"{from} <-> {to} : ";
        }

        public void RemoveNode(T nodeValue)
        {
            var node = GetNode(nodeValue);
            if (node == null) return;
            _nodes.Remove(nodeValue);
        }
        public void RemoveConnection(T from, T to)
        {
            var fromNode = GetNode(from);
            var toNode = GetNode(to);
            if (fromNode == null) return;
            if(toNode==null)  return;
            if (!fromNode.Connections.Contains(toNode)) return;
            fromNode.Connections.Remove(toNode);
        }
        
        public override string ToString()
        {
            var st = "";
            foreach (var node in _nodes.Values)
            {
                foreach (var ch in node.Connections)
                {
                    st += $"{node.Value} -> {ch.Value} : ";
                }
            }
            return st;
        }
        
        // Leftover from directional graph
        // public bool CheckForCircularDependency(bool selfConnectionsAllowed,ref UnorderedPair<T> invalidConnection)
        // {
        //     invalidConnection = UnorderedPair<T>.Empty;
        //     HashSet<T> visited = new HashSet<T>();
        //     foreach (var root in _roots)
        //     {
        //         visited.Clear();
        //         var result = VisitNode(null,root, ref visited, ref invalidConnection);
        //         if (result) return true;
        //     }
        //
        //     bool VisitNode(Node visitorParent, Node node, ref HashSet<T> visitedSet,ref UnorderedPair<T> foundInvalidConnection)
        //     {
        //         if (visitedSet.Contains(node.Value))
        //         {
        //             foundInvalidConnection = UnorderedPair.Create<T>(visitorParent.Value,node.Value);
        //             return true;
        //         }
        //         visitedSet.Add(node.Value);
        //         foreach (var childNode in node.Children)
        //         {
        //             if (selfConnectionsAllowed && childNode.Value.Equals(node.Value))
        //             {
        //                 continue;
        //             }
        //             var res = VisitNode(node,childNode, ref visitedSet,ref  foundInvalidConnection);
        //             if (res) return true;
        //         }
        //
        //         return false;
        //     }
        //     return false;
        // }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selfConnectionsAllowed"></param>
        /// <returns>Found and fixed</returns>
        // public bool FixCircularDependency(bool selfConnectionsAllowed)
        // {
        //     UnorderedPair<T> invalidConnection = UnorderedPair<T>.Empty;
        //     bool didFind = false;
        //     int counter = 0;
        //     while (CheckForCircularDependency(selfConnectionsAllowed, ref invalidConnection))
        //     {
        //         RemoveConnection(invalidConnection.first,invalidConnection.second);
        //         didFind = true;
        //         counter++;
        //         if (counter >= MaxLoops)
        //         {
        //             throw new Exception("Infinite loop?");
        //         }
        //     }
        //
        //     return didFind;
        // }

        public UnorderedPair<T>[] GetConnections()
        {
            List<UnorderedPair<T>> connections = new List<UnorderedPair<T>>();
            foreach (var node in _nodes.Values)
            {
                foreach (var child in node.Connections)
                {
                    connections.Add(UnorderedPair<T>.Create(node.Value, child.Value));
                }
            }

            return connections.ToArray();
        }
        
        public List<T> GetShortestPathDijkstra(T start, T end)
        {
            var dijsktra = new Dijsktra<T>(GetNodes,  GetConnections, (_, _) => 1);
            return dijsktra.GetShortestPath(start, end);
        }
        
    }
}