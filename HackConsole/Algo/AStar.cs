﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackConsole.Algo
{
    public class AStar<T>
    {
        readonly Dictionary<Vec, GridNode> _openList = new Dictionary<Vec, GridNode>();
        readonly Dictionary<Vec, GridNode> _closedList = new Dictionary<Vec, GridNode>();

        readonly Func<Vec, float> _distFunc;
        readonly List<Vec> _neighbors;

        readonly Grid<T> _grid;
        readonly Func<T, float> _costFunc;

        /// <summary>
        /// Creates an AStar grid.
        /// </summary>
        /// <param name="grid">The map</param>
        /// <param name="costFunc">Can determine the cost of a grid square. The lambda should return values in [1,inf], where 1 and inf are the most ideal for fast execution.</param>
        /// <param name="acceptDiagonals">true if you should be able to move diagonally. False otherwise. Diagonals cost x1.4 if allowed. </param>
        public AStar(Grid<T> grid, Func<T, float> costFunc, bool acceptDiagonals) {
            _grid = grid;
            _costFunc = costFunc;

            if (acceptDiagonals)
            {
                _neighbors = new List<Vec>() { new Vec(-1, 0), new Vec(-1, 1), new Vec(0, 1), new Vec(1, 1), new Vec(1, 0), new Vec(1, -1), new Vec(0, -1), new Vec(-1, -1), };
                _distFunc = (Vec v) => (Math.Max(Math.Abs(v.X), Math.Abs(v.Y)) * 0.6f + Math.Abs(v.X) * 0.4f + Math.Abs(v.Y) * 0.4f);
            }
            else
            {
                _neighbors = new List<Vec>() { new Vec(-1, 0), new Vec(0, 1), new Vec(1, 0), new Vec(0, -1) };
                _distFunc = (Vec v) => (Math.Abs(v.X) + Math.Abs(v.Y));
            }
        }

        public void Clear()
        {
            _openList.Clear();
            _closedList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns>A path from source to dest, including sturce and dest</returns>
        public List<Vec> Run(Vec source, Vec dest)
        {
            _openList.Add(source, new GridNode {
                Parent = null, Travelled = 0, Dist = _distFunc(source - dest)
            });

            Vec max = _grid.Size;

            while (true)
            {
                if (_openList.Count == 0)
                {
                    Clear();
                    return null;
                }
                KeyValuePair<Vec, GridNode> current = PopOpen();
                _closedList.Add(current.Key, current.Value);

                if (current.Key == dest)
                    return TraversePath(dest);

                foreach (Vec v in _neighbors)
                {
                    Vec nbr = current.Key + v;
                    if (nbr.X < 0 || nbr.Y < 0 || nbr.X >= _grid.Size.X || nbr.Y >= _grid.Size.Y)
                        continue;

                    if (_closedList.ContainsKey(nbr))
                        continue;

                    var cost = _costFunc(_grid[nbr]);
                    if (float.IsInfinity(cost))
                        continue;

                    var newNode = new GridNode
                    {
                        Travelled = current.Value.Travelled + cost * _distFunc(v),
                        Dist = _distFunc(nbr - dest),
                        Parent = current.Key,
                    };

                    if (_openList.ContainsKey(nbr))
                    {
                        var testNode = _openList[nbr];
                        if (testNode.PathLength <= newNode.PathLength)
                            continue;
                    }
                    _openList[nbr] = newNode;
                }
            }
        }

        private List<Vec> TraversePath(Vec dest)
        {
            var reversePath = new List<Vec>();

            while (true)
            {
                reversePath.Add(dest);
                var destOrNull = _closedList[dest].Parent;
                if (destOrNull is Vec v)
                {
                    dest = v;
                    continue;
                }
                
                Clear();
                reversePath.Reverse();
                return reversePath;
            }
        }

        KeyValuePair<Vec, GridNode> PopOpen()
        {
            var min = _openList.ElementAt(0);

            foreach (var item in _openList)
            {
                if ((item.Value.PathLength < min.Value.PathLength) || (item.Value.PathLength == min.Value.PathLength && item.Value.Dist < min.Value.Dist))
                    min = item;
            }

            _openList.Remove(min.Key);
            return min;
        }


        public struct GridNode
        {
            public Vec? Parent;
            public float Travelled;
            public float Dist;
            public float PathLength => Travelled + Dist;
        }
    }
}
