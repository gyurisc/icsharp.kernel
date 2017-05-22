using System;
using System.IO;

namespace ICSharp.Kernel
{
    public class Config
    {        
        public static PlatformID ActualPlatform { get; private set; }

        public static string UserDir { get; private set; }
        public static string ThisExecutable { get; private set; }
        public static string AppData { get; private set; }
        public static string JupyterDir { get; private set; }
        public static string KernelsDir { get; private set; }
        public static string KernelDir { get; private set; }
        public static string StaticDir { get; private set; }
        public static string TempDir { get; private set; }
        public static string KernelVersion { get; private set; }

        // http://jupyter-client.readthedocs.io/en/latest/kernels.html#kernel-specs
        static Config()
        {
            ThisExecutable = System.Reflection.Assembly.GetEntryAssembly().Location;
            UserDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            AppData = GetAppData();
            ActualPlatform = GetActualPlatform();
            JupyterDir = GetJupyterDir();
            KernelsDir = Path.Combine(JupyterDir, "kernels");
            KernelDir = Path.Combine(KernelsDir, "icsharpkernel");
            StaticDir = Path.Combine(KernelDir, "static");
            TempDir = Path.Combine(StaticDir, "temp");            
            KernelVersion = "1";
        }
     
        private static string GetJupyterDir()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            { 
                return Path.Combine(AppData, "jupyter");
            } else
            {
                return Path.Combine(AppData, "Jupyter");
            }
        }

        private static string GetAppData()
        {
            switch (ActualPlatform)
            {
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                    return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    break;
                case PlatformID.MacOSX:
                    return Path.Combine(UserDir, "Library");
                    break;
                case PlatformID.Unix:
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    break;

                default:
                    throw new Exception($"Unknown platform: {ActualPlatform}");
                    break;
            }
        }

        private static PlatformID GetActualPlatform()            
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                if (Directory.Exists("/Applications")
                    && Directory.Exists("/System")
                    && Directory.Exists("/Users")
                    && Directory.Exists("/Volumes"))
                {
                    return PlatformID.MacOSX;
                }
                else
                {
                    return PlatformID.Unix;
                }
            }

            return Environment.OSVersion.Platform; ;
        }
        
    }
}
