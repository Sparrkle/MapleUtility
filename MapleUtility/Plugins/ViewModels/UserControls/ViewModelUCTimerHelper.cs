﻿using MapleUtility.Plugin.Lib;
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

        private bool isTimerON = true;
        public bool IsTimerON
        {
            get { return isTimerON; }
            set
            {
                isTimerON = value;
                OnPropertyChanged("IsTimerON");

                if (!IsTimerON)
                {
                    RemoveAllRunningTimer();
                }
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

        public Key? TimerOnOffKey = null;
        public ModifierKeys? TimerOnOffModifierKey = null;
        public bool IsTimerResetChecked = false;
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

                if (inputKey.HasFlag(Key.LeftCtrl) || inputKey.HasFlag(Key.RightCtrl))
                {
                    timer.AlertKey = null;
                    timer.ModifierKey = ModifierKeys.Control;
                }
                else if (inputKey.HasFlag(Key.LeftAlt) || inputKey.HasFlag(Key.RightAlt))
                {
                    timer.AlertKey = null;
                    timer.ModifierKey = ModifierKeys.Alt;
                }
                else if (inputKey.HasFlag(Key.LeftShift) || inputKey.HasFlag(Key.RightShift))
                {
                    timer.AlertKey = null;
                    timer.ModifierKey = ModifierKeys.Shift;
                }
                else if (inputKey.HasFlag(Key.LWin) || inputKey.HasFlag(Key.RWin) || inputKey.HasFlag(Key.KanaMode))
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

            AlertDuration = settingItem.AlertDuration;
            IsAlertShowScreenChecked = settingItem.IsAlertShowScreenChecked;
            IsTimerResetChecked = settingItem.IsTimerResetChecked;
            TimerOnOffKey = settingItem.TimerOnOffKey;
            TimerOnOffModifierKey = settingItem.TimerOnOffModifierKey;
        }

        public void TickEvent(object sender, EventArgs e)
        {
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
            timerSettingVM.IsTimerResetChecked = IsTimerResetChecked;
            timerSettingVM.SoundList = SoundList;
            timerSettingVM.PresetList = PresetList;
            timerSettingVM.TimerList = TimerList;
            timerSettingVM.CurrentPreset = SelectedPreset;
            timerSettingVM.AlertDuration = AlertDuration;
            timerSettingVM.IsAlertShowScreenChecked = IsAlertShowScreenChecked;

            IsOpenSettingWindow = true;
            timerSettingWindow.ShowDialog();
            IsOpenSettingWindow = false;

            SoundList = timerSettingVM.SoundList;
            IsTimerResetChecked = timerSettingVM.IsTimerResetChecked;
            TimerOnOffKey = timerSettingVM.TimerOnOffKey;
            TimerOnOffModifierKey = timerSettingVM.TimerOnOffModifierKey;
            PresetList = timerSettingVM.PresetList;
            AlertDuration = timerSettingVM.AlertDuration;
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
            if(TimerOnOffModifierKey != null && TimerOnOffKey != null)
            {
                if (CheckPressModifierAndNormalKey(TimerOnOffModifierKey, TimerOnOffKey))
                    IsTimerON = !IsTimerON;
            }

            if (!IsTimerON)
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
                    if (!IsTimerResetChecked)
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

            var vm = window.DataContext as ViewModelTimerUIBar;
            vm.RunningTimerList = RunningTimerList;

            window.Show();
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

                wavePlayer.Volume = item.TimerVolume / 100;
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
