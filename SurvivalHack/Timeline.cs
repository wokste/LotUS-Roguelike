using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SurvivalHack
{
    public class Timeline<T>
    {
        private SortedDictionary<long,List<T>> _queue = new SortedDictionary<long, List<T>>();
        private int _index;
        private long _time = 0;//long.MinValue;
        private List<T> _list;

        public long Time => _time;

        public void AddRelative(T toInsert, long time)
        {
            Add(toInsert, _time + time);
        }

        public void Add(T toInsert, long time)
        {
            // If this is not the case, non-obvious bugs may arise.
            Debug.Assert(time > _time);

            List<T> list;
            if (!_queue.TryGetValue(time, out list))
            {
                list = new List<T>();
                _queue.Add(time, list);
            }

            list.Add(toInsert);
        }

        public T Dequeue()
        {
            var ret = Peek();

            _list[_index] = default(T); // Helps the GC.
            _index++;
            if (_index >= _list.Count)
            {
                _list = null;
            }
            return ret;
        }

        public T Peek()
        {
            if (_list == null)
            {
                var pair = _queue.First();
                _queue.Remove(pair.Key);
                _index = 0;
                _time = pair.Key;
                _list = pair.Value;
            }
            return _list[_index];
        }
    }
}
