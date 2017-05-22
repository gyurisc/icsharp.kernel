using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class Pyin
    {
        public string code { get; set; }
        public int execution_count { get; set; }
    }
}
