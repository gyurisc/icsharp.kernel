using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ICSharp.Kernel
{
    public class Evaluation
    {
        // THese properties are probably not needed anymore... 
        public static StringBuilder sbOut { get; private set; } = new StringBuilder();
        public static StringBuilder sbErr { get; private set; } = new StringBuilder();
        public static StringBuilder sbPrint { get; private set; } = new StringBuilder();
        public static StringWriter printStream { get; private set; } = new StringWriter(sbPrint);
        public static ScriptOptions scriptOptions { get; private set; }

        public static ScriptState<object> scriptState = null;
      
        static Evaluation()
        {
            Console.SetOut(printStream);

            // Adding Default references 
            var references = new Assembly[] {
                typeof(System.Linq.Enumerable).Assembly,
                typeof(System.Text.ASCIIEncoding).Assembly,
                typeof(ICSharp.Kernel.Kernel).Assembly
            };

            scriptOptions = ScriptOptions.Default.WithReferences(references);           
        }

        internal static void EvalInteraction(string code)
        {
            try
            {
                scriptState = scriptState == null ? CSharpScript.RunAsync(code, scriptOptions).Result : scriptState.ContinueWithAsync(code, scriptOptions).Result;
                if (scriptState.ReturnValue != null && !string.IsNullOrEmpty(scriptState.ReturnValue.ToString()))
                {
                    sbOut.Append(scriptState.ReturnValue.ToString());
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
                scriptState = scriptState == null ? CSharpScript.RunAsync(code,scriptOptions).Result : scriptState.ContinueWithAsync(code, scriptOptions).Result;
                if (scriptState.ReturnValue != null && !string.IsNullOrEmpty(scriptState.ReturnValue.ToString()))
                {
                    val = scriptState.ReturnValue.ToString();
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

        internal static string GetLastExpression()
        {
            if (scriptState == null) return string.Empty;
            if (scriptState.ReturnValue == null) return string.Empty;
            return scriptState.ReturnValue.ToString(); 
        }

        internal static void GetDeclarations(string code, int realLineNumber, int ch)
        {
            ScriptState<object> state = null;
            var scriptFileName = Path.Combine(Environment.CurrentDirectory, "script.csx");

            try
            {
                var script = CSharpScript.Create(code, scriptOptions);
                script.Compile();
                state = script.RunAsync().Result;
               
                if (scriptState.ReturnValue != null && !string.IsNullOrEmpty(scriptState.ReturnValue.ToString()))
                {
                    var val = scriptState.ReturnValue.ToString();
                }
            }
            catch (CompilationErrorException compilationError)
            {
                foreach (var error in compilationError.Diagnostics)
                {
                    //var err.Add(error.ToString());
                }
            }

        }
    }
}
