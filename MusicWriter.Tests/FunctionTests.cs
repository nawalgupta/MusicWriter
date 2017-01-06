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
        IStorageObject obj;
        AssortedFilesManager assortedfiles;

        [TestMethod]
        public void TestCreateGraph() {
            graph = new MemoryStorageGraph();
            obj = graph.CreateObject();
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
            code.Factories.Add(GloballyPerspectiveFunction.FactoryClass.Instance);

            TestCreateGraph();
        }

        [TestMethod]
        public void TestPolyline_1() {
            var polyline = new PolylineFunction(obj);

            polyline.Add(0, 0);
            polyline.Add(1, 2);

            Assert.AreEqual(polyline.GetValue(new FunctionCall(1)), 2f);
            Assert.AreEqual(polyline.GetValue(new FunctionCall(0.5f)), 1f);
            Assert.AreEqual(polyline.GetValue(new FunctionCall(0)), 0f);

            polyline.Add(0.5f, 1.5f);
            
            Assert.AreEqual(polyline.GetValue(new FunctionCall(1)), 2f);
            Assert.AreEqual(polyline.GetValue(new FunctionCall(0.75f)), 1.75f);
            Assert.AreEqual(polyline.GetValue(new FunctionCall(0.5f)), 1.5f);
            Assert.AreEqual(polyline.GetValue(new FunctionCall(0.25f)), 0.75f);
            Assert.AreEqual(polyline.GetValue(new FunctionCall(0)), 0f);

            polyline.Add(1.25f, 1);

            Assert.AreEqual(polyline.GetValue(new FunctionCall(1.0625f)), 1.75f);
        }

        [TestMethod]
        public void TestCodeRendering() {
            var func =
                new GloballyPerspectiveFunction(new SquareFunction());

            var rendered =
                new StringBuilder();

            code.Render(rendered, func, assortedfiles, null);

            Assert.AreEqual(rendered.ToString(), "square global");
        }
    }
}
