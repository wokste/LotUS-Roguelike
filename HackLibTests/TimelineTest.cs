using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurvivalHack;

namespace HackLibTests
{
    [TestClass]
    public class TimelineTest
    {
        [TestMethod]
        public void TestOrder()
        {
            var timeline = new Timeline<object>();

            var o1 = new StringBuilder("o1");
            var o2 = new StringBuilder("o2");
            var o3 = new StringBuilder("o3");

            timeline.Add(o1, 100);
            timeline.Add(o2, 120);
            timeline.Add(o3, 100);

            Assert.AreEqual(o1, timeline.Dequeue());
            Assert.AreEqual(o3, timeline.Dequeue());
            Assert.AreEqual(o2, timeline.Dequeue());
            try
            {
                timeline.Dequeue();
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }

        }

        [TestMethod]
        public void TestLoopDiff()
        {
            var timeline = new Timeline<object>();
            var ls = new List<object>{new StringBuilder("O1"), new StringBuilder("O2"), new StringBuilder("O3") };

            for (var l = 0; l < ls.Count; l++)
            {
                timeline.Add(ls[l], l + 10);
            }

            for (var i = 1; i < 10; i++)
            {
                for (var l = 0; l < ls.Count; l++)
                {
                    var e = timeline.Dequeue();
                    Assert.AreEqual(ls[l], e);

                    timeline.Add(ls[l], l + i * 100 + 10);

                }
            }
        }


        [TestMethod]
        public void TestLoopSame()
        {
            var timeline = new Timeline<object>();
            var ls = new List<object> { new StringBuilder("O1"), new StringBuilder("O2"), new StringBuilder("O3") };

            foreach (var t in ls)
            {
                timeline.Add(t, 10);
            }

            for (var i = 1; i < 10; i++)
            {
                foreach (var t in ls)
                {
                    var e = timeline.Dequeue();
                    Assert.AreEqual(t, e);

                    timeline.Add(t, i * 100 + 10);
                }
            }
        }
    }
}
