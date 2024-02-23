using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Lib;
using MapleUtility.Plugins.Models;
using MapleUtility.Plugins.ViewModels.Views.Timer;
using MapleUtility.Plugins.Views.Windows.Timer;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;

namespace MapleUtility.Plugins.ViewModels.UserControls
{
    public class ViewModelUCKalosHelper : Notifier
    {
        public IWavePlayer PrevWavePlayer;

        #region 즉사 관련
        private List<TimerKeyItem> instantKeyItems = new List<TimerKeyItem>(7);
        public List<TimerKeyItem> InstanceKeyItems
        {
            get { return instantKeyItems; }
            set
            {
                instantKeyItems = value;
                OnPropertyChanged("InstantKeyItems");
            }
        }

        private DateTime? nextInstantPatternTime;
        public DateTime? NextInstantPatternTime
        {
            get { return nextInstantPatternTime; }
            set
            {
                nextInstantPatternTime = value;
                OnPropertyChanged("NextInstantPatternTime");
                OnPropertyChanged("RemainInstantTime");
            }
        }

        public TimeSpan? RemainInstantTime
        {
            get
            {
                if (!NextInstantPatternTime.HasValue)
                    return null;

                var remainTime = NextInstantPatternTime - DateTime.Now;

                return remainTime;
            }
        }

        public string RemainInstantTimeSeconds
        {
            get
            {
                if (!RemainInstantTime.HasValue)
                    return "";

                var totalSeconds = RemainInstantTime.Value.TotalSeconds;

                if (totalSeconds > 1.0f)
                    return totalSeconds.ToString("F0");
                else
                    return totalSeconds.ToString("F1");
            }
        }

        private int? beforeInstantSoundTime = 0;
        public int? BeforeInstantSoundTime
        {
            get { return beforeInstantSoundTime; }
            set
            {
                beforeInstantSoundTime = value;
                OnPropertyChanged("BeforeInstantSoundTime");
            }
        }

        private SoundItem beforeInstantSoundItem;
        public SoundItem BeforeInstantSoundItem
        {
            get { return beforeInstantSoundItem; }
            set
            {
                beforeInstantSoundItem = value;
                OnPropertyChanged("BeforeInstantSoundItem");
            }
        }

        private float instantVolume = 100;
        public float InstantVolume
        {
            get { return instantVolume; }
            set
            {
                int intValue = Convert.ToInt32(value);

                if (intValue > 100)
                    intValue = 100;
                if (intValue < 0)
                    intValue = 0;

                instantVolume = intValue;
                OnPropertyChanged("InstantVolume");
            }
        }
        #endregion

        private bool isHelperON;
        public bool IsHelperON
        {
            get { return isHelperON; }
            set
            {
                isHelperON = value;
                OnPropertyChanged("IsHelperON");

                if (!value)
                    ResetEvent();
            }
        }

        private int uiBarTop;
        public int UIBarTop
        {
            get { return uiBarTop; }
            set
            {
                uiBarTop = value;
                OnPropertyChanged("UIBarTop");
            }
        }

        private int uiBarLeft;
        public int UIBarLeft
        {
            get { return uiBarLeft; }
            set
            {
                uiBarLeft = value;
                OnPropertyChanged("UIBarLeft");
            }
        }

        private float uiBarTransparency = 19;
        public float UIBarTransparency
        {
            get { return uiBarTransparency; }
            set
            {
                int intValue = Convert.ToInt32(value);

                if (intValue > 100)
                    intValue = 100;
                if (intValue < 1)
                    intValue = 1;

                uiBarTransparency = intValue;
                OnPropertyChanged("UIBarTransparency");
                OnPropertyChanged("UIBarBackground");
            }
        }

        public Color UIBarBackground
        {
            get
            {
                var alpha = Convert.ToByte(Math.Truncate(255 / 100 * UIBarTransparency));
                return Color.FromArgb(alpha, 39, 20, 30);
            }
        }

        private int uiBarWidth;
        public int UIBarWidth
        {
            get { return uiBarWidth; }
            set
            {
                uiBarWidth = value;
                OnPropertyChanged("UIBarWidth");
            }
        }

        private int uiBarHeight;
        public int UIBarHeight
        {
            get { return uiBarHeight; }
            set
            {
                uiBarHeight = value;
                OnPropertyChanged("UIBarHeight");
            }
        }

        public bool IsAlertInstantBeforeTimer { get; set; } = false;
        public bool IsOpenSettingWindow { get; set; } = false;

        #region Button Command Variables
        public ICommand ResetCommand { get; set; }
        public ICommand OpenKalosUIBarCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion

        public ViewModelUCKalosHelper()
        {
            ResetCommand = new RelayCommand(o => ResetEvent());
            OpenKalosUIBarCommand = new RelayCommand(o => OpenKalosUIBarEvent());
            CloseCommand = new RelayCommand(o => CloseEvent((Window)o));

            #region 즉사 Key Setting
            instantKeyItems.Add(new TimerKeyItem(InstantPressKeyEvent));

            for (int i=1; i<=3; i++)
                instantKeyItems.Add(new TimerKeyItem(InstantTimerPressKeyEvent, false));

            for (int i = 1; i <= 3; i++)
                instantKeyItems.Add(new TimerKeyItem(InstantTimerPressKeyEvent, true));
            #endregion

            IsHelperON = false;
        }

        public void InstantPressKeyEvent()
        {
            NextInstantPatternTime = DateTime.Now + new TimeSpan(0, 2, 30);
            IsAlertInstantBeforeTimer = false;
        }

        public void InstantTimerPressKeyEvent(float time)
        {
            NextInstantPatternTime = NextInstantPatternTime + TimeSpan.FromSeconds(time);
        }

        public void Initialize(SettingItem settingItem)
        {
            UIBarTop = settingItem.KALOS_UIBAR_TOP;
            UIBarLeft = settingItem.KALOS_UIBAR_LEFT;
            UIBarWidth = settingItem.KALOS_UIBAR_WIDTH;
            UIBarHeight = settingItem.KALOS_UIBAR_HEIGHT;
            UIBarTransparency = settingItem.KALOS_UIBAR_TRANSPARENCY;
            
            if(settingItem.KALOS_InstanceKeyItems != null)
            {
                for(int i=0; i<settingItem.KALOS_InstanceKeyItems.Count; i++)
                {
                    InstanceKeyItems[i].Time = settingItem.KALOS_InstanceKeyItems[i].Time;
                    InstanceKeyItems[i].ModifierKey = settingItem.KALOS_InstanceKeyItems[i].ModifierKey;
                    InstanceKeyItems[i].Key = settingItem.KALOS_InstanceKeyItems[i].Key;
                }
            }
            else
            {
                //초기화
                InstanceKeyItems[1].Time = 10;
                InstanceKeyItems[2].Time = 35;
                InstanceKeyItems[3].Time = 50;
                InstanceKeyItems[4].Time = 10;
                InstanceKeyItems[5].Time = 35;
                InstanceKeyItems[6].Time = 50;
            }

            InstantVolume = settingItem.KALOS_INSTANT_VOLUME;
        }

        private void PlaySound(SoundItem item, float volume)
        {
            try
            {
                if (item == null || item.Path == null)
                    return;

                if (PrevWavePlayer != null)
                {
                    PrevWavePlayer.Stop();
                    PrevWavePlayer.Dispose();
                    PrevWavePlayer = null;
                }

                var wavePlayer = new WaveOut();

                WaveStream waveStream;

                if (item.IsInternalSound)
                {
                    StreamResourceInfo resource = Application.GetResourceStream(new Uri("MapleUtility;component/Plugins/Sounds/" + item.Path, UriKind.Relative));
                    waveStream = new Mp3FileReader(resource.Stream);
                }
                else
                    waveStream = new MediaFoundationReader(item.Path);

                WaveChannel32 inputStream = new WaveChannel32(waveStream);
                inputStream.PadWithZeroes = false;

                wavePlayer.Volume = volume / 100;
                wavePlayer.Init(inputStream);
                wavePlayer.Play();

                wavePlayer.PlaybackStopped += delegate (object sender, StoppedEventArgs e)
                {
                    wavePlayer.Dispose();
                    waveStream.Dispose();
                };

                PrevWavePlayer = wavePlayer;
            }
            catch (Exception)
            {
                DebugLogHelper.Write(item.Name + " 타이머 사운드 재생 중 오류가 발생했습니다.");
            }
        }

        private void ResetEvent()
        {
            NextInstantPatternTime = null;
        }

        private void OpenKalosUIBarEvent()
        {
            var window = WindowKalosUIBar.Instance;
            window.DataContext = this;

            if (window.IsVisible)
                window.Hide();
            else
            {
                window.Show();

                var screen = window.GetCurrentScreenWorkArea();

                if (window.Left + window.Width / 2 < 0)
                    window.Left = 0;
                else if (window.Left + window.Width / 2 > screen.Width)
                    window.Left = screen.Width - window.Width;

                if (window.Top + window.Height / 2 < 0)
                    window.Top = 0;
                else if (window.Top + window.Height / 2 > screen.Height)
                    window.Top = screen.Height - window.Height;
            }
        }

        private void CloseEvent(Window window)
        {
            window.Close();
        }

        public void TickEvent(object sender, EventArgs e)
        {
            OnPropertyChanged("RemainInstantTime");
            OnPropertyChanged("RemainInstantTimeSeconds");

            if(RemainInstantTime != null)
            {
                if (BeforeInstantSoundTime != null)
                {
                    if (!IsAlertInstantBeforeTimer && RemainInstantTime.Value.TotalSeconds <= BeforeInstantSoundTime)
                    {
                        IsAlertInstantBeforeTimer = true;
                        PlaySound(BeforeInstantSoundItem, InstantVolume);
                    }
                }

                if (RemainInstantTime.Value.TotalSeconds <= 0)
                    InstantPressKeyEvent();
            }
        }

        public void KeyDownEvent(ModifierKeys modifierKeys, Key inputKey)
        {
            if (IsOpenSettingWindow)
                return;

            CheckKalosKey(modifierKeys, inputKey);
        }

        private void CheckKalosKey(ModifierKeys modifierKeys, Key inputKey)
        {
            if (!IsHelperON)
                return;

            foreach (var keyItem in instantKeyItems)
            {
                if (!(keyItem.ModifierKey == null && keyItem.Key == null))
                {
                    if (KeyInputHelper.CheckPressModifierAndNormalKey(modifierKeys, inputKey, keyItem.ModifierKey, keyItem.Key))
                        keyItem.KeyCommand.Execute(true);
                }
            }
        }
    }
}
