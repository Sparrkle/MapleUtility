using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Models
{
    public class PathfindResultItem
    {
        public List<BlockItem> Blocks { get; set; } = new List<BlockItem>();
        public int ResultCost;
    }
}
