using EnumsNET;
using MapleUtility.Plugins.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Models
{
    public class CapturePriorityItem : Notifier
    {
        private UnionCaptureType captureEnum;
        public UnionCaptureType CaptureEnum
        {
            get { return captureEnum; }
            set
            {
                captureEnum = value;
                OnPropertyChanged("CaptureEnum");
                OnPropertyChanged("Name");
            }
        }

        public string Name
        {
            get
            {
                if (CaptureEnum == UnionCaptureType.None)
                    return "";

                return captureEnum.AsString(EnumFormat.Description);
            }
        }

        private int priority;
        public int Priority
        {
            get { return priority; }
            set
            {
                priority = value;
                OnPropertyChanged("Priority");
            }
        }

        private int captureCount;
        public int CaptureCount
        {
            get { return captureCount; }
            set
            {
                if (value < 0)
                    captureCount = 0;
                else if (value > captureMax)
                    captureCount = captureMax;
                else
                    captureCount = value;
                OnPropertyChanged("CaptureCount");
            }
        }

        private int captureMax;
        public int CaptureMax
        {
            get { return captureMax; }
            set
            {
                captureMax = value;
                captureCount = value;
                OnPropertyChanged("CaptureMax");
            }
        }

        public CapturePriorityItem(UnionCaptureType captureEnum, int priority, int captureMax)
        {
            CaptureEnum = captureEnum;
            Priority = priority;
            CaptureMax = captureMax;
        }

        public void ChangeTarget(CapturePriorityItem target)
        {
            var targetEnum = target.CaptureEnum;
            var targetMaxCount = target.CaptureMax;
            var targetCount = target.CaptureCount;

            target.CaptureEnum = CaptureEnum;
            target.CaptureMax = CaptureMax;
            target.CaptureCount = CaptureCount;

            CaptureEnum = targetEnum;
            CaptureMax = targetMaxCount;
            CaptureCount = targetCount;
        }
    }
}
