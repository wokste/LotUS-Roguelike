using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HackLib
{
    public class Timeline<T>
    {
        private SortedDictionary<long,List<T>> _queue = new SortedDictionary<long, List<T>>();
        private int _index;
        //private long _minTime = 0;
        private List<T> _list;
        
        public void Enqueue(long time, T toInsert)
        {
            // If this is not the case, non-obvious bugs may arise.
            //Debug.Assert(time > _minTime);

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
            if (_list == null)
            {
                var pair = _queue.First();
                _queue.Remove(pair.Key);
                _index = 0;
                //_minTime = pair.Key;
                _list = pair.Value;
            }

            var ret = _list[_index];
            _list[_index] = default(T); // Helps the GC.
            _index++;
            if (_index >= _list.Count)
            {
                _list = null;
            }
            return ret;
        }
    }
}
