using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurvivalHack;

namespace HackLibTests
{
    [TestClass]
    public class BarTest
    {
        [TestMethod]
        public void BarValues()
        {
            var b = new Bar(10);

            Assert.AreEqual(b.Current, 10);
            Assert.AreEqual(b.Max, 10);
            Assert.AreEqual(b.Perc, 1f);

            b.Current -= 5;
            Assert.AreEqual(b.Current, 5);
            Assert.AreEqual(b.Max, 10);
            Assert.AreEqual(b.Perc, 0.5f);

            b.Current += 15;
            Assert.AreEqual(b.Current, 10);
            Assert.AreEqual(b.Max, 10);
            Assert.AreEqual(b.Perc, 1f);

            b.Max = 15;
            Assert.AreEqual(b.Current, 10);
            Assert.AreEqual(b.Max, 15);

            b.Max = 5;
            Assert.AreEqual(b.Current, 5);
            Assert.AreEqual(b.Max, 5);
            Assert.AreEqual(b.Perc, 1f);

            b.Max = 10;
            Assert.AreEqual(b.Current, 5);
            Assert.AreEqual(b.Max, 10);
            Assert.AreEqual(b.Perc, 0.5f);
            
            b.Current -= 20;
            Assert.AreEqual(b.Current, 0);
            Assert.AreEqual(b.Max, 10);
            Assert.AreEqual(b.Perc, 0f);
        }
    }
}
