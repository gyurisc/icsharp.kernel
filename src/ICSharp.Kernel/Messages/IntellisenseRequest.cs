using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class IntellisenseRequest : ShellMessage
    {
        public string text { get; set; }
        public string line { get; set; }
        public string block { get; set; }
        public int cursor_pos { get; set; }

    }
}
