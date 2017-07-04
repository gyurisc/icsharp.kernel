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

            globals = new InteractiveScriptGlobals(printStream, Microsoft.CodeAnalysis.CSharp.Scripting.Hosting.CSharpObjectFormatter.Instance);
            scriptOptions = ScriptOptions.Default.WithReferences(references);
        }

        public static void EvalInteraction(string code)
        {
            try
            {
                Script<object> newScript;
                if (state == null)
                {
                    newScript = CSharpScript.Create<object>(code, scriptOptions, globals.GetType(), assemblyLoader: null);
                }
                else
                {
                    newScript = state.Script.ContinueWith(code, scriptOptions);
                }

                state = state == null ? CSharpScript.RunAsync(code, scriptOptions).Result : 
                    state.ContinueWithAsync(code, scriptOptions).Result;
                if (state.ReturnValue != null && !string.IsNullOrEmpty(state.ReturnValue.ToString()))
                {
                    sbOut.Append(state.ReturnValue.ToString());
                }
            }
            catch (CompilationErrorException compilationError)
            {
                foreach (var error in compilationError.Diagnostics)
                {
                    sbErr.Append(error.ToString());                
                }
            }
            
        }

        internal static (string value, string[] errors) EvalInteractionNonThrowing(string code)
        {
            var val = string.Empty;
            var err = new List<string>();

            try
            {
                if (code.StartsWith("#help"))
                {
                    var icsharpHelp = "IC# notebook directives: " +
                        "#r\t\tAdd a metadata reference to specified assembly and all its dependencies, e.g. #r \"myLib.dll\"." +
                        "#load\t\tLoad specified script file and execute it, e.g. #load \"myScript.csx\".";

                    sbPrint.Append(icsharpHelp);

                    return (val, err.ToArray());
                }
  
                state = state == null ? CSharpScript.RunAsync(code,scriptOptions).Result : state.ContinueWithAsync(code, scriptOptions).Result;
                if (state.ReturnValue != null && !string.IsNullOrEmpty(state.ReturnValue.ToString()))
                {
                    val = state.ReturnValue.ToString();
                }
            }
            catch (CompilationErrorException compilationError)
            {
                foreach(var error in compilationError.Diagnostics)
                {
                    err.Add(error.ToString());
                }         
            }
            

            return (val, err.ToArray());
        }

        internal static object GetLastExpression()
        {
            return state?.ReturnValue; 
        }

        internal static ScriptOptions UpdateOptions(ScriptOptions options, InteractiveScriptGlobals globals)
        {
            var currentMetadataResolver = options.MetadataResolver;
            var currentSourceResolver = options.SourceResolver;

            string newWorkingDirectory = Directory.GetCurrentDirectory();
            var newReferenceSearchPaths = ImmutableArray.CreateRange(globals.ReferencePaths);
            var newSourceSearchPaths = ImmutableArray.CreateRange(globals.SourcePaths);

            // remove references and imports from the options, they have been applied and will be inherited from now on:
            //return options.
            //    RemoveImportsAndReferences().
            //    WithMetadataResolver(currentMetadataResolver.
            //        WithRelativePathResolver(
            //            currentMetadataResolver.PathResolver.
            //                WithBaseDirectory(newWorkingDirectory).
            //                WithSearchPaths(newReferenceSearchPaths))).
            //    WithSourceResolver(currentSourceResolver.
            //            WithBaseDirectory(newWorkingDirectory).
            //            WithSearchPaths(newSourceSearchPaths));
            return options;
        }


    }
}
