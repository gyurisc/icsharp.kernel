using ICSharp.Kernel;
using System;
using System.Globalization;
using System.IO;
using Trinet.Core.IO.Ntfs;
using XPlot.Plotly;
using static XPlot.Plotly.Graph;

namespace ICSharp.Console
{
    class Program
    {
        private static void ClearAlternativeStreamsWindows()
        {
            var path = System.Reflection.Assembly.GetEntryAssembly().Location;
            if (path != null)
            {
                foreach (var filePath in new FileInfo(path).Directory.GetFileSystemInfos())
                {
                    filePath.DeleteAlternateDataStream("Zone.Identifier");
                }
            }
        }

        static void Main(string[] args)
        {
            PltlyTest();
            //On Windows if you download our zip releases you files will be marked as from the Internet
            //Depending on how you extract the files this marker may be left, this will break Paket, so clear it.
            if (Environment.OSVersion.Platform != PlatformID.Unix && Environment.OSVersion.Platform != PlatformID.MacOSX)
            {
                ClearAlternativeStreamsWindows();
            }
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            App.Start(args);
        }

        public static void PltlyTest()
        {
            Heatmap h = new Heatmap();
            h.z = new int[,] { { 1, 20, 30 }, { 20, 1, 60 }, { 30, 60, 1 } };
            var chart = Chart.Plot(h);
            chart.WithLayout(new Layout.Layout() { title = "Heatmap example!" });
            chart.WithHeight(500);
            chart.WithWidth(700);

            string html = chart.GetInlineHtml();
        }
    }
}
