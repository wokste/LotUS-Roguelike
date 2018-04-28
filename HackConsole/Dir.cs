namespace HackConsole
{
    public enum Dir
    {
        Left, Up, Right, Down
    }

    public static class DirExtentions
    {
        public static bool IsHorizontal(this Dir d)
        {
            return (d == Dir.Left || d == Dir.Right);
        }

        public static bool IsVertical(this Dir d)
        {
            return (d == Dir.Up || d == Dir.Down);
        }

        public static Dir Flip(this Dir d)
        {
            return (Dir)((int)d ^ 2);
        }
    }
}