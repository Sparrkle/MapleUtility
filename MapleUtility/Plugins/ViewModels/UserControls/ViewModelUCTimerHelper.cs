using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Models;
using MapleUtility.Plugins.ViewModels.Views;
using MapleUtility.Plugins.ViewModels.Views.Timer;
using MapleUtility.Plugins.Views.Windows.Timer;
using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using Telerik.Windows.Controls;
using Application = System.Windows.Application;

namespace MapleUtility.Plugins.ViewModels.UserControls
{
    public class ViewModelUCTimerHelper : Notifier
    {
        private ObservableCollection<TimerItem> timerList;
        public ObservableCollection<TimerItem> TimerList
        {
            get { return timerList; }
            set
            {
                timerList = value;
                OnPropertyChanged("TimerList");
            }
        }

        public IEnumerable<TimerItem> PresetTimerList
        {
            get
            {
                if (SelectedPreset == null || TimerList == null)
                    return null;

                return TimerList.Where(o => o.Preset == SelectedPreset);
            }
        }

        private ObservableCollection<PresetItem> presetList;
        public ObservableCollection<PresetItem> PresetList
        {
            get { return presetList; }
            set
            {
                presetList = value;
                OnPropertyChanged("PresetList");
            }
        }

        private ObservableCollection<ImageItem> imageList;
        public ObservableCollection<ImageItem> ImageList
        {
            get { return imageList; }
            set
            {
                imageList = value;
                OnPropertyChanged("ImageList");
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

        private bool isTimerON = true;
        public bool IsTimerON
        {
            get { return isTimerON; }
            set
            {
                isTimerON = value;
                OnPropertyChanged("IsTimerON");
                OnPropertyChanged("IsTimerDisabled");

                if (!IsTimerON)
                {
                    IsTimerPaused = false;
                    IsTimerLocked = false;
                    RemoveAllRunningTimer();
                }
            }
        }

        private bool isTimerPaused;
        public bool IsTimerPaused
        {
            get { return isTimerPaused; }
            set
            {
                isTimerPaused = value;
                OnPropertyChanged("IsTimerPaused");

                PauseEvent();
            }
        }

        private bool isTimerLocked;
        public bool IsTimerLocked
        {
            get { return isTimerLocked; }
            set
            {
                isTimerLocked = value;
                OnPropertyChanged("IsTimerLocked");
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

        public bool IsTimerAllChecked
        {
            get
            {
                if (PresetTimerList == null || PresetTimerList.Count() == 0)
                    return false;
                else
                    return PresetTimerList.Where(o => o.IsChecked).Count() == PresetTimerList.Count();
            }
            set
            {
                if (PresetTimerList == null || PresetTimerList.Count() == 0)
                    return;

                foreach(var Timer in PresetTimerList)
                    Timer.IsChecked = value;

                CheckEvent();
            }
        }

        public bool IsRemoveTimerEnabled
        {
            get
            {
                if (PresetTimerList == null || PresetTimerList.Count() == 0 || PresetTimerList.Where(o => o.IsChecked).Count() == 0)
                    return false;
                else
                    return true;
            }
        }

        private PresetItem selectedPreset;
        public PresetItem SelectedPreset
        {
            get { return selectedPreset; }
            set
            {
                if(PresetTimerList != null && PresetTimerList.Count() > 0)
                    IsTimerAllChecked = false;

                selectedPreset = value;
                OnPropertyChanged("SelectedPreset");
                OnPropertyChanged("PresetTimerList");
            }
        }

        private Color remainSquareColor;
        public Color RemainSquareColor
        {
            get { return remainSquareColor; }
            set
            {
                remainSquareColor = value;
                OnPropertyChanged("RemainSquareColor");
            }
        }

        private bool isNumpadKeySync;
        public bool IsNumpadKeySync
        {
            get { return isNumpadKeySync; }
            set
            {
                isNumpadKeySync = value;
                OnPropertyChanged("IsNumpadKeySync");
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

        private float remainBackAlpha;
        public float RemainBackAlpha
        {
            get { return remainBackAlpha; }
            set
            {
                remainBackAlpha = value;
                OnPropertyChanged("RemainBackAlpha");
                OnPropertyChanged("RemainBackColor");
            }
        }

        public Color RemainBackColor
        {
            get
            {
                var alpha = Convert.ToByte(Math.Truncate(255 / 100 * RemainBackAlpha));
                return Color.FromArgb(alpha, 0, 0, 0);
            }
        }

        public Key? TimerOnOffKey = null;
        public ModifierKeys? TimerOnOffModifierKey = null;
        public Key? PauseAllKey = null;
        public ModifierKeys? PauseAllModifierKey = null;
        public Key? TimerLockKey = null;
        public ModifierKeys? TimerLockModifierKey = null;
        public bool IsOpenSettingWindow = false;
        public int AlertDuration;
        public bool IsAlertShowScreenChecked;
        public RadDesktopAlertManager AlertManager = new RadDesktopAlertManager();

        #region Button Command Variables
        public ICommand ColumnSettingCommand { get; set; }
        public ICommand AddTimerCommand { get; set; }
        public ICommand RemoveTimerCommand { get; set; }
        public ICommand RemoveRunningTimerCommand { get; set; }
        public ICommand PlayTestSoundCommand { get; set; }
        public ICommand OpenSettingCommand { get; set; }
        public ICommand CheckCommand { get; set; }
        public ICommand OpenUIBarCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SettingKeyCommand { get; set; }
        #endregion

        public ViewModelUCTimerHelper()
        {
            TimerList = new ObservableCollection<TimerItem>();
            RunningTimerList = new ObservableCollection<TimerItem>();

            ColumnSettingCommand = new RelayCommand(o => ColumnSettingEvent());
            AddTimerCommand = new RelayCommand(o => AddTimerEvent());
            RemoveTimerCommand = new RelayCommand(o => RemoveTimerEvent());
            RemoveRunningTimerCommand = new RelayCommand(o => RemoveRunningTimerEvent((TimerItem) o));
            PlayTestSoundCommand = new RelayCommand(o => PlaySound((TimerItem) o));
            OpenSettingCommand = new RelayCommand(o => OpenSettingEvent((Window) o));
            CheckCommand = new RelayCommand(o => CheckEvent());
            OpenUIBarCommand = new RelayCommand(o => OpenUIBarEvent());
            CloseCommand = new RelayCommand(o => CloseEvent((Window) o));
            SettingKeyCommand = new RelayCommand(o => SettingKeyEvent(o));
        }

        public void Initialize(SettingItem settingItem)
        {
            if (settingItem.TimerList == null)
                TimerList = new ObservableCollection<TimerItem>();
            else
                TimerList = settingItem.TimerList;

            //구버전 호환!
            foreach (var timer in TimerList)
            {
                if (!timer.AlertKey.HasValue || timer.ModifierKey.HasValue)
                    continue;

                var inputKey = timer.AlertKey.Value;

                if (inputKey == Key.LeftCtrl || inputKey == Key.RightCtrl)
                {
                    timer.AlertKey = null;
                    timer.ModifierKey = ModifierKeys.Control;
                }
                else if (inputKey == Key.LeftAlt || inputKey == Key.RightAlt)
                {
                    timer.AlertKey = null;
                    timer.ModifierKey = ModifierKeys.Alt;
                }
                else if (inputKey == Key.LeftShift || inputKey == Key.RightShift)
                {
                    timer.AlertKey = null;
                    timer.ModifierKey = ModifierKeys.Shift;
                }
                else if (inputKey == Key.LWin || inputKey == Key.RWin || inputKey == Key.KanaMode)
                    timer.AlertKey = null;
            }

            if (settingItem.SoundList == null)
                SoundList = InitialHelper.InitializeSoundList();
            else
                SoundList = settingItem.SoundList;

            if (settingItem.PresetList == null)
            {
                PresetList = new ObservableCollection<PresetItem>();
                var defaultPreset = new PresetItem()
                {
                    Name = "Default"
                };

                if (TimerList.Count() > 0)
                {
                    foreach (var timer in TimerList)
                        timer.Preset = defaultPreset;
                }

                PresetList.Add(defaultPreset);
                SelectedPreset = defaultPreset;
            }
            else
            {
                PresetList = settingItem.PresetList;
                SelectedPreset = settingItem.SelectedPreset;
            }

            if (settingItem.ImageList == null)
                ImageList = new ObservableCollection<ImageItem>();
            else
                ImageList = settingItem.ImageList;

            if (settingItem.ColumnList == null)
            {
                ColumnList = new ObservableCollection<ColumnItem>()
                {
                    new ColumnItem(1, "단축키 설정"),
                    new ColumnItem(2, "타이머 시간"),
                    new ColumnItem(3, "자동 반복"),
                    new ColumnItem(4, "시간 초기화"),
                    new ColumnItem(5, "이미지"),
                    new ColumnItem(6, "알림 사운드"),
                };
            }
            else
                ColumnList = settingItem.ColumnList;

            RemainSquareColor = settingItem.RemainSquareColor;
            RemainBackAlpha = settingItem.RemainBackAlpha;
            AlertDuration = settingItem.AlertDuration;
            IsShowUIBarTimerName = settingItem.IsShowUIBarTimerName;
            IsAlertShowScreenChecked = settingItem.IsAlertShowScreenChecked;
            TimerOnOffKey = settingItem.TimerOnOffKey;
            TimerOnOffModifierKey = settingItem.TimerOnOffModifierKey;
            PauseAllKey = settingItem.PauseAllKey;
            PauseAllModifierKey = settingItem.PauseAllModifierKey;
            TimerLockKey = settingItem.TimerLockKey;
            TimerLockModifierKey = settingItem.TimerLockModifierKey;
        }

        public void TickEvent(object sender, EventArgs e)
        {
            if (IsTimerPaused)
                return;

            foreach(var runningTimer in RunningTimerList.ToList())
            {
                if(runningTimer.EndTime <= DateTime.Now)
                {
                    if(IsAlertShowScreenChecked)
                    {
                        AlertManager.ShowAlert(new RadDesktopAlert()
                        {
                            Header = "Maple Utility",
                            Content = runningTimer.Name + "의 타이머가 완료되었습니다.",
                            ShowDuration = AlertDuration
                        });
                    }

                    PlaySound(runningTimer);

                    runningTimer.EndTime = null;
                    RunningTimerList.Remove(runningTimer);

                    if (runningTimer.IsTimerLoopChecked)
                    {
                        runningTimer.EndTime = DateTime.Now + runningTimer.TimerTime;
                        RunningTimerList.Add(runningTimer);
                    }
                }
                else
                    runningTimer.RefreshRemainTime();
            }
        }

        private void ColumnSettingEvent()
        {
            var window = new WindowTimerColumnSetting();
            var vm = window.DataContext as ViewModelTimerColumnSetting;

            vm.ColumnList = ColumnList;

            window.ShowDialog();

            ColumnList = vm.ColumnList;
        }

        private void AddTimerEvent()
        {
            var newTimer = new TimerItem();
            newTimer.Preset = SelectedPreset;
            TimerList.Add(newTimer);

            OnPropertyChanged("PresetTimerList");
            CheckEvent();
        }

        private void RemoveTimerEvent()
        {
            foreach (var timer in TimerList.Where(o => o.IsChecked).ToList())
                TimerList.Remove(timer);

            OnPropertyChanged("PresetTimerList");
            CheckEvent();
        }

        public void RemoveAllRunningTimer()
        {
            foreach (var runningTimer in RunningTimerList)
                runningTimer.EndTime = null;
            RunningTimerList.Clear();
        }

        private void RemoveRunningTimerEvent(TimerItem item)
        {
            item.EndTime = null;
            RunningTimerList.Remove(item);
        }

        private void OpenSettingEvent(Window window)
        {
            var timerSettingWindow = new WindowTimerSettingWindow();
            var timerSettingVM = timerSettingWindow.DataContext as ViewModelSettingWindow;

            timerSettingWindow.Left = window.Left + (window.ActualWidth - timerSettingWindow.Width) / 2;
            timerSettingWindow.Top = window.Top + (window.ActualHeight - timerSettingWindow.Height) / 2;

            timerSettingVM.TimerOnOffKey = TimerOnOffKey;
            timerSettingVM.TimerOnOffModifierKey = TimerOnOffModifierKey;
            timerSettingVM.PauseAllKey = PauseAllKey;
            timerSettingVM.PauseAllModifierKey = PauseAllModifierKey;
            timerSettingVM.TimerLockKey = TimerLockKey;
            timerSettingVM.TimerLockModifierKey = TimerLockModifierKey;
            timerSettingVM.SoundList = SoundList;
            timerSettingVM.PresetList = PresetList;
            timerSettingVM.ImageList = ImageList;
            timerSettingVM.TimerList = TimerList;
            timerSettingVM.CurrentPreset = SelectedPreset;
            timerSettingVM.RemainSquareResultColor = RemainSquareColor;
            timerSettingVM.RemainBackAlpha = RemainBackAlpha;
            timerSettingVM.AlertDuration = AlertDuration;
            timerSettingVM.IsShowUIBarTimerName = IsShowUIBarTimerName;
            timerSettingVM.IsAlertShowScreenChecked = IsAlertShowScreenChecked;

            IsOpenSettingWindow = true;
            timerSettingWindow.ShowDialog();
            IsOpenSettingWindow = false;

            SoundList = timerSettingVM.SoundList;
            TimerOnOffKey = timerSettingVM.TimerOnOffKey;
            TimerOnOffModifierKey = timerSettingVM.TimerOnOffModifierKey;
            PauseAllKey = timerSettingVM.PauseAllKey;
            PauseAllModifierKey = timerSettingVM.PauseAllModifierKey;
            TimerLockKey = timerSettingVM.TimerLockKey;
            TimerLockModifierKey = timerSettingVM.TimerLockModifierKey;
            PresetList = timerSettingVM.PresetList;
            ImageList = timerSettingVM.ImageList;
            RemainSquareColor = timerSettingVM.RemainSquareResultColor;
            RemainBackAlpha = timerSettingVM.RemainBackAlpha;
            AlertDuration = timerSettingVM.AlertDuration;
            IsShowUIBarTimerName = timerSettingVM.IsShowUIBarTimerName;
            IsAlertShowScreenChecked = timerSettingVM.IsAlertShowScreenChecked;
        }

        public void KeyDownEvent(ModifierKeys modifierKeys, Key inputKey)
        {
            if (IsOpenSettingWindow)
                return;

            CheckTimerKey(modifierKeys, inputKey);
        }

        private void CheckTimerKey(ModifierKeys modifierKeys, Key inputKey)
        {
            if(!IsTimerLocked)
            {
                if (!(PauseAllModifierKey == null && PauseAllKey == null))
                {
                    if (KeyInputHelper.CheckPressModifierAndNormalKey(modifierKeys, inputKey, PauseAllModifierKey, PauseAllKey))
                        IsTimerPaused = !IsTimerPaused;
                }
            }

            if(!IsTimerPaused)
            {
                if (!(TimerLockKey == null && TimerLockModifierKey == null))
                {
                    if (KeyInputHelper.CheckPressModifierAndNormalKey(modifierKeys, inputKey, TimerLockModifierKey, TimerLockKey))
                        IsTimerLocked = !IsTimerLocked;
                }
            }

            if (!(TimerOnOffModifierKey == null && TimerOnOffKey == null))
            {
                if (KeyInputHelper.CheckPressModifierAndNormalKey(modifierKeys, inputKey, TimerOnOffModifierKey, TimerOnOffKey))
                    IsTimerON = !IsTimerON;
            }

            if (!IsTimerON || IsTimerPaused || IsTimerLocked)
                return;

            foreach (var timer in PresetTimerList)
            {
                if ((!timer.ModifierKey.HasValue && !timer.AlertKey.HasValue) || !timer.TimerTime.HasValue || timer.TimerTime.Value.TotalSeconds < 1)
                    continue;

                if (!KeyInputHelper.CheckPressModifierAndNormalKey(modifierKeys, inputKey, timer.ModifierKey, timer.AlertKey))
                    continue;

                if (timer.SoundItem != null)
                {
                    if (!timer.SoundItem.IsInternalSound && !File.Exists(timer.SoundItem.Path))
                        continue;
                }

                if (timer.EndTime != null && timer.EndTime > DateTime.Now)
                {
                    if (!timer.IsTimerResetTimeChecked)
                        continue;

                    timer.EndTime = DateTime.Now + timer.TimerTime;
                }
                else
                {
                    timer.EndTime = DateTime.Now + timer.TimerTime;
                    RunningTimerList.Add(timer);
                }
                DebugLogHelper.Write(timer.Name + " 타이머 작동되었습니다.");
            }
        }

        private void PauseEvent()
        {
            // 일시정지 된 상태이므로, 해제
            if(!IsTimerPaused)
            {
                foreach(var runningTimer in RunningTimerList)
                {
                    runningTimer.EndTime += DateTime.Now - runningTimer.PauseTime;
                    runningTimer.PauseTime = null;
                }
            }
            else // 일시정지 설정
            {
                foreach (var runningTimer in RunningTimerList)
                    runningTimer.PauseTime = DateTime.Now;
            }
        }

        private void OpenUIBarEvent()
        {
            var window = WindowTimerUIBar.Instance;
            window.DataContext = this;

            window.Show();
        }

        private void CloseEvent(Window window)
        {
            window.Close();
        }

        private void SettingKeyEvent(object parameter)
        {
            var values = (object[])parameter;
            var window = values[0] as Window;
            var item = values[1] as TimerItem;

            var dialog = new WindowTimerPressKeyboard();
            var vm = dialog.DataContext as ViewModelTimerPressKeyboard;

            dialog.Left = window.Left + (window.ActualWidth - dialog.Width) / 2;
            dialog.Top = window.Top + (window.ActualHeight - dialog.Height) / 2;

            vm.PressedKey = item.AlertKey;
            vm.ModifierKey = item.ModifierKey;
            vm.ChangeKeyText();

            IsOpenSettingWindow = true;
            dialog.ShowDialog();
            IsOpenSettingWindow = false;

            item.AlertKey = vm.PressedKey;
            item.ModifierKey = vm.ModifierKey;
        }

        private void PlaySound(TimerItem item)
        {
            try
            {
                var soundItem = item.SoundItem;
                if (soundItem == null || soundItem.Path == null)
                    return;

                if (item.PrevWavePlayer != null)
                {
                    item.PrevWavePlayer.Stop();
                    item.PrevWavePlayer = null;
                }

                var wavePlayer = new WaveOut();

                WaveStream waveStream;

                if (soundItem.IsInternalSound)
                {
                    StreamResourceInfo resource = Application.GetResourceStream(new Uri("MapleUtility;component/Plugins/Sounds/" + soundItem.Path, UriKind.Relative));
                    waveStream = new Mp3FileReader(resource.Stream);
                }
                else
                    waveStream = new MediaFoundationReader(soundItem.Path);

                WaveChannel32 inputStream = new WaveChannel32(waveStream);
                inputStream.PadWithZeroes = false;

                wavePlayer.Volume = item.Volume / 100;
                wavePlayer.Init(inputStream);
                wavePlayer.Play();

                wavePlayer.PlaybackStopped += delegate (object sender, StoppedEventArgs e)
                {
                    wavePlayer.Dispose();
                    waveStream.Dispose();
                };

                item.PrevWavePlayer = wavePlayer;
            }
            catch(Exception)
            {
                DebugLogHelper.Write(item.Name + " 타이머 사운드 재생 중 오류가 발생했습니다.");
            }
        }

        private void CheckEvent()
        {
            OnPropertyChanged("IsTimerAllChecked");
            OnPropertyChanged("IsRemoveTimerEnabled");
        }
    }
}
