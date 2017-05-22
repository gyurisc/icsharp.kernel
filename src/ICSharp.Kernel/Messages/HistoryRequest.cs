using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class HistoryRequest : ShellMessage
    {
        public bool output { get; set; }
        public bool raw { get; set; }
        public string hist_access_type { get; set; }
        public int session { get; set; }
        public int start { get; set; }
        public int stop { get; set; }
        public int n { get; set; }
        public string pattern { get; set; }
        public bool unique { get; set; }
    }
}
