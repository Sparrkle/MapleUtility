using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Models
{
    public class ColumnItem : Notifier
    {
        private int id;
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private bool isVisible = true;
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }

        private int index = -1;
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                OnPropertyChanged("Index");
            }
        }

        public ColumnItem(int id, string name, int index)
        {
            Id = id;
            Name = name;
            Index = index;
        }
    }
}
