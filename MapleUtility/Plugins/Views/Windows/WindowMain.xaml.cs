using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.ViewModels.UserControls;
using MapleUtility.Plugins.ViewModels.Views;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

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
            if (!Directory.Exists(Defines.ImageFolderPath))
                Directory.CreateDirectory(Defines.ImageFolderPath);

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

            var unionVM = ucUnionHelper.DataContext as ViewModelUCUnionRelocateHelper;
            unionVM.Initialize(settingItem);

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
            var unionVM = ucUnionHelper.DataContext as ViewModelUCUnionRelocateHelper;
            timerVM.RemoveAllRunningTimer();

            foreach (var timer in timerVM.TimerList)
                timer.PrevWavePlayer?.Dispose();

            var settingItem = new SettingItem
            {
                TimerList = timerVM.TimerList,
                SoundList = timerVM.SoundList,
                PresetList = timerVM.PresetList,
                ImageList = timerVM.ImageList,
                ColumnList = timerVM.ColumnList,
                SelectedPreset = timerVM.SelectedPreset,
                RemainSquareColor = timerVM.RemainSquareColor,
                RemainBackAlpha = timerVM.RemainBackAlpha,
                AlertDuration = timerVM.AlertDuration,
                IsShowUIBarTimerName = timerVM.IsShowUIBarTimerName,
                IsAlertShowScreenChecked = timerVM.IsAlertShowScreenChecked,
                TimerOnOffKey = timerVM.TimerOnOffKey,
                TimerOnOffModifierKey = timerVM.TimerOnOffModifierKey,
                PauseAllKey = timerVM.PauseAllKey,
                PauseAllModifierKey = timerVM.PauseAllModifierKey,
                TimerLockKey = timerVM.TimerLockKey,
                TimerLockModifierKey = timerVM.TimerLockModifierKey,
                UIBAR_WIDTH = Defines.UIBAR_WIDTH,
                UIBAR_HEIGHT = Defines.UIBAR_HEIGHT,

                CharacterList = unionVM.CharacterList,
            };
            SettingHelper.SaveSettingFile(settingItem);

            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var lastestTime = SettingHelper.LoadSettingChurukoLab();
            
            if(lastestTime.HasValue)
            {
                var now = DateTime.Now;
                if (now.Year < lastestTime.Value.Year || now.Month < lastestTime.Value.Month || now.Day <= lastestTime.Value.Day)
                    return;
            }

            var windowChurukoLab = new WindowChurukoLab();
            windowChurukoLab.Show();
        }
    }
}
