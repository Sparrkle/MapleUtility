using FMUtils.KeyboardHook;
using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Models;
using MapleUtility.Plugins.ViewModels.UserControls;
using MapleUtility.Plugins.ViewModels.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
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

            Defines.HILLA_UIBAR_WIDTH = settingItem.HILLA_UIBAR_WIDTH;
            Defines.HILLA_UIBAR_HEIGHT = settingItem.HILLA_UIBAR_HEIGHT;

            var timerVM = ucTimerHelper.DataContext as ViewModelUCTimerHelper;
            timerVM.Initialize(settingItem);

            var unionVM = ucUnionHelper.DataContext as ViewModelUCUnionRelocateHelper;
            unionVM.Initialize(settingItem);

            var hillaVM = ucVerusHillaHelper.DataContext as ViewModelUCVerusHillaHelper;
            hillaVM.Initialize(settingItem);

            vm.mainTimer.Tick += timerVM.TickEvent;
            vm.mainTimer.Tick += hillaVM.TickEvent;

            _globalKeyboardHook = new GlobalKeyboardHookHelper();
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;

            InitializeTray();
        }

        Key prevKeyUp;
        int prevEventTime = 0;

        private void OnKeyPressed(object sender, GlobalKeyboardHookHelperEventArgs e)
        {
            var inputKey = KeyInterop.KeyFromVirtualKey(e.KeyboardData.VirtualCode);
            var modifierKeys = KeyInputHelper.GetModifierKeys(Control.ModifierKeys);
            var keyEventTime = e.KeyboardData.TimeStamp - prevEventTime;

            if (prevKeyUp == Key.LeftShift && keyEventTime == 0)
            {
                switch(inputKey)
                {
                    case Key.Delete:
                        inputKey = Key.Decimal;
                        break;
                    case Key.Insert:
                        inputKey = Key.NumPad0;
                        break;
                    case Key.End:
                        inputKey = Key.NumPad1;
                        break;
                    case Key.Down:
                        inputKey = Key.NumPad2;
                        break;
                    case Key.PageDown:
                        inputKey = Key.NumPad3;
                        break;
                    case Key.Left:
                        inputKey = Key.NumPad4;
                        break;
                    case Key.Clear:
                        inputKey = Key.NumPad5;
                        break;
                    case Key.Right:
                        inputKey = Key.NumPad6;
                        break;
                    case Key.Home:
                        inputKey = Key.NumPad7;
                        break;
                    case Key.Up:
                        inputKey = Key.NumPad8;
                        break;
                    case Key.PageUp:
                        inputKey = Key.NumPad9;
                        break;
                }
            }

            var inputKeyString = inputKey.ToString();

            if (e.KeyboardState == GlobalKeyboardHookHelper.KeyboardState.KeyDown)
            {
                prevEventTime = e.KeyboardData.TimeStamp;
                DebugLogHelper.Write(inputKeyString + " 키를 눌렀습니다.");

                var timerVM = ucTimerHelper.DataContext as ViewModelUCTimerHelper;
                timerVM.KeyDownEvent(modifierKeys, inputKey);

                var hillaVM = ucVerusHillaHelper.DataContext as ViewModelUCVerusHillaHelper;
                hillaVM.KeyDownEvent(modifierKeys, inputKey);
            }
            else if (e.KeyboardState == GlobalKeyboardHookHelper.KeyboardState.KeyUp)
            {
                prevKeyUp = inputKey;
                prevEventTime = e.KeyboardData.TimeStamp;
                DebugLogHelper.Write(inputKeyString + " 키를 뗐습니다.");
            }
            else if (e.KeyboardState == GlobalKeyboardHookHelper.KeyboardState.SysKeyDown)
                DebugLogHelper.Write(inputKeyString + " 시스템키를 눌렀습니다.");
            else
                DebugLogHelper.Write(inputKeyString + " 시스템키를 뗐습니다.");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var vm = this.DataContext as ViewModelMainWindow;
            vm.mainTimer.Stop();

            var timerVM = ucTimerHelper.DataContext as ViewModelUCTimerHelper;
            var unionVM = ucUnionHelper.DataContext as ViewModelUCUnionRelocateHelper;
            var hillaVM = ucVerusHillaHelper.DataContext as ViewModelUCVerusHillaHelper;
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

                BackKey = hillaVM.BackKey,
                BackModifierKey = hillaVM.BackModifierKey,
                ScytheKey = hillaVM.ScytheKey,
                ScytheModifierKey = hillaVM.ScytheModifierKey,
                NextKey = hillaVM.NextKey,
                NextModifierKey = hillaVM.NextModifierKey,

                UIBAR_WIDTH = Defines.UIBAR_WIDTH,
                UIBAR_HEIGHT = Defines.UIBAR_HEIGHT,
                HILLA_UIBAR_WIDTH = Defines.HILLA_UIBAR_WIDTH,
                HILLA_UIBAR_HEIGHT = Defines.HILLA_UIBAR_HEIGHT,

                CharacterList = unionVM.CharacterList,
                BlockManager = unionVM.BlockManager
            };
            SettingHelper.SaveSettingFile(settingItem);

            App.ni.Visible = false;
            App.ni.Dispose();
            App.Current.Shutdown();
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

        private void InitializeTray()
        {
            System.Windows.Forms.ContextMenu menu = new System.Windows.Forms.ContextMenu();    // Menu 객체

            System.Windows.Forms.MenuItem closeItem = new System.Windows.Forms.MenuItem();    // Menu 객체에 들어갈 각각의 menu
            closeItem.Index = 0;
            closeItem.Text = "종료";    // menu 이름

            closeItem.Click += delegate (object click, EventArgs eClick)    // menu 의 클릭 이벤트 등록
            {
                App.Current.Shutdown();
            };

            menu.MenuItems.Add(closeItem);

            App.ni.Icon = Properties.Resources.UffieIcon;    // 아이콘 등록 2번째 방법
            App.ni.Visible = true;
            App.ni.DoubleClick += delegate (object senders, EventArgs args)    // Tray icon의 더블 클릭 이벤트 등록
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                this.Activate();
                this.Topmost = true;  // important
                this.Topmost = false; // important
                this.Focus();         // important
            };
            App.ni.ContextMenu = menu;    // Menu 객체 등록
            App.ni.Text = "Maple Utility";    // Tray icon 이름
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();
            else
            {
                this.Show();
                this.Activate();
                this.Topmost = true;  // important
                this.Topmost = false; // important
                this.Focus();         // important
            }

            base.OnStateChanged(e);
        }
    }
}
