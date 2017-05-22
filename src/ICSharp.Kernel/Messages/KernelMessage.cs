using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class KernelMessage
    {
        public List<byte[]> Identifiers { get; set; }
        public string HmacSignature { get; set; }
        public Header Header { get; set; }
        public Header ParentHeader { get; set; }
        public string Metadata { get; set; }
        public ShellMessage Content { get; set; }
    }
}
