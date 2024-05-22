using FMUtils.KeyboardHook;
using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Models;
using MapleUtility.Plugins.ViewModels.UserControls;
using MapleUtility.Plugins.ViewModels.Views;
using MapleUtility.Plugins.Views.Windows.Timer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Windows.Controls;

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

            Windows11Palette.Palette.UseSystemAccentColor = true;
            Windows11Palette.Palette.FontSizeS = 10;
            Windows11Palette.Palette.FontSize = 12;
            Windows11Palette.Palette.FontSizeM = 16;
            Windows11Palette.Palette.FontSizeL = 18;
            Windows11Palette.Palette.FontSizeXL = 26;
            Windows11Palette.Palette.FocusColor = Colors.Transparent;
            Windows11ThemeSizeHelper.Helper.IsInCompactMode = true;

            var vm = new ViewModelMainWindow();
            this.DataContext = vm;
            InitializeComponent();

            var settingItem = SettingHelper.LoadSettingFile();
            if (settingItem == null)
                settingItem = new SettingItem();

            // 사운드 경로 체크
            var errorSounds = new List<string>();
            if(settingItem.SoundList != null)
            {
                foreach (var soundItem in settingItem.SoundList)
                {
                    if (!soundItem.IsInternalSound)
                    {
                        if (!File.Exists(soundItem.Path))
                            errorSounds.Add(soundItem.Path);
                    }
                }
            }

            if(errorSounds.Count() > 0)
                System.Windows.MessageBox.Show($"경고 : 다음 사운드 파일을 읽을 수 없습니다. 파일을 다시 설정해주세요.\n{string.Join("\n", errorSounds)}", "Maple Utility");

            vm.Initialize(settingItem);

            var timerVM = ucTimerHelper.DataContext as ViewModelUCTimerHelper;
            timerVM.Initialize(settingItem);

            //var unionVM = ucUnionHelper.DataContext as ViewModelUCUnionRelocateHelper;
            //unionVM.Initialize(settingItem);

            var hillaVM = ucVerusHillaHelper.DataContext as ViewModelUCVerusHillaHelper;
            hillaVM.Initialize(settingItem);

            var kalosVM = ucKalosHelper.DataContext as ViewModelUCKalosHelper;
            kalosVM.Initialize(settingItem);

            vm.mainTimer.Tick += timerVM.TickEvent;
            vm.mainTimer.Tick += hillaVM.TickEvent;
            vm.mainTimer.Tick += kalosVM.TickEvent;

            _globalKeyboardHook = new GlobalKeyboardHookHelper();
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;

            InitializeTray();

            if (settingItem.IsTray)
            {
                WindowState = System.Windows.WindowState.Minimized;
                this.Hide();
            }

            if (settingItem.IsShowUIBar)
                timerVM.OpenUIBarEvent();
            if (settingItem.IsShowHillaUIBar)
                hillaVM.OpenHillaUIBarEvent();
            if (settingItem.IsShowKalosUIBar)
                kalosVM.OpenKalosUIBarEvent();
        }

        bool isUped;
        Key prevKeyUp;
        int prevEventTime = 0;
        int prevEventUpDownTime = -1;
        int prevEventUpDownTime2 = -1;

        private void OnKeyPressed(object sender, GlobalKeyboardHookHelperEventArgs e)
        {
            var inputKey = KeyInterop.KeyFromVirtualKey(e.KeyboardData.VirtualCode);
            var modifierKeys = KeyInputHelper.GetModifierKeys(Control.ModifierKeys);
            switch(inputKey)
            {
                case Key.LeftShift:
                case Key.RightShift:
                    modifierKeys = modifierKeys | ModifierKeys.Shift; break;
                case Key.LeftAlt:
                case Key.RightAlt:
                    modifierKeys = modifierKeys | ModifierKeys.Alt; break;
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    modifierKeys = modifierKeys | ModifierKeys.Control; break;
            }

            var inputKeyString = inputKey.ToString();

            if (e.KeyboardState == GlobalKeyboardHookHelper.KeyboardState.KeyDown || e.KeyboardState == GlobalKeyboardHookHelper.KeyboardState.SysKeyDown)
            {
                if (isUped)
                {
                    prevEventUpDownTime2 = prevEventUpDownTime;
                    prevEventUpDownTime = e.KeyboardData.TimeStamp - prevEventTime;
                }

                if (inputKey == Key.LeftShift && prevEventUpDownTime == 0 && prevEventUpDownTime2 == 0)
                {
                    switch (prevKeyUp)
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
                isUped = false;
                prevEventTime = e.KeyboardData.TimeStamp;

                DebugLogHelper.Write(inputKeyString + " 키를 눌렀습니다.");

                var timerVM = ucTimerHelper.DataContext as ViewModelUCTimerHelper;
                timerVM.KeyDownEvent(modifierKeys, inputKey);

                var hillaVM = ucVerusHillaHelper.DataContext as ViewModelUCVerusHillaHelper;
                hillaVM.KeyDownEvent(modifierKeys, inputKey);

                var kalosVM = ucKalosHelper.DataContext as ViewModelUCKalosHelper;
                kalosVM.KeyDownEvent(modifierKeys, inputKey);
            }
            else if (e.KeyboardState == GlobalKeyboardHookHelper.KeyboardState.KeyUp)
            {
                //Console.WriteLine(inputKeyString + " -- " + e.KeyboardData.TimeStamp);
                isUped = true;
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
            //var unionVM = ucUnionHelper.DataContext as ViewModelUCUnionRelocateHelper;
            var hillaVM = ucVerusHillaHelper.DataContext as ViewModelUCVerusHillaHelper;
            var kalosVM = ucKalosHelper.DataContext as ViewModelUCKalosHelper;
            timerVM.RemoveAllRunningTimer();

            foreach (var timer in timerVM.TimerList)
                timer.PrevWavePlayer?.Dispose();

            var settingItem = new SettingItem
            {
                TimerList = timerVM.TimerList,
                SoundList = vm.SoundList,
                PresetList = timerVM.PresetList,
                ImageList = timerVM.ImageList,
                ColumnList = timerVM.ColumnList,
                SelectedUIBarStyle = timerVM.SelectedUIBarStyle,
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

                UIBarFontSize = timerVM.UIBarFontSize,
                SelectedUIBarFontName = timerVM.SelectedUIBarFont.Source,

                UIBAR_TOP = timerVM.UIBarTop,
                UIBAR_LEFT = timerVM.UIBarLeft,
                UIBAR_TRANSPARENCY = timerVM.UIBarTransparency,
                UIBAR_WIDTH = timerVM.UIBarWidth,
                UIBAR_HEIGHT = timerVM.UIBarHeight,
                HILLA_UIBAR_TOP = hillaVM.UIBarTop,
                HILLA_UIBAR_LEFT = hillaVM.UIBarLeft,
                HILLA_UIBAR_TRANSPARENCY = hillaVM.UIBarTransparency,
                HILLA_UIBAR_WIDTH = hillaVM.UIBarWidth,
                HILLA_UIBAR_HEIGHT = hillaVM.UIBarHeight,
                HILLA_VOLUME = hillaVM.Volume,
                KALOS_UIBAR_TOP = kalosVM.UIBarTop,
                KALOS_UIBAR_LEFT = kalosVM.UIBarLeft,
                KALOS_UIBAR_TRANSPARENCY = kalosVM.UIBarTransparency,
                KALOS_UIBAR_WIDTH = kalosVM.UIBarWidth,
                KALOS_UIBAR_HEIGHT = kalosVM.UIBarHeight,
                KALOS_INSTANT_VOLUME = kalosVM.InstantVolume,
                KALOS_InstanceKeyItems = kalosVM.InstanceKeyItems,

                IsTray = WindowState == WindowState.Minimized,
                IsShowUIBar = (WindowTimerUIBar.Instance as WindowTimerUIBar).IsVisible,
                IsShowHillaUIBar = (WindowVerusHillaUIBar.Instance as WindowVerusHillaUIBar).IsVisible,
                IsShowKalosUIBar = (WindowKalosUIBar.Instance as WindowKalosUIBar).IsVisible,

                //CharacterList = unionVM.CharacterList,
                //BlockManager = unionVM.BlockManager,
                //SelectedRank = unionVM.SelectedRank,
                //CapturePriorityList = unionVM.CapturePriorityList
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
                var time = new DateTime(now.Year, now.Month, now.Day);
                if (time <= lastestTime)
                    return;
            }

            var windowChurukoLab = new WindowChurukoLab();
            try
            {
                windowChurukoLab.Show();
            }
            catch(Exception)
            {
                windowChurukoLab.Close();
            }
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

        public void BringToForeground()
        {
            if (this.WindowState == WindowState.Minimized || this.Visibility == Visibility.Hidden)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            }

            // According to some sources these steps gurantee that an app will be brought to foreground.
            this.Activate();
            this.Topmost = true;
            this.Topmost = false;
            this.Focus();
        }
    }
}
