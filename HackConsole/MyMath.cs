namespace HackConsole
{
    public static class MyMath
    {
        public static int Clamp(int val, int min, int max)
        {
            return val < min ? min : val > max ? max : val;
        }
    }
}
