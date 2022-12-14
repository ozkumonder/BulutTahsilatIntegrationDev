using System;
using System.IO;

namespace BulutTahsilatIntegration.WinService.Core
{
  public  class FileHelper
    {
        public static string MovePath(string filePath, string startingFolder, string newFolder)
        {
            if (Directory.Exists(filePath))
            {
                if (!Directory.Exists(startingFolder))
                {
                    Directory.CreateDirectory(ConfigHelper.WebConfigRead("FilePath"));
                }
            }
            File.Move(startingFolder,newFolder);

            return "";
        }

        public static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }
    }
}
