using MapleUtility.Plugins.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Models
{
    public class RelocateCaptureItem
    {
        public int BlockCount { get; set; }
        public UnionCaptureType CaptureType { get; set; }

        public bool IsCentre
        {
            get
            {
                switch(CaptureType)
                {
                    case UnionCaptureType.DEX:
                    case UnionCaptureType.STR:
                    case UnionCaptureType.LUK:
                    case UnionCaptureType.INT:
                    case UnionCaptureType.AttackPoint:
                    case UnionCaptureType.MagicAttack:
                    case UnionCaptureType.HP:
                    case UnionCaptureType.MP:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public RelocateCaptureItem(UnionCaptureType captureType, int blockCount)
        {
            CaptureType = captureType;
            BlockCount = blockCount;
        }
    }
}
