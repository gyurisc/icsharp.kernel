using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class ClearOutput
    {
        public bool wait { get; internal set; }
        public bool stedrr { get; internal set; }
        public bool stdout { get; internal set; }
        public bool other { get; internal set; }
    }
}
