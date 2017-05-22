using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class ExecuteReplyError : ShellMessage
    {
        public string status { get; set; }
        public int execution_count { get; set; }

        public string ename { get; set; }
        public string evalue { get; set; }
        public string[] traceback { get; set; }

    }
}
