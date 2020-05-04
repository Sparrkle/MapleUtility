using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MapleUtility.Plugins.ViewModels.Views.Timer
{
    public class ViewModelTimerUIBar : Notifier
    {
        public int UIBarWidth
        {
            get { return Defines.UIBAR_WIDTH; }
            set
            {
                Defines.UIBAR_WIDTH = value;
                OnPropertyChanged("UIBarWidth");
            }
        }

        public int UIBarHeight
        {
            get { return Defines.UIBAR_HEIGHT; }
            set
            {
                Defines.UIBAR_HEIGHT = value;
                OnPropertyChanged("UIBarHeight");
            }
        }

        private ObservableCollection<TimerItem> runningTimerList;
        public ObservableCollection<TimerItem> RunningTimerList
        {
            get { return runningTimerList; }
            set
            {
                runningTimerList = value;
                OnPropertyChanged("RunningTimerList");
            }
        }

        private bool isShowUIBarTimerName;
        public bool IsShowUIBarTimerName
        {
            get { return isShowUIBarTimerName; }
            set
            {
                isShowUIBarTimerName = value;
                OnPropertyChanged("IsShowUIBarTimerName");
            }
        }

        #region Button Command Variables
        public ICommand CloseCommand { get; set; }
        #endregion

        public ViewModelTimerUIBar()
        {
            CloseCommand = new RelayCommand(o => CloseEvent((Window)o));
        }

        private void CloseEvent(Window window)
        {
            window.Close();
        }
    }
}
