using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MapleUtility.Plugins.ViewModels.Views.UnionRelocate
{
    public class ViewModelUnionRelocatePrioritySetting : Notifier
    {
        private ObservableCollection<CapturePriorityItem> capturePriorityList;
        public ObservableCollection<CapturePriorityItem> CapturePriorityList
        {
            get { return capturePriorityList; }
            set
            {
                capturePriorityList = value;
                OnPropertyChanged("CapturePriorityList");
            }
        }

        #region Button Command Variables
        public ICommand CloseCommand { get; set; }
        public ICommand UpCommand { get; set; }
        public ICommand DownCommand { get; set; }
        #endregion

        public ViewModelUnionRelocatePrioritySetting()
        {
            CloseCommand = new RelayCommand(o => CloseEvent((Window) o));
            UpCommand = new RelayCommand(o => UpEvent((CapturePriorityItem)o));
            DownCommand = new RelayCommand(o => DownEvent((CapturePriorityItem)o));
        }

        private void UpEvent(CapturePriorityItem item)
        {
            item.ChangeTarget(CapturePriorityList[item.Priority - 2]);
        }

        private void DownEvent(CapturePriorityItem item)
        {
            item.ChangeTarget(CapturePriorityList[item.Priority]);
        }

        private void CloseEvent(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }
    }
}
