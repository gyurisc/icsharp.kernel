using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static TableOutput AsTable<T>(this IEnumerable<T> list, List<string> propertyNames = null)
        {
            var table = new TableOutput();

            PropertyInfo[] properties = typeof(T).GetProperties();

            if (propertyNames != null && propertyNames.Count > 0)
            {
                properties = properties
                    .Where(p => propertyNames.Contains(p.Name))
                    .Select(p);
            }


            foreach (var i in list)
            {

            }
            return new TableOutput();
        }
    }
}
