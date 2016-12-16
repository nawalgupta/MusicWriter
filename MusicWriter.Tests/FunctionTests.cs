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

            polyline.Add(Time.Zero, 0);
            polyline.Add(Time.Note, 2);

            Assert.AreEqual(polyline.GetValue(new FunctionCall(Time.Note)), 2f);
            Assert.AreEqual(polyline.GetValue(new FunctionCall(Time.Note_2nd)), 1f);
            Assert.AreEqual(polyline.GetValue(new FunctionCall(Time.Zero)), 0f);

            polyline.Add(Time.Note_2nd, 1.5f);
            
            Assert.AreEqual(polyline.GetValue(new FunctionCall(Time.Note)), 2f);
            Assert.AreEqual(polyline.GetValue(new FunctionCall(Time.Note_2nd + Time.Note_4th)), 1.75f);
            Assert.AreEqual(polyline.GetValue(new FunctionCall(Time.Note_2nd)), 1.5f);
            Assert.AreEqual(polyline.GetValue(new FunctionCall(Time.Note_4th)), 0.75f);
            Assert.AreEqual(polyline.GetValue(new FunctionCall(Time.Zero)), 0f);

            polyline.Add(Time.Note + Time.Note_4th, 1);

            Assert.AreEqual(polyline.GetValue(new FunctionCall(Time.Note + Time.Note_16th)), 1.75f);
        }
    }
}
