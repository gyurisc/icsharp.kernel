using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharp.Kernel
{
    public class App
    {
        private static string fileName;
        private static string json;
        private static object connectionInformation;
        private static string userDir;

        public static string Black { get; private set; } = "\u001B[0;30m";
        public static string Blue { get; private set; }  =  "\u001B[0;34m";
        public static string Green { get; private set; }  =  "\u001B[0;32m";
        public static string Cyan { get; private set; }  =  "\u001B[0;36m";
        public static string Red { get; private set; }  =  "\u001B[0;31m";
        public static string Purple { get; private set; }  =  "\u001B[0;35m";
        public static string Brown { get; private set; }  =  "\u001B[0;33m";
        public static string Gray { get; private set; }  =  "\u001B[0;37m";
        public static string DarkGray { get; private set; }  =  "\u001B[1;30m";
        public static string LightBlue { get; private set; }  =  "\u001B[1;34m";
        public static string LightGreen { get; private set; }  =  "\u001B[1;32m";
        public static string LightCyan { get; private set; }  =  "\u001B[1;36m";
        public static string LightRed { get; private set; }  =  "\u001B[1;31m";
        public static string LightPurple { get; private set; }  =  "\u001B[1;35m";
        public static string Yellow { get; private set; }  =  "\u001B[1;33m";
        public static string White { get; private set; }  =  "\u001B[1;37m";
        public static string Reset { get; private set; } = "\u001B[0m";


        public static Kernel kernel { get; private set; }
        public static void Start(string[] args)
        {
            if (args.Length == 0)
            {
                Install(true);
                StartJupyter();
            }
            else if (args[0] == "--install")
            {
                Install(true);
            }
            else
            {
                // Verify the kernel installation status 
                Install(false);

                // Clear temporary folder 
                try
                {
                    if (Directory.Exists(Config.TempDir))
                    {
                        Directory.Delete(Config.TempDir, true);
                    }

                    // adds the default display printers
                    Printers.addDefaultDisplayPrinters();

                    // get connection information
                    fileName = args[0];
                    json = File.ReadAllText(fileName);
                    var connectionInformation = JsonConvert.DeserializeObject<ConnectionInformation>(json);

                    System.Diagnostics.Debugger.Launch();

                    // start the kernel
                    kernel = new Kernel(connectionInformation);
                    kernel.StartAsync();

                    // block forever
                    Thread.Sleep(Timeout.Infinite);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private static void StartJupyter()
        {
            userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            Console.WriteLine("Starting Jupyter...");

            Process p = new Process();
            p.StartInfo.FileName = "jupyter";
            p.StartInfo.Arguments = "notebook";
            p.StartInfo.WorkingDirectory = userDir;

            if (!p.Start()) {
                throw new Exception("Unable to start Jupyter, please install Jupyter first");
            }
        }

        public static void Help(object value)
        {
            var text = new StringBuilder();

            // adding to the payload
            kernel.AddPayload(text.ToString());
        }
        private static void Install(bool forceInstall)
        {
            var thisExecutable = Assembly.GetEntryAssembly().Location;
            var kernelDir = Config.KernelDir;
            var staticDir = Config.StaticDir;
            var tempDir = Config.TempDir;
            var customDir = Path.Combine(staticDir, "custom");

            CreateDir(kernelDir);
            CreateDir(staticDir);
            CreateDir(tempDir);
            CreateDir(customDir);

            var allFiles = new List<String>();

            var configFile = Path.Combine(kernelDir, "ipython_config.py");
            allFiles.Add(configFile);

            //var configqtFile = Path.Combine(kernelDir, "ipython_qtconsole_config.py");
            //allFiles.Add(configqtFile);

            var kernelFile = Path.Combine(kernelDir, "kernel.json");
            allFiles.Add(kernelFile);

            var logoFile = Path.Combine(customDir, "icsharp_logo.png");
            allFiles.Add(logoFile);

            var kjsFile = Path.Combine(kernelDir, "kernel.js");
            allFiles.Add(kjsFile);

            var cjsFile = Path.Combine(customDir, "csharp.js");
            allFiles.Add(cjsFile);

            var wjsFile = Path.Combine(customDir, "webintellisense.js");
            allFiles.Add(wjsFile);

            var wcjsFile = Path.Combine(customDir, "webintellisense-codemirror.js");
            allFiles.Add(wcjsFile);

            var logo64File = Path.Combine(kernelDir, "logo-64x64.png");
            allFiles.Add(logo64File);

            var logo32File = Path.Combine(kernelDir, "logo-32x32.png");
            allFiles.Add(logo32File);

            var versionFile = Path.Combine(kernelDir, "version.txt");
            allFiles.Add(versionFile);

            var missingFiles = allFiles.Where(fn => File.Exists(fn)==false).ToList();
            var differentVersion = File.Exists(versionFile) && File.ReadAllText(versionFile) != Config.KernelVersion;

            if (forceInstall) {
                Console.WriteLine("Force install required, performing install...");
            }
            else if (missingFiles.Count > 0) {
                Console.WriteLine("One or more files are missing, performing install...");
            } else if (differentVersion) {
                Console.WriteLine("Different version found, performing install...");
            }

            if (forceInstall || missingFiles.Count > 0 || differentVersion)
            {
                // write the version file 
                File.WriteAllText(versionFile, Config.KernelVersion);

                // write the startup script 
                var codeTemeplate = CSharpResources.IPython_Config;
                var env = Environment.OSVersion.Platform;
                var code = codeTemeplate; 

                if (env == PlatformID.Win32Windows || env == PlatformID.Win32NT || env == PlatformID.Win32S)
                {
                    code = codeTemeplate.Replace("\"mono\",","");
                }

                code = code.Replace("%kexe", thisExecutable);
                code = code.Replace("%kstatic", staticDir);

                Console.WriteLine($"Saving custom config file {configFile}");
                File.WriteAllText(configFile, code);

                // Skipping writing qt config file. 

                // write custom logo file 
                Console.WriteLine($"Saving custom logo {logoFile}");
                File.WriteAllBytes(logoFile, CSharpResources.ICSharp_Logo);

                // write csharp css file 
                var cssFile = Path.Combine(customDir, "csharp.css");
                Console.WriteLine($"Saving csharp css {cssFile}");
                File.WriteAllText(cssFile, CSharpResources.CSharp_Css);

                // write kernel js file 
                Console.WriteLine($"Saving jernel js {kjsFile}");
                File.WriteAllText(kjsFile, CSharpResources.Kernel_Js);

                // write csharp js file 
                Console.WriteLine($"Saving csharp js {cjsFile}");
                File.WriteAllText(cjsFile, CSharpResources.CSharp_Js);

                // write webintellisense js file 
                Console.WriteLine($"Saving webintellisense js {wjsFile}");
                File.WriteAllText(wjsFile, CSharpResources.WebIntellisense_Js);

                // write webintellisens-codemore js file 
                Console.WriteLine($"Saving webintellisense-codemirror js {wcjsFile}");
                File.WriteAllText(wcjsFile, CSharpResources.WebIntellisenseCodeMirror_Js);

                // Kernel info folder 
                var jsonTemplate = CSharpResources.ICSharpKernel_Json;
                code = jsonTemplate;

                if (env == PlatformID.Win32Windows || env == PlatformID.Win32NT || env == PlatformID.Win32S)
                {
                    code = jsonTemplate.Replace("\"mono\",", "");
                }

                code = code.Replace("%s", thisExecutable.Replace("\\", "/"));
                Console.WriteLine($"Saving custom kernel.json file {kernelFile}");
                File.WriteAllText(kernelFile, code);

                Console.WriteLine($"Saving kernel icon {logo64File}");
                File.WriteAllBytes(logo64File, CSharpResources.ICSharp_64logo);

                Console.WriteLine($"Saving kernel icon {logo32File}");
                File.WriteAllBytes(logo32File, CSharpResources.ICSharp_32logo);

                // TODO: Installing Dependencies for kernel
                Console.WriteLine("Installing Dependencies is not yet implemented!");
            }
        }

        private static void CreateDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
