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

        public ObservableCollection<string> ContextMenuItems
        {
            get
            {
                return new ObservableCollection<string>(CenterUnionItems.Where(o => o != Name));
            }
        }

        public CenterUnionCaptureModel(string name)
        {
            Name = name;
        }
    }
}
