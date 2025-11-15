using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapleUtility.Plugins.Models
{
    public class CommandItem
    {
        public DateTime PressedTime { get; set; }
        public Key? Key { get; set; }
    }
}
