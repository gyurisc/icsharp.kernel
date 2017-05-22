using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class Header
    {
        public string msg_id { get; set; }
        public string username { get; set; }
        public string session { get; set; }
        public string msg_type { get; set; }
    }
}
