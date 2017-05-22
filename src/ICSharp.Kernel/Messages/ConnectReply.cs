using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class ConnectReply : ShellMessage
    {
        public int shell_port { get; set; }
        public int iopub_port { get; set; }
        public int stdin_port { get; set; }
        public int hb_port { get; set; }

    }
}
