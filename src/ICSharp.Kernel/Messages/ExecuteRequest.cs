using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class ExecuteRequest : ShellMessage
    {
        public string code { get; set; }
        public bool silent { get; set; }
        public bool store_history { get; set; }
    }
}
