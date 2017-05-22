using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel
{
    public class CustomErrorInfo
    {
        public string FileName { get; set; }
        public int StartLine { get; set; }
        public int StartColumn { get; set; }
        public int EndLine { get; set; }
        public int EndColumn { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; }
        public string Subcategory { get; set; }
        public int CellNumber { get; set; }
    }

    public class PreprocessResult
    {
        public string[] OriginalLines { get; set; }
        public string[] HelpLines { get; set; }
        public string[] CsiOutputLines { get; set; }
        public string[] NuGetLines { get; set; }
        public string[] FilteredLines { get; set; }
        public CustomErrorInfo[] Errors { get; set; }
    }
    public class NuGetManager
    {
        private object syncObject;
        private object packagesDir;

        public NuGetManager(string executingDirectory)
        {
            syncObject = new object();
            packagesDir = Path.Combine(executingDirectory);     
        }

        internal PreprocessResult Preprocess(string code)
        {
            var lines = code.Split('\n');

            return new PreprocessResult()
            {
                OriginalLines = lines,
                HelpLines = new string[] { },
                NuGetLines = new string[] { },
                CsiOutputLines = new string[] { },
                FilteredLines = lines
            };

            //// split the source code into lines, then get the nuget lines
            //let lines = source.Split('\n')
            //let linesSplit = DirectivePreprocessor.partitionLines lines

            //let orEmpty key = let opt = Map.tryFind key linesSplit
            //                if opt.IsSome then opt.Value else Seq.empty

            //let helpLines = DirectivePreprocessor.Line.HelpDirective |> orEmpty
            //let fsiOutputLines = DirectivePreprocessor.Line.FSIOutputDirective |> orEmpty
            //let nugetLines = DirectivePreprocessor.Line.NugetDirective |> orEmpty
            //let otherLines = DirectivePreprocessor.Line.Other |> orEmpty

            ////NuGet broke, we've replaced it with Paket: https://github.com/fsprojects/IfSharp/issues/106

            //{
            //    OriginalLines = lines;
            //    HelpLines = helpLines |> Seq.map(fun(_, line)->line) |> Seq.toArray;
            //    FsiOutputLines = fsiOutputLines |> Seq.map(fun(_, line)->line) |> Seq.toArray;
            //    NuGetLines = nugetLines |> Seq.map(fun(_, line)->line) |> Seq.toArray;
            //    FilteredLines = otherLines |> Seq.map(fun(_, line)->line) |> Seq.toArray;
            //    Errors = Array.empty //errors;
            //}
        }
    }
}
