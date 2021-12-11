using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class SimpleGraph<T>
    {
        private const int MaxLoops = 500;
        private class Node
        {
            public T Value;
            public List<Node> Parents = new List<Node>();
            public List<Node> Children = new List<Node>();

            public Node(T value)
            {
                Value = value;
            }
        }

        private string _initialConnections = "";
        
        private List<Node> _nodes = new List<Node>();
        private List<Node> _roots = new List<Node>();

        public void AddConnection(T from, T to)
        {
            var fromNode = _nodes.FirstOrDefault(x => x.Value.Equals(from));
            var toNode = _nodes.FirstOrDefault(x => x.Value.Equals(to));
            if(fromNode==null) _nodes.Add(fromNode=new Node(from));
            if (toNode == null)
            {
                if (from.Equals(to))
                {
                    toNode = fromNode;
                }
                else
                {
                    _nodes.Add(toNode=new Node(to));
                }
            }
            fromNode.Children.Add(toNode);
            toNode.Parents.Add(fromNode);
            if(fromNode.Parents.Count==0 && !_roots.Contains(fromNode)) _roots.Add(fromNode);
            if(fromNode.Parents.Count!=0 && _roots.Contains(fromNode)) _roots.Remove(fromNode);
            if(_roots.Contains(toNode)) _roots.Remove(toNode);
            _initialConnections += $"{from} -> {to} : ";
        }

        public void RemoveNode(T nodeValue)
        {
            var node = _nodes.FirstOrDefault(x => x.Value.Equals(nodeValue));
            if (node == null) return;
            _nodes.Remove(node);
            if (_roots.Contains(node)) _roots.Remove(node);
        }
        public void RemoveConnection(T from, T to)
        {
            var fromNode = _nodes.FirstOrDefault(x => x.Value.Equals(from));
            var toNode = _nodes.FirstOrDefault(x => x.Value.Equals(to));
            if (fromNode == null) return;
            if(toNode==null)  return;
            if (!fromNode.Children.Contains(toNode) || !toNode.Parents.Contains(fromNode)) return;
            fromNode.Children.Remove(toNode);
            if(fromNode.Children.Count==0 && fromNode.Parents.Count==0) RemoveNode(fromNode.Value);
            toNode.Parents.Remove(fromNode);
            if(toNode.Children.Count==0 && toNode.Parents.Count==0) RemoveNode(toNode.Value);
        }

        public T[] GetRoots()
        {
            return _roots.Select(x=>x.Value).ToArray();
        }

        public override string ToString()
        {
            var st = "";
            foreach (var node in _nodes)
            {
                foreach (var ch in node.Children)
                {
                    st += $"{node.Value} -> {ch.Value} : ";
                }
            }
            return st;
        }

        public bool CheckForCircularDependency(bool selfConnectionsAllowed,ref Tuple<T,T> invalidConnection)
        {
            invalidConnection = Tuple.Create<T,T>(default, default);
            HashSet<T> visited = new HashSet<T>();
            foreach (var root in _roots)
            {
                visited.Clear();
                var result = VisitNode(null,root, ref visited, ref invalidConnection);
                if (result) return true;
            }

            bool VisitNode(Node visitorParent, Node node, ref HashSet<T> visitedSet,ref Tuple<T,T> foundInvalidConnection)
            {
                if (visitedSet.Contains(node.Value))
                {
                    foundInvalidConnection = Tuple.Create<T,T>(visitorParent.Value,node.Value);
                    return true;
                }
                visitedSet.Add(node.Value);
                foreach (var childNode in node.Children)
                {
                    if (selfConnectionsAllowed && childNode.Value.Equals(node.Value))
                    {
                        continue;
                    }
                    var res = VisitNode(node,childNode, ref visitedSet,ref  foundInvalidConnection);
                    if (res) return true;
                }

                return false;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selfConnectionsAllowed"></param>
        /// <returns>Found and fixed</returns>
        public bool FixCircularDependency(bool selfConnectionsAllowed)
        {
            Tuple<T, T> invalidConnection = null;
            bool didFind = false;
            int counter = 0;
            while (CheckForCircularDependency(selfConnectionsAllowed, ref invalidConnection))
            {
                RemoveConnection(invalidConnection.Item1,invalidConnection.Item2);
                didFind = true;
                counter++;
                if (counter >= MaxLoops)
                {
                    throw new Exception("Infinite loop?");
                    return false;
                }
            }

            return didFind;
        }

        public Tuple<T, T>[] GetConnections()
        {
            List<Tuple<T, T>> connections = new List<Tuple<T, T>>();
            foreach (var node in _nodes)
            {
                foreach (var child in node.Children)
                {
                    connections.Add(Tuple.Create(node.Value,child.Value));
                }
            }

            return connections.ToArray();
        }
    }
}