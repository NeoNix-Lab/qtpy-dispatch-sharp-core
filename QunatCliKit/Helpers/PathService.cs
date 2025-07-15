namespace QunatCliKit.Helpers
{
    public static class PathService
    {
        public static bool ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            return IsValidPath(path);
        }


        public static void Exist(string Path)
        {
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
        }

        public static void Override(string Path)
        {
            if (Directory.Exists(Path))
            {
                Environment.SetEnvironmentVariable("QT_SDK_PATH", Path, EnvironmentVariableTarget.User);
            }
            else 
                throw new DirectoryNotFoundException($"The directory '{Path}' does not exist.");
        }
        

        private static bool IsValidPath(string path)
        {
            // 1) Caratteri non validi
            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return false;

            // 2) Deve essere rooted e fully qualified
            if (!Path.IsPathRooted(path) || !Path.IsPathFullyQualified(path))
                return false;

            // 3) Normalizzazione
            try
            {
                string normalized = Path.GetFullPath(path);
                // Opzionale: controlla lunghezza, esistenza, ecc.
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
