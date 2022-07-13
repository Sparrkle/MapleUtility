using MapleUtility.Plugins.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Models
{
    public class CenterUnionCaptureModel : Notifier
    {
        public static List<string> CenterUnionItems = new List<string>()
        {
            "STR",
            "DEX",
            "MP",
            "INT",
            "HP",
            "LUK",
            "마력",
            "공격력"
        };

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
                OnPropertyChanged("NameContainsArrow");
                OnPropertyChanged("ContextMenuItems");
            }
        }

        public string NameContainsArrow
        {
            get { return Name + " ▶"; }
        }

        public UnionCaptureType CenterVariableType;

        public ObservableCollection<string> ContextMenuItems
        {
            get
            {
                return new ObservableCollection<string>(CenterUnionItems.Where(o => o != Name));
            }
        }

        public CenterUnionCaptureModel(string name, int offset)
        {
            Name = name;

            switch (offset)
            {
                case 0:
                    CenterVariableType = UnionCaptureType.Variable11;
                    break;
                case 1:
                    CenterVariableType = UnionCaptureType.Variable1;
                    break;
                case 2:
                    CenterVariableType = UnionCaptureType.Variable10;
                    break;
                case 3:
                    CenterVariableType = UnionCaptureType.Variable2;
                    break;
                case 4:
                    CenterVariableType = UnionCaptureType.Variable8;
                    break;
                case 5:
                    CenterVariableType = UnionCaptureType.Variable4;
                    break;
                case 6:
                    CenterVariableType = UnionCaptureType.Variable7;
                    break;
                case 7:
                    CenterVariableType = UnionCaptureType.Variable5;
                    break;
            }
        }
    }
}
