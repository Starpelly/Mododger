using System.IO;

namespace Mododger
{
    internal class Tools
    {
        /// <summary>
        /// Get any file in the "assets" path.
        /// </summary>
        public static string AssetPath(string path)
        {
            var re = new FileInfo(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath).Directory.FullName + @"\Assets\" + path;
            return re;
        }
    }
}
