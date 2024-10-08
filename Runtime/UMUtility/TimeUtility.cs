namespace UM.Runtime.UMUtility
{
    public static class TimeUtility
    {
        public static int ToMilliseconds(this float seconds) => (int) (seconds * 1000);
    }
}