using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel
{
    public class ConnectionInformation
    {
        public int stdin_port { get; set; }
        public string ip { get; set; }
        public int control_port { get; set; }
        public int hb_port { get; set; }
        public string signature_scheme { get; set; }
        public string key { get; set; }
        public int shell_port { get; set; }
        public string transport { get; set; }
        public int iopub_port { get; set; }
    }
}
