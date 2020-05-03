using MapleUtility.Plugins.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.ViewModels.Views
{
    public class ViewModelDebug : Notifier
    {
        private ObservableCollection<string> debugTextList = new ObservableCollection<string>();
        public ObservableCollection<string> DebugTextList
        {
            get { return debugTextList; }
            set
            {
                debugTextList = value;
                OnPropertyChanged("DebugTextList");
            }
        }
    }
}
