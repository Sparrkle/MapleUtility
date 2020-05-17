using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MapleUtility.Plugins.ViewModels.Views.UnionRelocate
{
    public class ViewModelUnionRelocateSyncSelect : Notifier
    {
        private bool isUtilitySyncChecked;
        public bool IsUtilitySyncChecked
        {
            get { return isUtilitySyncChecked; }
            set
            {
                isUtilitySyncChecked = value;
                OnPropertyChanged("IsUtilitySyncChecked");
                OnPropertyChanged("IsSyncEnabled");
            }
        }

        private bool isUtilitySyncEnabled;
        public bool IsUtilitySyncEnabled
        {
            get { return isUtilitySyncEnabled; }
            set
            {
                isUtilitySyncEnabled = value;
                if (value)
                    IsUtilitySyncChecked = true;
                else
                    IsMapleDataSyncChecked = true;

                OnPropertyChanged("IsUtilitySyncEnabled");
            }
        }

        private bool isMapleDataSyncChecked;
        public bool IsMapleDataSyncChecked
        {
            get { return isMapleDataSyncChecked; }
            set
            {
                isMapleDataSyncChecked = value;
                OnPropertyChanged("IsMapleDataSyncChecked");
                OnPropertyChanged("IsSyncEnabled");
            }
        }

        private string syncData;
        public string SyncData
        {
            get { return syncData; }
            set
            {
                syncData = MapleDataHelper.GetCharacterList(value);
                OnPropertyChanged("SyncData");
                OnPropertyChanged("IsSyncEnabled");
            }
        }

        public bool IsSyncEnabled
        {
            get
            {
                if(IsMapleDataSyncChecked)
                {
                    if (string.IsNullOrEmpty(SyncData))
                        return false;
                }

                return true;
            }
        }

        #region Button Command Variables
        public ICommand SyncCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion

        public ViewModelUnionRelocateSyncSelect()
        {
            SyncCommand = new RelayCommand(o => SyncEvent((Window) o));
            CloseCommand = new RelayCommand(o => CloseEvent((Window) o));
        }

        private void SyncEvent(Window window)
        {
            window.DialogResult = true;
            window.Close();
        }

        private void CloseEvent(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }
    }
}
