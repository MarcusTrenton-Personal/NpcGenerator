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
        public bool AddNode(T node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (m_nodeEdges.ContainsKey(node))
            {
                return false;
            }

            m_nodeEdges[node] = new HashSet<T>();
            return true;
        }

        public bool AddEdge(T start, T end)
        {
            if (start is null)
            {
                throw new ArgumentNullException(nameof(start));
            }
            if (end is null)
            {
                throw new ArgumentNullException(nameof(end));
            }

            AddNodeIfNotAlreadyContained(start);
            AddNodeIfNotAlreadyContained(end);

            if (ReferenceEquals(start, end) || start.Equals(end))
            {
                return false;
            }

            bool successful = m_nodeEdges[start].Add(end);
            return successful;
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

        private bool FindCycle(T startingNode, Dictionary<T, CycleMarker> markers)
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

        private class CycleMarker
        {
            public bool Visited { get; set; } = false;
            public bool InRecursionPath { get; set; } = false;
        }

        private readonly Dictionary<T,HashSet<T>> m_nodeEdges = new Dictionary<T,HashSet<T>>();
    }
}
