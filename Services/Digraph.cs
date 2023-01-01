/*Copyright(C) 2022 Marcus Trenton, marcus.trenton@gmail.com

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <https://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;

namespace Services
{
    public class Digraph<T> where T : notnull
    {
        public bool AddNode(in T node)
        {
            ParamUtil.VerifyNotNull(nameof(node), node);

            if (m_nodeEdges.ContainsKey(node))
            {
                return false;
            }

            m_nodeEdges[node] = new HashSet<T>();
            return true;
        }

        public bool AddEdge(in T start, in T end)
        {
            return AddEdge(start, end, 1);
        }

        public bool AddEdge(in T start, in T end, int weight)
        {
            ParamUtil.VerifyNotNull(nameof(start), start);
            ParamUtil.VerifyNotNull(nameof(end), end);
            ParamUtil.VerifyWholeNumber(nameof(weight), weight);

            AddNodeIfNotAlreadyContained(start);
            AddNodeIfNotAlreadyContained(end);

            if (AreEqual(start, end))
            {
                return false;
            }

            bool successful = m_nodeEdges[start].Add(end);
            if (successful)
            {
                m_edgeWeight[(start, end)] = weight;
            }
            return successful;
        }

        private bool AreEqual(T a, T b)
        {
            return ReferenceEquals(a, b) || a.Equals(b);
        }

        private void AddNodeIfNotAlreadyContained(T node)
        {
            bool hasNode = m_nodeEdges.ContainsKey(node);
            if (!hasNode)
            {
                AddNode(node);
            }
        }

        public bool HasCycle(out List<T> path)
        {
            path = null;

            Dictionary<T, CycleMarker> markers = new Dictionary<T, CycleMarker>(m_nodeEdges.Count);
            foreach (T node in m_nodeEdges.Keys)
            {
                markers[node] = new CycleMarker();
            }

            foreach (T node in m_nodeEdges.Keys)
            {
                bool visited = markers[node].Visited;
                if (!visited)
                {
                    bool hasCycle = FindCycle(node, markers);
                    if (hasCycle)
                    {
                        path = CyclePath(markers);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool FindCycle(in T startingNode, in Dictionary<T, CycleMarker> markers)
        {
            CycleMarker marker = markers[startingNode];
            if (!marker.Visited)
            {
                marker.Visited = true;
                marker.InRecursionPath = true;

                foreach (T adjacentNode in m_nodeEdges[startingNode])
                {
                    if (!markers[adjacentNode].Visited)
                    {
                        bool isCycle = FindCycle(adjacentNode, markers);
                        if (isCycle)
                        {
                            return true;
                        }
                    }
                    else if (markers[adjacentNode].InRecursionPath)
                    {
                        return true;
                    }
                }
            }

            marker.InRecursionPath = false;
            return false;
        }

        private List<T> CyclePath(Dictionary<T, CycleMarker> markers)
        {
            List<T> path = new List<T>();
            foreach (T node in m_nodeEdges.Keys)
            {
                if (markers[node].InRecursionPath)
                {
                    T nextNode = node;
                    do
                    {
                        path.Add(nextNode);
                        foreach (T candidateNode in m_nodeEdges[nextNode])
                        {
                            if (markers[candidateNode].InRecursionPath)
                            {
                                nextNode = candidateNode;
                                break;
                            }
                        }
                    }
                    while (!path.Contains(nextNode));
                    path.Add(nextNode); //Close the loop
                }
                break;
            }
            return path;
        }

        //Traverse all nodes a single time. Edges represent prerequisites.
        //A node can only be visited when all nodes with edges into it have already been visited.
        //An example of this traversal is researching techs in a tech tree.
        //This traversal will throw an exception if the Digraph has a cycle.
        public List<T> GetPrerequisiteTraversalPath()
        {
            bool hasCycle = HasCycle(out _);
            if (hasCycle)
            {
                throw new InvalidOperationException("Digraph has a cycle, so cannot do prerequisite traversal");
            }

            List<T> path = new List<T>();
            HashSet<T> toVisit = new HashSet<T>(m_nodeEdges.Keys);
            while (toVisit.Count > 0)
            {
                bool foundNextNode = false;
                foreach (T candidate in toVisit)
                {
                    bool canVisit = AllPrerequisitesMet(candidate, path);
                    if (canVisit)
                    {
                        foundNextNode = true;
                        path.Add(candidate);
                        toVisit.Remove(candidate);
                        break;
                    }
                }
                if (!foundNextNode)
                {
                    throw new InvalidOperationException("Cannot find traversal candidate in graph with no cycles. This should be impossible.");
                }
            }
            return path;
        }

        private bool AllPrerequisitesMet(T candidate, List<T> visited)
        {
            foreach (KeyValuePair<T, HashSet<T>> nodeEdges in m_nodeEdges)
            {
                if (candidate.Equals(nodeEdges.Key))
                {
                    continue;
                }

                bool candidateHasPrerequisite = nodeEdges.Value.Contains(candidate);
                T prerequisite = nodeEdges.Key;
                bool prequisiteIsVisited = visited.Contains(prerequisite);
                if (candidateHasPrerequisite && !prequisiteIsVisited)
                {
                    return false;
                }
            }

            return true;
        }

        //Return a list of nodes reachable from the start, including the start, by traversing edges.
        //This traversal will throw an exception if the Digraph has a cycle.
        public HashSet<T> NodesReachableFrom(T start)
        {
            if (start is null)
            {
                throw new ArgumentNullException(nameof(start));
            }
            if (!m_nodeEdges.ContainsKey(start))
            {
                throw new ArgumentException("Initial node must be in the diagraph");
            }

            HashSet<T> visited = new HashSet<T>();
            AddReachableNodesFrom(start, visited);
            return visited;
        }

        private void AddReachableNodesFrom(T start, HashSet<T> visited)
        {
            bool isNew = visited.Add(start);
            if (isNew)
            {
                foreach (T node in m_nodeEdges[start])
                {
                    AddReachableNodesFrom(node, visited);
                }
            }
        }

        public List<T> ShortestPathBetween(T start, T end, out int distance)
        {
            if (start is null)
            {
                throw new ArgumentNullException(nameof(start));
            }
            if (end is null)
            {
                throw new ArgumentNullException(nameof(end));
            }
            if (!m_nodeEdges.ContainsKey(start))
            {
                throw new ArgumentException(nameof(start) + " is not in the graph");
            }
            if (!m_nodeEdges.ContainsKey(end))
            {
                throw new ArgumentException(nameof(end) + " is not in the graph");
            }

            if (AreEqual(start, end))
            {
                distance = 0;
                return new List<T>() { start } ;
            }

            DijkstraDistanceAlgorithm(start, out Dictionary<T,int> distanceFromStart, out Dictionary<T,T> pathBack);

            if (!pathBack.ContainsKey(end))
            {
                distance = int.MinValue;
                return null;
            }

            distance = distanceFromStart[end];

            List<T> path = new List<T>();
            T i = end;
            while(!AreEqual(i, start))
            {
                path.Add(i);
                i = pathBack[i];
            }
            path.Add(i);
            path.Reverse();
            return path;
        }

        //Algorithm modified from https://www.geeksforgeeks.org/shortest-path-in-a-directed-graph-by-dijkstras-algorithm/
        private void DijkstraDistanceAlgorithm(T start, out Dictionary<T,int> distanceFromStart, out Dictionary<T,T> pathBack)
        {
            distanceFromStart = new Dictionary<T,int>(m_nodeEdges.Count);
            pathBack = new Dictionary<T,T>(m_nodeEdges.Count);
            Dictionary<T,bool> visited = new Dictionary<T,bool>(m_nodeEdges.Count);
            foreach (T node in m_nodeEdges.Keys)
            {
                visited[node] = false;
                distanceFromStart[node] = int.MaxValue;
            }
            distanceFromStart[start] = 0;

            T current = start;
            HashSet<T> toSearch = new HashSet<T>();
            while (true)
            {
                visited[current] = true;
                foreach (T adjacentNode in m_nodeEdges[current])
                {
                    if (visited[adjacentNode])
                    {
                        continue;
                    }

                    toSearch.Add(adjacentNode);
                    int candidateDistance = distanceFromStart[current] + m_edgeWeight[(current, adjacentNode)];
                    if (candidateDistance < distanceFromStart[adjacentNode])
                    {
                        distanceFromStart[adjacentNode] = candidateDistance;
                        pathBack[adjacentNode] = current;
                    }
                }

                toSearch.Remove(current);
                if (toSearch.Count == 0)
                {
                    break;
                }

                int minimumDistance = int.MaxValue;
                T next = default;

                foreach (T node in toSearch)
                {
                    if (distanceFromStart[node] < minimumDistance)
                    {
                        minimumDistance = distanceFromStart[node];
                        next = node;
                    }
                }
                current = next;
            }
        }

        private class CycleMarker
        {
            public bool Visited { get; set; } = false;
            public bool InRecursionPath { get; set; } = false;
        }

        //Keep the weights in a separate container for performance reasons.
        //Using a complex object for an edge combining destination node and a weight results in unneeded allocation when determining
        //if a node has an edge to a destinaion.
        private readonly Dictionary<T,HashSet<T>> m_nodeEdges = new Dictionary<T,HashSet<T>>();
        private readonly Dictionary<(T, T), int> m_edgeWeight = new Dictionary<(T, T), int>();
    }
}
