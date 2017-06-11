using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace ICSharp.Kernel
{
    public class BinaryOutput
    {
        public string ContentType { get; set; }
        public object Data { get; set; }
    }

    public class TableOutput
    {
        public List<string> Columns { get; set; }
        public List<List<string>> Rows { get; set; }
    }

    public class LatexOutput
    {
        public string Latex { get; set; }
    }

    public class HtmlOutput
    {
        public string Html { get; set; }
    }

    public class SvgOutput { public string Svg { get; set; } }

    public class Printers
    {
        private static CSharpObjectFormatter formatter = Microsoft.CodeAnalysis.CSharp.Scripting.Hosting.CSharpObjectFormatter.Instance;
        private static Dictionary<Type, Func<object, BinaryOutput>> customPrinters { get; set; }

        internal static string htmlEncode(string str)
        {
            return HttpUtility.HtmlEncode(str);
        }

        public static BinaryOutput PrintVariable(object input)
        {
            if (input == null)
            {
                return new BinaryOutput() { ContentType = "text/plain", Data = string.Empty };
            }

            BinaryOutput result = null;

            if (customPrinters.ContainsKey(input.GetType()))
            {
                var func = customPrinters[input.GetType()];
                result = func(input);
                return result;
            }

            switch (input)
            {
                case TableOutput t:
                    result = PrintTable(t);
                    break;
                case SvgOutput svg:
                    result = PrintSvg(svg);
                    break;
                case HtmlOutput html:
                    result = PrintHtml(html);
                    break;
                case LatexOutput ltx:
                    result = PrintLatex(ltx);
                    break;
                default:
                    result = DefaultDisplayPrinter(input);
                    break;
            }
                    return result;
        }

        static Printers()
        {
            customPrinters = new Dictionary<Type, Func<object, BinaryOutput>>();
        }

        public static void RegisterCustomPrinter(Type t, Func<object, BinaryOutput> callback)
        {
            if (!customPrinters.ContainsKey(t))
            {
                customPrinters.Add(t, callback);
            }
            else
            {
                customPrinters[t] = callback;
            }
        }

        #region Printers
        public static BinaryOutput DefaultDisplayPrinter(object input)
        {
            var output = formatter.FormatObject(input);

            if (output.StartsWith("Submission#"))
            {
                // Roslyn adds the Submission#N. as a prefix. Get rid of this
                output = output.Substring(output.IndexOf('.') + 1);
            }

            return new BinaryOutput() { ContentType = "text/plain", Data = output};
        }

        public static BinaryOutput PrintTable(TableOutput table)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table>");

            // header 
            sb.Append("<thead>");
            sb.Append("<tr>");

            foreach (var col in table.Columns)
            {
                sb.Append("<th>");
                sb.Append(htmlEncode(col));
                sb.Append("</th>");
            }

            sb.Append("</tr>");
            sb.Append("</thead>");

            // body 
            sb.Append("<tbody>");

            foreach (var row in table.Rows)
            {
                sb.Append("<tr>");

                foreach (var cell in row)
                {
                    sb.Append("<td>");
                    sb.Append(htmlEncode(cell));
                    sb.Append("</td>");
                }

                sb.Append("</tr>");
            }

            sb.Append("</tbody>");

            sb.Append("</table>");
            var output = new BinaryOutput() { ContentType = "text/html", Data = sb.ToString() };
            return output; 
        }

        public static BinaryOutput PrintSvg(SvgOutput g)
        {
            var output = new BinaryOutput() { ContentType = "image/svg+xml", Data = g.Svg };
            return output;
        }

        public static BinaryOutput PrintHtml(HtmlOutput input)
        {
            var output = new BinaryOutput() { ContentType = "text/html", Data = input.Html };
            return output;
        }


        public static BinaryOutput PrintLatex(LatexOutput x)
        {
            var output = new BinaryOutput() { ContentType = "text/latex", Data = x.Latex };
            return output;
        }
        #endregion

    }
}
