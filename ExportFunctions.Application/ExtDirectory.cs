namespace ExportFunctions.Application
{
    public static class ExtDirectory
    {
        public static string CreateDirectory( this string path )
        {

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Console.WriteLine($"Creade directory: {path}");
                
            }
            return path;
        }
    }
}
