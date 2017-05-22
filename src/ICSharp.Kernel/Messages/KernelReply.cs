using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class KernelReply_LanguageInfo
    {
        public string name { get; set; }
        public string version { get; set; }
        public string mimetype { get; set; }
        public string file_extension { get; set; }
        public string pygments_lexer { get; set; }
        public string codemirror_mode { get; set; }
        public string nbconvert_exporter { get; set; }
    }

    public class KernelReply_HelpLink
    {
        public string text { get; set; }
        public string url { get; set; }
    }

    public class KernelReply : ShellMessage
    {
        public string protocol_version { get; set; }
        public string implementation { get; set; }
        public string implementation_version { get; set; }
        public KernelReply_LanguageInfo language_info { get; set; }
        public string language { get; set; }
        public string banner { get; set; }
        public KernelReply_HelpLink[] help_links { get; set; }
    }
}
