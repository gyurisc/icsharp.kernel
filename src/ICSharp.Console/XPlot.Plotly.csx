using System;
using System.IO;
using System.Reflection;
using ICSharp.Kernel; 
using XPlot.Plotly;

Func<object, BinaryOutput> PlotlyChartPrinter = s => new BinaryOutput() { ContentType = "text/html", Data = (s as PlotlyChart).GetInlineHtml() };

Printers.RegisterCustomPrinter(typeof(PlotlyChart), PlotlyChartPrinter);

string script = @" 
<script type=""text / javascript"">
var require_save = require;
var requirejs_save = requirejs;
var define_save = define;
require = requirejs = define = undefined;
%s
require = require_save;
requirejs = requirejs_save;
define = define_save;
function ifsharpMakeImage(gd, fmt)
{
    return Plotly.toImage(gd, { format: fmt})
        .then(function(url) {
        var img = document.createElement('img');
        img.setAttribute('src', url);
        var div = document.createElement('div');
        div.appendChild(img);
        gd.parentNode.replaceChild(div, gd);
    });
}
function ifsharpMakePng(gd)
{
    var fmt =
        (document.documentMode || / Edge /.test(navigator.userAgent)) ?
            'svg' : 'png';
    return ifsharpMakeImage(gd, fmt);
}
function ifsharpMakeSvg(gd)
{
    return ifsharpMakeImage(gd, 'svg');
}
</script>";

var plotly_min_js = App.ReadFileFromSourceDirectory("plotly-latest.min.js");

script = script.Replace("%s", plotly_min_js);
App.Display(script.AsHtml());

// more to come... 

