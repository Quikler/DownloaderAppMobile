using System;
using System.IO;
using System.Linq;

namespace DownloaderAppMobile.Helpers
{
    public static class PathHelper
    {
        public static void CopyEmbeddedResourceToFile(string resourceName, string destinationPath)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                    throw new FileNotFoundException("Resource not found", resourceName);

                using (var fileStream = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }
        }

        public static string CreateValidFileName(string str)
        {
            char[] invalidChars = new[] { '\\', ':', '*', '?', '\"', '<', '>', '|', '\0', '/' };
            var output = new System.Text.StringBuilder();
            bool lastCharWasSpace = false;

            foreach (char c in str)
            {
                if (invalidChars.Contains(c))
                    continue;

                if (char.IsWhiteSpace(c))
                {
                    if (lastCharWasSpace)
                        continue;

                    lastCharWasSpace = true;
                }
                else
                {
                    lastCharWasSpace = false;
                }

                output.Append(c);
            }

            return output.ToString().Trim();
        }

        public static string CreateValidFilePath(string folderPath, string fileName, string extension)
            => Path.Combine(folderPath, CreateValidFileName(fileName)) + GetExtensionWithPeriod(extension);

        public static string CreateValidFilePath(string folderPath, string fileNameWithExtension)
            => Path.Combine(folderPath, CreateValidFileName(fileNameWithExtension));

        public static string ChangeDirectory(string path, string newDirectory)
            => Path.Combine(newDirectory, Path.GetFileName(path));

        private static string GetExtensionWithPeriod(string extension)
            => extension.Contains('.') ? extension : $".{extension}";
    }
}
