using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ICSharp.Kernel
{
    public class CSharpResources
    {
        private static string basePath;

        public static byte[] ICSharp_Logo { get; private set; }
        public static string ICSharpKernel_Json { get; private set; }
        public static string IPython_Config { get; private set; }
        public static string CSharp_Css { get; private set; }
        public static string Kernel_Js { get; private set; }
        public static byte[] ICSharp_64logo { get; private set; }
        public static byte[] ICSharp_32logo { get; private set; }
        public static string WebIntellisense_Js { get; private set; }
        public static string WebIntellisenseCodeMirror_Js { get; private set; }
        public static string CSharp_Js { get; private set; }

        static CSharpResources()
        {
            var location = Assembly.GetEntryAssembly().Location;
            basePath = Path.GetDirectoryName(location);

            ICSharp_Logo = GetBytes("ipython-profile\\icsharp_logo.png");
            CSharp_Css = GetString("ipython-profile\\static\\custom\\csharp.css");
            Kernel_Js = GetString("ipython-profile\\kernel.js");
            CSharp_Js = GetString("ipython-profile\\static\\custom\\csharp.js");
            WebIntellisense_Js = GetString("ipython-profile\\static\\custom\\webintellisense.js");
            WebIntellisenseCodeMirror_Js = GetString("ipython-profile\\static\\custom\\webintellisense-codemirror.js");
            IPython_Config = GetString("ipython-profile\\ipython_config.py");
            ICSharpKernel_Json = GetString("kernel-spec\\kernel.json");
            ICSharp_64logo = GetBytes("kernel-spec\\logo-64x64.png");
            ICSharp_32logo = GetBytes("kernel-spec\\logo-32x32.png");
        }

        private static string UpdatePathForPlatform(string path)
        {
            var paths = new List<string>();

            paths.Add(basePath);
            paths.AddRange(path.Split('\\'));

            return Path.Combine(paths.ToArray());
        }

        private static string GetString(string path)
        {
            var fullPath = UpdatePathForPlatform(path);
            var content = File.ReadAllText(fullPath);
            return content;
        }

        private static byte[] GetBytes(string path)
        {
            var fullPath = UpdatePathForPlatform(path);
            return File.ReadAllBytes(fullPath);
        }
    }
}
