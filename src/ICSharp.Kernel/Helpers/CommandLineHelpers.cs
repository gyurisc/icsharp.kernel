using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel
{
    internal static class CommandLineHelpers
    {
        internal static ScriptOptions RemoveImportsAndReferences(this ScriptOptions options)
        {
            return options.WithReferences(Array.Empty<MetadataReference>()).WithImports(Array.Empty<string>());
        }
    }
}
