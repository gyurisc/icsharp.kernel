using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class Pyout
    {
        public int execution_count { get; set; }
        public Dictionary<string, object> data { get; set; }
        public Dictionary<string, object> metadata { get; set; }
    }
}
