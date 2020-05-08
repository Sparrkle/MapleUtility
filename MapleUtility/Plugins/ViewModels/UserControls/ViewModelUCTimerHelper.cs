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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using Telerik.Windows.Controls;

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

        public bool IsRemoveTimerEnabeld
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
        public bool IsOpenSettingWindow = false;
        public int AlertDuration;
        public bool IsAlertShowScreenChecked;
        public RadDesktopAlertManager AlertManager = new RadDesktopAlertManager();

        #region Button Command Variables
        public ICommand AddTimerCommand { get; set; }
        public ICommand RemoveTimerCommand { get; set; }
        public ICommand RemoveRunningTimerCommand { get; set; }
        public ICommand PlayTestSoundCommand { get; set; }
        public ICommand OpenSettingCommand { get; set; }
        public ICommand CheckCommand { get; set; }
        public ICommand OpenUIBarCommand { get; set; }
        public ICommand CloseUIBarCommand { get; set; }
        public ICommand SettingKeyCommand { get; set; }
        #endregion

        public List<Key> PressedKeyList = new List<Key>();

        public ViewModelUCTimerHelper()
        {
            TimerList = new ObservableCollection<TimerItem>();
            RunningTimerList = new ObservableCollection<TimerItem>();

            AddTimerCommand = new RelayCommand(o => AddTimerEvent());
            RemoveTimerCommand = new RelayCommand(o => RemoveTimerEvent());
            RemoveRunningTimerCommand = new RelayCommand(o => RemoveRunningTimerEvent((TimerItem) o));
            PlayTestSoundCommand = new RelayCommand(o => PlaySound((TimerItem) o));
            OpenSettingCommand = new RelayCommand(o => OpenSettingEvent((Window) o));
            CheckCommand = new RelayCommand(o => CheckEvent());
            OpenUIBarCommand = new RelayCommand(o => OpenUIBarEvent());
            CloseUIBarCommand = new RelayCommand(o => CloseUIBarEvent((Window) o));
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

            RemainSquareColor = settingItem.RemainSquareColor;
            RemainBackAlpha = settingItem.RemainBackAlpha;
            AlertDuration = settingItem.AlertDuration;
            IsShowUIBarTimerName = settingItem.IsShowUIBarTimerName;
            IsAlertShowScreenChecked = settingItem.IsAlertShowScreenChecked;
            TimerOnOffKey = settingItem.TimerOnOffKey;
            TimerOnOffModifierKey = settingItem.TimerOnOffModifierKey;
            PauseAllKey = settingItem.PauseAllKey;
            PauseAllModifierKey = settingItem.PauseAllModifierKey;
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
                }
                else
                    runningTimer.RefreshRemainTime();
            }
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
            PresetList = timerSettingVM.PresetList;
            ImageList = timerSettingVM.ImageList;
            RemainSquareColor = timerSettingVM.RemainSquareResultColor;
            RemainBackAlpha = timerSettingVM.RemainBackAlpha;
            AlertDuration = timerSettingVM.AlertDuration;
            IsShowUIBarTimerName = timerSettingVM.IsShowUIBarTimerName;
            IsAlertShowScreenChecked = timerSettingVM.IsAlertShowScreenChecked;
        }

        public void KeyDownEvent(GlobalKeyboardHookHelperEventArgs args)
        {
            if (IsOpenSettingWindow)
                return;

            var inputKey = KeyInterop.KeyFromVirtualKey(args.KeyboardData.VirtualCode);

            if (!PressedKeyList.Any(o => o == inputKey))
                PressedKeyList.Add(inputKey);

            CheckTimerKey();
        }

        public void KeyUpEvent(GlobalKeyboardHookHelperEventArgs args)
        {
            if (IsOpenSettingWindow)
                return;

            var inputKey = KeyInterop.KeyFromVirtualKey(args.KeyboardData.VirtualCode);

            if (PressedKeyList.Any(o => o == inputKey))
                PressedKeyList.Remove(inputKey);
        }

        private void CheckTimerKey()
        {
            if (PauseAllModifierKey != null && PauseAllKey != null)
            {
                if (CheckPressModifierAndNormalKey(PauseAllModifierKey, PauseAllKey))
                    IsTimerPaused = !IsTimerPaused;
            }

            if (TimerOnOffModifierKey != null && TimerOnOffKey != null)
            {
                if (CheckPressModifierAndNormalKey(TimerOnOffModifierKey, TimerOnOffKey))
                    IsTimerON = !IsTimerON;
            }

            if (!IsTimerON || IsTimerPaused)
                return;

            foreach (var timer in PresetTimerList)
            {
                if ((!timer.ModifierKey.HasValue && !timer.AlertKey.HasValue) || !timer.TimerTime.HasValue)
                    continue;

                if (!CheckPressModifierAndNormalKey(timer.ModifierKey, timer.AlertKey))
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

        private bool CheckPressModifierAndNormalKey(ModifierKeys? modifierKeys, Key? pressKey)
        {
            if(modifierKeys.HasValue)
            {
                if(modifierKeys.Value.HasFlag(ModifierKeys.Control))
                {
                    if (!PressedKeyList.Any(o => o == Key.LeftCtrl || o == Key.RightCtrl))
                        return false;
                }
                if (modifierKeys.Value.HasFlag(ModifierKeys.Alt))
                {
                    if (!PressedKeyList.Any(o => o == Key.LeftAlt || o == Key.RightAlt))
                        return false;
                }
                if (modifierKeys.Value.HasFlag(ModifierKeys.Shift))
                {
                    if (!PressedKeyList.Any(o => o == Key.LeftShift || o == Key.RightShift))
                        return false;
                }
            }

            if(pressKey.HasValue)
            {
                if (PressedKeyList.Any(o => o == pressKey))
                    return true;
                else
                    return false;
            }

            return true;
        }

        private void OpenUIBarEvent()
        {
            var window = WindowTimerUIBar.Instance;
            window.DataContext = this;

            window.Show();
        }

        private void CloseUIBarEvent(Window window)
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
                if (soundItem == null)
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
            OnPropertyChanged("IsRemoveTimerEnabeld");
        }
    }
}
