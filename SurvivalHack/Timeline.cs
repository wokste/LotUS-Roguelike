using System.Collections.Generic;

namespace SurvivalHack
{
    public interface IEvent {
        void Run();

        int RepeatTurns { get; }
    }

    public class Timeline : IEvent
    {
        private readonly SortedDictionary<long,List<IEvent>> _sporadicListQueue = new SortedDictionary<long, List<IEvent>>();
        private long _time = 0;//long.MinValue;
        //private List<IEvent> _repeatList;

        //public long Time => _time;
        public int RepeatTurns => 1;

        public void Insert(IEvent evt)
        {
            if (evt.RepeatTurns <= 0)
                return;

            var absoluteTime = _time + evt.RepeatTurns;

            if (!_sporadicListQueue.TryGetValue(absoluteTime, out var list))
            {
                list = new List<IEvent>();
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
