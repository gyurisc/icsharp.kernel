using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class Payload
    {
        public string html { get; set; }
        public string source { get; set; }
        public int start_line_number { get; set; }
        public string text { get; set; }
    }
}
