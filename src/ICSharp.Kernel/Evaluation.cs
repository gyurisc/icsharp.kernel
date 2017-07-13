using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
using Microsoft.CodeAnalysis;
using System.Threading;

namespace ICSharp.Kernel
{
    public class Evaluation
    {
        // THese properties are probably not needed anymore... 
        public static StringBuilder sbOut { get; private set; } = new StringBuilder();
        public static StringBuilder sbErr { get; private set; } = new StringBuilder();
        public static StringBuilder sbPrint { get; private set; } = new StringBuilder();
        public static StringWriter printStream { get; private set; } = new StringWriter(sbPrint);
        //public static ScriptOptions scriptOptions { get; private set; }

        static ScriptOptions scriptOptions;
        static ScriptState<object> state = null;
        static InteractiveScriptGlobals globals;
        static ScriptSourceResolver sourceResolver; 
        
        static Evaluation()
        {
            Console.SetOut(printStream);

            // Adding Default references 
            var references = new Assembly[] {
                typeof(System.Linq.Enumerable).Assembly,
                typeof(System.Text.ASCIIEncoding).Assembly,
                typeof(ICSharp.Kernel.Kernel).Assembly,
                typeof(XPlot.Plotly.Graph).Assembly
            };

            // Source paths
            string baseDirectory = Path.GetDirectoryName(typeof(Evaluation).GetTypeInfo().Assembly.ManifestModule.FullyQualifiedName);

            List<string> searchPaths = new List<string>();
            searchPaths.Add(Directory.GetCurrentDirectory());
            searchPaths.Add(Path.GetTempPath());
            
            sourceResolver = ScriptSourceResolver.Default
                .WithBaseDirectory(baseDirectory)
                .WithSearchPaths(searchPaths);

            globals = new InteractiveScriptGlobals(printStream, Microsoft.CodeAnalysis.CSharp.Scripting.Hosting.CSharpObjectFormatter.Instance);
            scriptOptions = ScriptOptions.Default
                .WithReferences(references)
                .WithSourceResolver(sourceResolver);
            
        }

        public static void EvalInteraction(string code)
        {
            var cancellationToken = new CancellationToken();

            Script<object> newScript;
            if (state == null)
            {
                newScript = CSharpScript.Create<object>(code, scriptOptions, globals.GetType(), assemblyLoader: null);
            }
            else
            {
                newScript = state.Script.ContinueWith(code, scriptOptions);
            }

            var diagnostics = newScript.Compile(cancellationToken);

            if (diagnostics.Length > 0)
            {
                foreach (var error in diagnostics)
                {
                    sbErr.Append(error.ToString());
                }

                return;
            }

            var task = (state == null) ?
                newScript.RunAsync(globals, catchException: e => false, cancellationToken: cancellationToken) :
                newScript.RunFromAsync(state, catchException: e => false, cancellationToken: cancellationToken);

            state = task.GetAwaiter().GetResult();

            //state = state == null ? CSharpScript.RunAsync(code, scriptOptions).Result :
            //    state.ContinueWithAsync(code, scriptOptions).Result;
            if (state.ReturnValue != null && !string.IsNullOrEmpty(state.ReturnValue.ToString()))
            {
                sbOut.Append(state.ReturnValue.ToString());
            }
        }


        internal static (string value, string[] errors) EvalInteractionNonThrowing(string code)
        {
            var val = string.Empty;
            var err = new List<string>();
            var cancellationToken = new CancellationToken();

            if (code.StartsWith("#help"))
            {
                var icsharpHelp = "IC# notebook directives: " +
                    "#r\t\tAdd a metadata reference to specified assembly and all its dependencies, e.g. #r \"myLib.dll\"." +
                    "#load\t\tLoad specified script file and execute it, e.g. #load \"myScript.csx\".";

                sbPrint.Append(icsharpHelp);

                return (val, err.ToArray());
            }

            Script<object> newScript;
            if (state == null)
            {
                newScript = CSharpScript.Create<object>(code, scriptOptions, globals.GetType(), assemblyLoader: null);
            }
            else
            {
                newScript = state.Script.ContinueWith(code, scriptOptions);
            }

            var diagnostics = newScript.Compile(cancellationToken);
            if (diagnostics.Length > 0)
            {
                foreach (var error in diagnostics)
                {
                    err.Add(error.ToString());
                }

                return (val, err.ToArray());
            }

            var task = (state == null) ?
                newScript.RunAsync(globals, catchException: e => true, cancellationToken: cancellationToken) :
                newScript.RunFromAsync(state, catchException: e => true, cancellationToken: cancellationToken);

            state = task.GetAwaiter().GetResult();

            if (state.Exception != null)
            {
                err.Add(state.Exception.ToString());
            }

            //state = state == null ? CSharpScript.RunAsync(code,scriptOptions).Result : state.ContinueWithAsync(code, scriptOptions).Result;
            if (state.ReturnValue != null && !string.IsNullOrEmpty(state.ReturnValue.ToString()))
            {
                val = state.ReturnValue.ToString();
            }

            return (val, err.ToArray());
        }

        internal static object GetLastExpression()
        {
            return state?.ReturnValue; 
        }
    }
}
