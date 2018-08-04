using System.Collections.Generic;

namespace HackConsole
{
    public class Grid<T>
    {
        private readonly T[,] _grid;
        public readonly Size Size;

        public Grid(Size size, T defaultValue = default(T))
        {
            Size = size;
            _grid = new T[size.X, size.Y];

            for (var y = 0; y < Size.Y; ++y)
                for (var x = 0; x < Size.X; ++x)
                    _grid[x, y] = defaultValue;
        }

        public T this[Vec v] {
            get => _grid[v.X, v.Y];
            set => _grid[v.X, v.Y] = value;
        }

        public T this[int x, int y] {
            get => _grid[x, y];
            set => _grid[x, y] = value;
        }

        public IEnumerable<Vec> Ids()
        {
            for (var y = 0; y < Size.Y; ++y)
                for (var x = 0; x < Size.X; ++x)
                    yield return new Vec(x, y);
        }
    }
}
