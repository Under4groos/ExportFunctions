namespace Expo
{
    public static class ExtString
    {
        public static string GetPath(this string[] str)
        {
            return str[1].Replace("\"", string.Empty);
        }
    }
}
