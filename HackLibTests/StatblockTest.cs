using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurvivalHack.Combat;

namespace HackLibTests
{
    [TestClass]
    public class StatblockTest
    {
        [TestMethod]
        public void BarValues()
        {
            var sb = new StatBlock(10,0,0);

            Assert.AreEqual(sb.Cur(0), 10);
            Assert.AreEqual(sb.Max(0), 10);
            Assert.AreEqual(sb.Perc(0), 1f);
            /*
            sb.TakeDamage( -= 5;
            Assert.AreEqual(sb.Cur(0), 5);
            Assert.AreEqual(sb.Max, 10);
            Assert.AreEqual(sb.Perc, 0.5f);

            b.Current += 15;
            Assert.AreEqual(sb.Current, 10);
            Assert.AreEqual(sb.Max, 10);
            Assert.AreEqual(sb.Perc, 1f);

            b.Max = 15;
            Assert.AreEqual(sb.Current, 10);
            Assert.AreEqual(sb.Max, 15);

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
            */
        }
    }
}
