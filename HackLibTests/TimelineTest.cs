using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurvivalHack;

namespace HackConsoleTests
{
    [TestClass]
    public class TimelineTest
    {
        private class MockEvent : ITimeEvent
        {
            private readonly string v;
            public int RepeatTurns { get; set; } = 1;
            public int Hits { get; private set; } = 0;
            public Action<MockEvent> OnHit;

            public MockEvent(string v)
            {
                this.v = v;
            }

            public override string ToString()
            {
                return $"{v} ({Hits})";
            }

            public void Run()
            {
                Hits++;
                OnHit?.Invoke(this);
            }
        }

        [TestMethod]
        public void TestLoop()
        {
            var timeline = new Timeline();
            var fixedEvents = new List<MockEvent>{new MockEvent("F0"), new MockEvent("F1"), new MockEvent("F2") };
            var evenEvent = new MockEvent("even") { RepeatTurns = 2 };
            var turn3Event = new MockEvent("turn3") { RepeatTurns = 3, OnHit = m => m.RepeatTurns = -1 };

            var sporadicEvents = new List<(MockEvent,int)> { (new MockEvent("S0-3"), 3), (new MockEvent("S0-7"),7), (new MockEvent("S0-5"),5) };

            // Change initial time point;
            for (int i = 0; i < 12; ++i)
                timeline.Run();

            // Add events
            foreach (var l in fixedEvents)
                timeline.Insert(l);
            timeline.Insert(evenEvent);
            timeline.Insert(turn3Event);

            // Check for event hits per type of event.
            for (var i = 0; i < 10; i++)
            {
                foreach (var l in fixedEvents)
                    Assert.AreEqual(i, l.Hits);

                Assert.AreEqual(i / 2, evenEvent.Hits);
                Assert.AreEqual(i < 3 ? 0 : 1, turn3Event.Hits);

                timeline.Run();
            }
        }
    }
}
