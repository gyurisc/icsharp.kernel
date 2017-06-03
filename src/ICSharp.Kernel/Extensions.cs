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
                    .Select(p => p).ToArray();
            }

            table.Columns = properties.Select(p => p.Name).ToList();
            table.Rows = new List<List<string>>();

            foreach (var i in list)
            {
                var row = MakeRowFormObject(i, properties);    
            }

            return new TableOutput();
        }

        private static List<string> MakeRowFormObject<T>(T obj, PropertyInfo[] properties)
        {
            List<string> result = new List<string>();

            foreach (var p in properties)
            {
                var val = p.GetValue(obj);
                result.Add(val.ToString());
            }

            return result;
        }
    }
}
