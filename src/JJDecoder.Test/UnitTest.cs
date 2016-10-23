using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace JJDecoder.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void SimpleDecodeTest()
        {
            var encoded = File.ReadAllText(@"TestData\test1.txt");
            var result = JJDecorer.JJDecoder.Decode(encoded);
            Assert.AreEqual("alert(\"Hello, JavaScript\" )", result);
        }

        [TestMethod]
        public void PalindromeDecodeTest()
        {
            var encoded = File.ReadAllText(@"TestData\test2.txt");
            var result = JJDecorer.JJDecoder.Decode(encoded);
            Assert.AreEqual(@"alert(""Hello, JavaScript"" )", result);
        }

        [TestMethod]
        public void GlobalVarDecodeTest()
        {
            var encoded = File.ReadAllText(@"TestData\test3.txt");
            var result = JJDecorer.JJDecoder.Decode(encoded);
            Assert.AreEqual(@"alert(""Hello, JavaScript"" )", result);
        }
    }
}
