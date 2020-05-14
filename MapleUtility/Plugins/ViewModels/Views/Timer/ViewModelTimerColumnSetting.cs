using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MapleUtility.Plugins.ViewModels.Views.Timer
{
    public class ViewModelTimerColumnSetting : Notifier
    {
        private ObservableCollection<ColumnItem> columnList;
        public ObservableCollection<ColumnItem> ColumnList
        {
            get { return columnList; }
            set
            {
                columnList = value;
                OnPropertyChanged("ColumnList");
            }
        }

        public bool IsColumnAllChecked
        {
            get
            {
                if (ColumnList == null || ColumnList.Count() == 0)
                    return false;
                else
                    return ColumnList.Where(o => o.IsVisible).Count() == ColumnList.Count();
            }
            set
            {
                if (ColumnList == null || ColumnList.Count() == 0)
                    return;

                foreach (var column in ColumnList)
                    column.IsVisible = value;

                OnPropertyChanged("IsColumnAllChecked");
            }
        }

        #region Button Command Variables
        public ICommand CheckCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion

        public ViewModelTimerColumnSetting()
        {
            CheckCommand = new RelayCommand(o => OnPropertyChanged("IsColumnAllChecked"));
            CloseCommand = new RelayCommand(o => CloseEvent((Window)o));
        }

        private void CloseEvent(Window window)
        {
            window.Close();
        }
    }
}
