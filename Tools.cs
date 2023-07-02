using System.IO;

namespace Mododger
{
    public class Tools
    {
        /// <summary>
        /// Get any file in the "assets" path.
        /// </summary>
        public static string AssetPath(string path)
        {
            var assets = new FileInfo(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath).Directory.FullName + @"\Assets\";
            if (!Directory.Exists(assets))
                Directory.CreateDirectory(assets);
            var re = assets + path;
            return re;
        }
    }
}
