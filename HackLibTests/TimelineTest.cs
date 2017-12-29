using System;
using System.Collections.Generic;
using System.Text;
using HackLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HackLibTests
{
    [TestClass]
    public class TimelineTest
    {
        [TestMethod]
        public void TestOrder()
        {
            var timeline = new Timeline<object>();

            var o1 = new object();
            var o2 = new object();
            var o3 = new object();

            timeline.Enqueue(100, o1);
            timeline.Enqueue(120, o2);
            timeline.Enqueue(100, o3);

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

            for (int l = 0; l < ls.Count; l++)
            {
                timeline.Enqueue(l, ls[l]);
            }

            for (int i = 1; i < 10; i++)
            {
                for (int l = 0; l < ls.Count; l++)
                {
                    var e = timeline.Dequeue();
                    Assert.AreEqual(ls[l], e);

                    timeline.Enqueue(l + i * 100, ls[l]);

                }
            }
        }


        [TestMethod]
        public void TestLoopSame()
        {
            var timeline = new Timeline<object>();
            var ls = new List<object> { new StringBuilder("O1"), new StringBuilder("O2"), new StringBuilder("O3") };

            for (int l = 0; l < ls.Count; l++)
            {
                timeline.Enqueue(0, ls[l]);
            }

            for (int i = 1; i < 10; i++)
            {
                for (int l = 0; l < ls.Count; l++)
                {
                    var e = timeline.Dequeue();
                    Assert.AreEqual(ls[l], e);

                    timeline.Enqueue(i * 100, ls[l]);

                }
            }
        }
    }
}
