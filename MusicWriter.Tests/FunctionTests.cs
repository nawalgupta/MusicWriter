using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace MusicWriter.Tests
{
    [TestClass]
    public sealed class FunctionTests
    {
        FunctionCodeTools code;
        EditorFile file;

        IStorageGraph graph;
        IStorageObject obj1;
        IStorageObject obj2;

        [TestMethod]
        public void TestCreateGraph() {
            graph = new MemoryStorageGraph();
            var factoryset = new FactorySet<IContainer>();

            factoryset
                .Factories
                .Add(
                        PolylineContainer.CreateFactory(
                                new FactorySet<PolylineData>(
                                        PolylineData.FactoryInstance
                                    ),
                                new ViewerSet<PolylineData>()
                            )
                    );

            file = new EditorFile(graph, factoryset);

            obj1 = graph.CreateObject();
            obj2 = graph.CreateObject();
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
            var polyline = new PolylineData(obj1.ID, file);
            polyline.Bind();

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

            polyline.Unbind();
        }

        [TestMethod]
        public void TestPolyline_2() {
            var polyline = new PolylineData(obj2.ID, file);
            polyline.Bind();

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

            polyline.Unbind();
        }

        [TestMethod]
        public void TestCodeRendering() {
            var func =
                new GlobalPerspectiveFunction(new SquareFunction());

            var rendered =
                new StringBuilder();

            code.Render(rendered, func, null);

            Assert.AreEqual(rendered.ToString(), "square time.global");
        }
    }
}
