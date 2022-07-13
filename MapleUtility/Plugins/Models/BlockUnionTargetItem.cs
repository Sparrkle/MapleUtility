using MapleUtility.Plugins.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Models
{
    public class BlockUnionTargetItem
    {
        public List<RowColumnItem> RowColumns;
        public UnionCaptureType CaptureType;

        public BlockUnionTargetItem(UnionCaptureType captureType, List<RowColumnItem> rowColumns)
        {
            CaptureType = captureType;
            RowColumns = rowColumns;
        }
    }
}
