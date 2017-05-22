using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class CompleteReply : ShellMessage
    {
        public object matches { get; set; }
        public string matched_text { get; set; }
        public string status { get; set; }
        public int filter_start_index { get; set; }
    }
}
