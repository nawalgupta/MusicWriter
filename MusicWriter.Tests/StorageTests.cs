using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.Tests
{
    public sealed class StorageTests
    {
        IStorageGraph graph;
        IStorageObject a;

        [TestInitialize]
        [TestMethod]
        public void CreateGraph_memory() {
            graph = new MemoryStorageGraph();
        }

        [TestMethod]
        public void MakeObjA() {
            a = graph.CreateObject();

            Assert.IsNotNull(a);
        }

        [TestMethod]
        public void ConnectObjA() {
            graph[graph.Root].Add("first-element", a.ID);

            Assert.IsTrue(graph.Outgoing(a.ID).Any(kvp => kvp.Key == "first-element" && kvp.Value == a.ID));
        }
    }
}
