using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class ExecuteReplyOk : ShellMessage
    {
        public string status { get; internal set; }
        public int execution_count { get; internal set; }
        public Payload[] payload { get; internal set; }
        public object user_variables { get; internal set; }
        public object user_expressions { get; internal set; }
    }
}
