using System;

namespace HackConsole
{
    public struct Transform
    {
        public Vec Offset;
        public int ClockwiseRotates;

        public Vec Convert(Vec input)
        {
            return Rotate(input) + Offset;
        }

        public Dir Convert(Dir input)
        {
            return (Dir)(((int)input + ClockwiseRotates) % 4);
        }

        private Vec Rotate(Vec input)
        {
            switch (ClockwiseRotates)
            {
                case 0:
                    return input;
                case 1:
                    return new Vec(input.Y, -input.X);
                case 2:
                    return new Vec(-input.X, -input.Y);
                case 3:
                    return new Vec(-input.Y, input.X);
                default:
                    throw new ArithmeticException($"ClockwiseRotates should be in the range[0,3]. It is {ClockwiseRotates}");
            }
        }
    }
}
