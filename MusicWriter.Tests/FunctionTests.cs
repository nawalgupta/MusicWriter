using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace MusicWriter.Tests
{
    [TestClass]
    public sealed class FunctionTests
    {
        FunctionCodeTools code;

        IStorageGraph graph;
        IStorageObject obj1;
        IStorageObject obj2;
        AssortedFilesManager assortedfiles;

        [TestMethod]
        public void TestCreateGraph() {
            graph = new MemoryStorageGraph();
            obj1 = graph.CreateObject();
            obj2 = graph.CreateObject();
            assortedfiles = new AssortedFilesManager(graph.CreateObject());
        }

        [TestMethod]
        [TestInitialize]
        public void TestCreateCode() {
            code = new FunctionCodeTools();

            code.Factories.Add(SquareFunction.FactoryClass.Instance);
            code.Factories.Add(PolylineFunction.FactoryClass.Instance);
            code.Factories.Add(PolynomialFunction.FactoryClass.Instance);
            code.Factories.Add(StepwiseIntegratedFunction.FactoryClass.Instance);
            code.Factories.Add(GlobalPerspectiveFunction.FactoryClass.Instance);
            code.Factories.Add(LocalPerspectiveFunction.FactoryClass.Instance);

            TestCreateGraph();
        }

        [TestMethod]
        public void TestPolyline_1() {
            var polyline = new PolylineData(obj1);

            polyline.Add(0, 0);
            polyline.Add(1, 2);

            Assert.AreEqual(polyline.GetValue(1), 2f);
            Assert.AreEqual(polyline.GetValue(0.5f), 1f);
            Assert.AreEqual(polyline.GetValue(0), 0f);

            polyline.Add(0.5f, 1.5f);
            
            Assert.AreEqual(polyline.GetValue(1), 2f);
            Assert.AreEqual(polyline.GetValue(0.75f), 1.75f);
            Assert.AreEqual(polyline.GetValue(0.5f), 1.5f);
            Assert.AreEqual(polyline.GetValue(0.25f), 0.75f);
            Assert.AreEqual(polyline.GetValue(0), 0f);

            polyline.Add(1.25f, 1);

            Assert.AreEqual(polyline.GetValue(1.0625f), 1.75f);
        }

        [TestMethod]
        public void TestPolyline_2() {
            var polyline = new PolylineData(obj2);

            //         XXXX
            //       XXXXXXXXXXX
            // XXXXXXXXXXXXXXXXX
            // 0   1   2   3   4

            polyline.Add(0, 1);
            polyline.Add(1, 1);
            polyline.Add(2, 3);
            polyline.Add(4, 2);

            Assert.AreEqual(polyline.GetIntegratedValue(0), 0);
            Assert.AreEqual(polyline.GetIntegratedValue(1), 1);
            Assert.AreEqual(polyline.GetIntegratedValue(2) - polyline.GetIntegratedValue(1), 2);
            Assert.AreEqual(polyline.GetIntegratedValue(4), 8);

            float y;
            Assert.IsTrue(polyline.GetInvertedIntegratedValue(1.75f, out y));
            Assert.AreEqual(y, 1.5f);
        }

        [TestMethod]
        public void TestCodeRendering() {
            var func =
                new GlobalPerspectiveFunction(new SquareFunction());

            var rendered =
                new StringBuilder();

            code.Render(rendered, func, assortedfiles, null);

            Assert.AreEqual(rendered.ToString(), "square time.global");
        }
    }
}
