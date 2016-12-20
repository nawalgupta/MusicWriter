using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MusicWriter.Tests
{
    [TestClass]
    public class FunctionTests
    {
        [TestMethod]
        public void TestPolyline_1() {
            var polyline = new PolylineFunction();

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
    }
}
