using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MusicWriter.Tests
{
    [TestClass]
    public class OtherTests
    {
        [TestMethod]
        public void TestBitArray() {
            var bitarray = new ShiftableBitArray();

            for (int i = 0; i < 256; i++) {
                var val = ((i / 32) % 2) == 1;

                bitarray.Insert(i, val);

                Assert.AreEqual(bitarray[i], val);
            }
            
            for (int i = 0; i < 512; i++) {
                var val =
                    i < 256 ?
                        ((i / 32) % 2) == 1 :
                        false;

                Assert.AreEqual(bitarray[i], val);
            }
        }
    }
}
