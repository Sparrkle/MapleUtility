using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.ViewModels.UserControls;
using MapleUtility.Plugins.Views.UserControls;
using MapleUtility.Plugins.Views.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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

        public DispatcherTimer mainTimer = new DispatcherTimer();

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

        private void DonateEvent()
        {
            Process.Start(new ProcessStartInfo("https://toss.me/churuko"));
        }

        private void InformationEvent(Window window)
        {
            var informationWindow = new WindowInformation();
            informationWindow.Left = window.Left + (window.ActualWidth - informationWindow.Width) / 2;
            informationWindow.Top = window.Top + (window.ActualHeight - informationWindow.Height) / 2;

            informationWindow.ShowDialog();
        }
    }
}
