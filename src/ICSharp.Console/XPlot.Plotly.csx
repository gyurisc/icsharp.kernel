using System;
using ICsharp.Kernel; 
using XPlot.Plotly;

Func<object, BinaryOutput> PlotlyChartPrinter = s => new BinaryOutput() { ContentType = "text/html", Data = PlotlyChartPrinter.GetInlineHtml() };

Printers.RegisterCustomPrinter(typeof(PlotlyChart), PlotlyChartPrinter);