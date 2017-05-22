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
        internal static string htmlEncode(string str)
        {
            return HttpUtility.HtmlEncode(str);
        }


        #region Printers
        public static BinaryOutput defaultDisplayPrinter(object x)
        {
            return new BinaryOutput() { ContentType = "text/plain", Data = x.ToString()};
        }

        public static BinaryOutput tablePrinter(TableOutput x)
        {
            var output = new BinaryOutput() { ContentType = "text/plain", Data = "Table printer is not yet supported!" };
            return output; 
        }

        public static BinaryOutput svgPrinter(SvgOutput x)
        {
            var output = new BinaryOutput() { ContentType = "image/svg+xml", Data = x.Svg };
            return output;
        }

        public static BinaryOutput htmlPrinter(HtmlOutput x)
        {
            var output = new BinaryOutput() { ContentType = "text/html", Data = x.Html };
            return output;
        }


        public static BinaryOutput htmlPrinter(LatexOutput x)
        {
            var output = new BinaryOutput() { ContentType = "text/latex", Data = x.Latex };
            return output;
        }

        #endregion
        public static void addDefaultDisplayPrinters()
        {
            

            /*
                /// Adds default display printers
                let addDefaultDisplayPrinters() =

                    // add table printer
                    addDisplayPrinter(fun (x:TableOutput) ->
                        let sb = StringBuilder()
                        sb.Append("<table>") |> ignore

                        // output header
                        sb.Append("<thead>") |> ignore
                        sb.Append("<tr>") |> ignore
                        for col in x.Columns do
                            sb.Append("<th>") |> ignore
                            sb.Append(htmlEncode col) |> ignore
                            sb.Append("</th>") |> ignore
                        sb.Append("</tr>") |> ignore
                        sb.Append("</thead>") |> ignore

                        // output body
                        sb.Append("<tbody>") |> ignore
                        for row in x.Rows do
                            sb.Append("<tr>") |> ignore
                            for cell in row do
                                sb.Append("<td>") |> ignore
                                sb.Append(htmlEncode cell) |> ignore
                                sb.Append("</td>") |> ignore

                            sb.Append("</tr>") |> ignore
                        sb.Append("<tbody>") |> ignore
                        sb.Append("</tbody>") |> ignore
                        sb.Append("</table>") |> ignore

                        { ContentType = "text/html"; Data = sb.ToString() }
                    )
             */
        }
    }
}
