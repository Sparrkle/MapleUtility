using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Models
{
    public class RowColumnItem
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public RowColumnItem(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
