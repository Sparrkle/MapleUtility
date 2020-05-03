using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Models;
using MapleUtility.Plugins.ViewModels.UserControls;
using MapleUtility.Plugins.ViewModels.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MapleUtility.Plugins.Views.Windows
{
    /// <summary>
    /// WindowMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowMain : Window
    {
        private GlobalKeyboardHookHelper _globalKeyboardHook;

        public WindowMain()
        {
            var vm = new ViewModelMainWindow();
            this.DataContext = vm;
            InitializeComponent();

            var settingItem = SettingHelper.LoadSettingFile();
            if (settingItem == null)
                settingItem = new SettingItem();

            Defines.UIBAR_WIDTH = settingItem.UIBAR_WIDTH;
            Defines.UIBAR_HEIGHT = settingItem.UIBAR_HEIGHT;

            var timerVM = ucTimerHelper.DataContext as ViewModelUCTimerHelper;
            timerVM.Initialize(settingItem);

            vm.mainTimer.Tick += timerVM.TickEvent;

            _globalKeyboardHook = new GlobalKeyboardHookHelper();
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;
        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookHelperEventArgs e)
        {
            if(e.KeyboardState == GlobalKeyboardHookHelper.KeyboardState.KeyDown)
            {
                DebugLogHelper.Write(KeyInterop.KeyFromVirtualKey(e.KeyboardData.VirtualCode).ToString() + " 키를 눌렀습니다.");
                var timerVM = ucTimerHelper.DataContext as ViewModelUCTimerHelper;
                timerVM.KeyDownEvent(e);
            }
            else if(e.KeyboardState == GlobalKeyboardHookHelper.KeyboardState.KeyUp)
            {
                DebugLogHelper.Write(KeyInterop.KeyFromVirtualKey(e.KeyboardData.VirtualCode).ToString() + " 키를 뗐습니다.");
                var timerVM = ucTimerHelper.DataContext as ViewModelUCTimerHelper;
                timerVM.KeyUpEvent(e);
            }
            else if(e.KeyboardState == GlobalKeyboardHookHelper.KeyboardState.SysKeyDown)
                DebugLogHelper.Write(KeyInterop.KeyFromVirtualKey(e.KeyboardData.VirtualCode).ToString() + " 시스템키를 눌렀습니다.");
            else
                DebugLogHelper.Write(KeyInterop.KeyFromVirtualKey(e.KeyboardData.VirtualCode).ToString() + " 시스템키를 뗐습니다.");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var vm = this.DataContext as ViewModelMainWindow;
            vm.mainTimer.Stop();

            var timerVM = ucTimerHelper.DataContext as ViewModelUCTimerHelper;
            timerVM.RemoveAllRunningTimer();

            foreach (var timer in timerVM.TimerList)
                timer.PrevWavePlayer?.Dispose();

            var settingItem = new SettingItem
            {
                TimerList = timerVM.TimerList,
                SoundList = timerVM.SoundList,
                PresetList = timerVM.PresetList,
                SelectedPreset = timerVM.SelectedPreset,
                AlertDuration = timerVM.AlertDuration,
                IsAlertShowScreenChecked = timerVM.IsAlertShowScreenChecked,
                IsTimerResetChecked = timerVM.IsTimerResetChecked,
                TimerOnOffKey = timerVM.TimerOnOffKey,
                TimerOnOffModifierKey = timerVM.TimerOnOffModifierKey,
                UIBAR_WIDTH = Defines.UIBAR_WIDTH,
                UIBAR_HEIGHT = Defines.UIBAR_HEIGHT
            };
            SettingHelper.SaveSettingFile(settingItem);

            Application.Current.Shutdown();
        }
    }
}
