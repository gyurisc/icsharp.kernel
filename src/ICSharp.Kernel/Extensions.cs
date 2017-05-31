using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel
{
    public static class DisplayExtensions
    {
        public static LatexOutput AsMath(this String str)
        {
            LatexOutput latex = new LatexOutput() { Latex = $"$${str}$$" };
            return latex;
        }
    }
}
