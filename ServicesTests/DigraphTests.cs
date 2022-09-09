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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestClass]
    public class DigraphTests
    {
        [TestMethod]
        public void AddNewIntNode()
        {
            Digraph<int> graph = new Digraph<int>();

            bool success = graph.AddNode(1);
            
            Assert.IsTrue(success, "Failed to add node to empty graph");
        }

        [TestMethod]
        public void AddNewObjectNode()
        {
            Digraph<TestObject> graph = new Digraph<TestObject>();

            bool success = graph.AddNode(new TestObject());

            Assert.IsTrue(success, "Failed to add node to empty graph");
        }

        [TestMethod]
        public void AddMultipleNodes()
        {
            Digraph<float> graph = new Digraph<float>();

            float[] values = new float[] { 1.5f, 2.4f, 3.3f, 4.2f, 5.1f };
            foreach (float value in values)
            {
                bool success = graph.AddNode(value);
                Assert.IsTrue(success, "Failed to add node to graph");
            }
        }

        [TestMethod]
        public void AddRedundantNode()
        {
            Digraph<string> graph = new Digraph<string>();

            const string VALUE = "Chocolate";
            graph.AddNode(new string(VALUE));
            bool success = graph.AddNode(new string(VALUE));

            Assert.IsFalse(success, "Incorrectly added redundant node");
        }

        [TestMethod]
        public void AddNullNode()
        {
            Digraph<StringBuilder> graph = new Digraph<StringBuilder>();

            bool threwExcepion = false;
            try
            {
                bool success = graph.AddNode(null);
            }
            catch (ArgumentNullException)
            {
                threwExcepion = true;
            }

            Assert.IsTrue(threwExcepion, "Failed to throw ArgumentNullException");
        }

        [TestMethod]
        public void AddEdgeNullStart()
        {
            Digraph<string> graph = new Digraph<string>();

            bool threwExcepion = false;
            try
            {
                bool success = graph.AddEdge(start: null, end: "Dinosaur");
            }
            catch (ArgumentNullException)
            {
                threwExcepion = true;
            }

            Assert.IsTrue(threwExcepion, "Failed to throw ArgumentNullException");
        }

        [TestMethod]
        public void AddEdgeNullEnd()
        {
            Digraph<string> graph = new Digraph<string>();

            bool threwExcepion = false;
            try
            {
                bool success = graph.AddEdge(start: "World", end: null);
            }
            catch (ArgumentNullException)
            {
                threwExcepion = true;
            }

            Assert.IsTrue(threwExcepion, "Failed to throw ArgumentNullException");
        }

        [TestMethod]
        public void AddEdgeToSelf()
        {
            Digraph<string> graph = new Digraph<string>();

            const string VALUE = "World";
            bool success = graph.AddEdge(start: VALUE, end: VALUE);

            Assert.IsFalse(success, "Incorrectly added a node with an edge to itself");
        }

        [TestMethod]
        public void AddEdge()
        {
            Digraph<string> graph = new Digraph<string>();

            bool success = graph.AddEdge(start: "Dinosaur", end: "Civilization");

            Assert.IsTrue(success, "Failed to add an edge");
        }

        [TestMethod]
        public void DetectIntCycleWithOnlyIsolatedNodes()
        {
            Digraph<int> graph = new Digraph<int>();

            graph.AddNode(0);
            graph.AddNode(1);
            graph.AddNode(2);
            graph.AddNode(3);

            bool hasCycle = graph.HasCycle(out List<int> path);

            Assert.IsFalse(hasCycle, "Incorrectly detected cycles in a graph of isolated nodes");
            Assert.IsNull(path, "Graph without cycles should return a null path");
        }

        [TestMethod]
        public void DetectIntCycleTrue()
        {
            Digraph<int> graph = new Digraph<int>();

            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 0);
            graph.AddEdge(2, 3);

            bool hasCycle = graph.HasCycle(out List<int> path);

            Assert.IsTrue(hasCycle, "Failed detect a cycle in a graph with a cycle");
            for (int i = 0; i < path.Count; i++)
            {
                int initialValue = path[i];
                int nextValue = path[(i + 1) % path.Count];
                bool isIncrementing = initialValue + 1 == nextValue;
                bool isWrapAround = initialValue == 2 && nextValue == 0;
                bool isClosedLoop = initialValue == nextValue;
                Assert.IsTrue(isIncrementing || isWrapAround || isClosedLoop, 
                    "Return path has incorrect link: " + initialValue + " -> " + nextValue);
            }
        }

        [TestMethod]
        public void DetectCycleFalse()
        {
            Digraph<int> graph = new Digraph<int>();

            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 4);
            graph.AddEdge(2, 3);

            bool hasCycle = graph.HasCycle(out List<int> path);

            Assert.IsFalse(hasCycle, "Incorrectly detected cycles in a graph without a cycle");
            Assert.IsNull(path, "Graph without cycles should return a null path");
        }

        [TestMethod]
        public void DetectCycleTrueWithMixedIsolatedNodes()
        {
            Digraph<int> graph = new Digraph<int>();

            graph.AddNode(-1);
            graph.AddNode(-2);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 0);
            graph.AddEdge(2, 3);
            graph.AddNode(-3);

            bool hasCycle = graph.HasCycle(out List<int> path);

            Assert.IsTrue(hasCycle, "Failed detect a cycle in a graph with a cycle");
            for (int i = 0; i < path.Count; i++)
            {
                int initialValue = path[i];
                int nextValue = path[(i + 1) % path.Count];
                bool isIncrementing = initialValue + 1 == nextValue;
                bool isWrapAround = initialValue == 2 && nextValue == 0;
                bool isClosedLoop = initialValue == nextValue;
                Assert.IsTrue(isIncrementing || isWrapAround || isClosedLoop,
                    "Return path has incorrect link: " + initialValue + " -> " + nextValue);
            }
        }

        [TestMethod]
        public void DetectObjectCycleTrue()
        {
            Digraph<string> graph = new Digraph<string>();

            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");
            graph.AddEdge("C", "A");
            graph.AddEdge("C", "D");

            bool hasCycle = graph.HasCycle(out List<string> path);

            Assert.IsTrue(hasCycle, "Failed detect a cycle in a graph with a cycle");
            for (int i = 0; i < path.Count; i++)
            {
                string initialValue = path[i];
                string nextValue = path[(i + 1) % path.Count];
                bool isAB = initialValue == "A" && nextValue == "B";
                bool isBC = initialValue == "B" && nextValue == "C";
                bool isCA = initialValue == "C" && nextValue == "A";
                bool isClosedLoop = initialValue == nextValue;
                Assert.IsTrue(isAB || isBC || isCA || isClosedLoop,
                    "Return path has incorrect link: " + initialValue + " -> " + nextValue);
            }
        }

        private struct TestObject
        {
#pragma warning disable CS0169 // Field is never used. Don't care as it's a test object. 
            int x;
            string y;
#pragma warning restore CS0169 // Field is never used
        }
    }
}
