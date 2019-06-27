﻿using System.Collections.Generic;

namespace SurvivalHack
{
    public interface ITimeEvent {
        void Run();

        int TicksPerTurn { get; }
    }

    public class Timeline : ITimeEvent
    {
        private readonly SortedDictionary<long,List<ITimeEvent>> _sporadicListQueue = new SortedDictionary<long, List<ITimeEvent>>();
        private long _time = 0;

        public int TicksPerTurn => 1;

        public void Insert(ITimeEvent evt)
        {
            if (evt.TicksPerTurn <= 0)
                return;

            var absoluteTime = _time + evt.TicksPerTurn;

            if (!_sporadicListQueue.TryGetValue(absoluteTime, out var list))
            {
                list = new List<ITimeEvent>();
                _sporadicListQueue.Add(absoluteTime, list);
            }

            list.Add(evt);
        }

        public void Run()
        {
            // Run repeat list
            if (_sporadicListQueue.TryGetValue(++_time, out var sporadicList))
            {
                foreach (var e in sporadicList)
                    e.Run();

                foreach (var e in sporadicList)
                    Insert(e);
            }
        }
    }
}
