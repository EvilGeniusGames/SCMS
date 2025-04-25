using System.IO;

namespace SCMS.Classes
{
    public static class ThemeAssetManager
    {
        public static void EnsureThemeAssets()
        {
            var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "Themes", "default");
            var targetPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Themes", "default");

            var assetFolders = new[] { "css", "js", "images", "fonts" };

            foreach (var folder in assetFolders)
            {
                var srcDir = Path.Combine(sourcePath, folder);
                var tgtDir = Path.Combine(targetPath, folder);

                if (!Directory.Exists(tgtDir))
                    Directory.CreateDirectory(tgtDir);

                if (Directory.Exists(srcDir))
                {
                    foreach (var file in Directory.GetFiles(srcDir))
                    {
                        var fileName = Path.GetFileName(file);
                        var destFile = Path.Combine(tgtDir, fileName);

                        if (!File.Exists(destFile))
                        {
                            File.Copy(file, destFile);
                        }
                    }
                }
            }
        }
    }
}
