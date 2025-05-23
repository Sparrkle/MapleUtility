﻿using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Models;
using MapleUtility.Plugins.ViewModels.UserControls;
using MapleUtility.Plugins.Views.UserControls;
using MapleUtility.Plugins.Views.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Telerik.Windows.Controls;

namespace MapleUtility.Plugins.ViewModels.Views
{
    public class ViewModelMainWindow : Notifier
    {
        private RadTabItem selectedTab;
        public RadTabItem SelectedTab
        {
            get { return selectedTab; }
            set
            {
                if(selectedTab != null)
                {
                    if (selectedTab.Header.ToString() == "Timer Helper")
                    {
                        if (value.Header.ToString() != "Timer Helper")
                        {
                            var vm = (selectedTab.Content as UserControlTimerHelper).DataContext as ViewModelUCTimerHelper;
                            vm.IsOpenSettingWindow = true;
                        }
                    }
                }

                if (value.Header.ToString() == "Timer Helper")
                {
                    var vm = (value.Content as UserControlTimerHelper)?.DataContext as ViewModelUCTimerHelper;
                    if(vm != null)
                        vm.IsOpenSettingWindow = false;
                }

                selectedTab = value;
                OnPropertyChanged("SelectedTab");
            }
        }

        private ObservableCollection<SoundItem> soundList;
        public ObservableCollection<SoundItem> SoundList
        {
            get { return soundList; }
            set
            {
                soundList = value;
                OnPropertyChanged("SoundList");
            }
        }

        public IEnumerable<SoundItem> OrderedSoundList
        {
            get { return SoundList.OrderBy(o => o.Priority); }
        }

        public DispatcherTimer mainTimer = new DispatcherTimer();
        public List<RadTabItem> TabItems { get; set; }

        #region Button Command Variables
        public ICommand DonateCommand { get; set; }
        public ICommand InformationCommand { get; set; }
        #endregion

        public ViewModelMainWindow()
        {
            DonateCommand = new RelayCommand(o => DonateEvent());
            InformationCommand = new RelayCommand(o => InformationEvent((Window)o));

            mainTimer = new DispatcherTimer(DispatcherPriority.Render);
            mainTimer.Interval = TimeSpan.FromSeconds(0.03);
            mainTimer.Start();
        }

        public void Initialize(SettingItem settingItem)
        {
            if (settingItem.SoundList == null)
                SoundList = InitialHelper.InitializeSoundList();
            else
            {
                if(settingItem.SoundList.FirstOrDefault()?.Priority == -1)
                {
                    var priority = 1;
                    foreach (var sound in settingItem.SoundList)
                        sound.Priority = priority++;
                }
                SoundList = settingItem.SoundList;
            }
        }

        private void DonateEvent()
        {
            MessageBox.Show("토스 1001-8787-5135 ㅅㅇㅊ", "후원 정보");
        }

        private void InformationEvent(Window window)
        {
            var informationWindow = new WindowInformation();
            informationWindow.Left = window.Left + (window.ActualWidth - informationWindow.Width) / 2;
            informationWindow.Top = window.Top + (window.ActualHeight - informationWindow.Height) / 2;

            informationWindow.ShowDialog();
        }

        public void ChangeSoundList()
        {
            OnPropertyChanged("SoundList");
            OnPropertyChanged("OrderedSoundList");
            ItemCheckInViewModels();
        }

        public void ItemCheckInViewModels()
        {
            foreach(var tab in TabItems)
            {
                var vm = tab.DataContext as IViewModelItemAvailable;
                if (vm == null)
                    continue;

                vm.ItemCheckEvent();
            }
        }
    }
}
