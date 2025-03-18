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
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;

namespace MapleUtility.Plugins.ViewModels.UserControls
{
    public class ViewModelUCVerusHillaHelper : Notifier, IViewModelItemAvailable
    {
        public IWavePlayer PrevWavePlayer;

        private List<TimerKeyItem> keyItems = new List<TimerKeyItem>();
        public List<TimerKeyItem> KeyItems
        {
            get { return keyItems; }
            set
            {
                keyItems = value;
                OnPropertyChanged("KeyItems");
            }
        }

        private int currentPhase;
        public int CurrentPhase
        {
            get { return currentPhase; }
            set
            {
                currentPhase = value;
                OnPropertyChanged("CurrentPhase");
                OnPropertyChanged("NextPatternTime");
                OnPropertyChanged("RemainTime");
            }
        }

        private TimeSpan? realPatternTime;
        public TimeSpan? RealPatternTime
        {
            get { return realPatternTime; }
            set
            {
                realPatternTime = value;
                OnPropertyChanged("RealPatternTime");
            }
        }

        private TimeSpan? latestPatternTime;
        public TimeSpan? LatestPatternTime
        {
            get { return latestPatternTime; }
            set
            {
                latestPatternTime = value;
                OnPropertyChanged("LatestPatternTime");
                OnPropertyChanged("NextPatternTime");
                OnPropertyChanged("RemainTime");
            }
        }

        private DateTime? InternalLatestPatternTime = null;

        public TimeSpan? NextPatternTime
        {
            get
            {
                if (!LatestPatternTime.HasValue)
                    return null;

                return LatestPatternTime - PhasePatternTime;
            }
        }

        public TimeSpan PhasePatternTime
        {
            get
            {
                switch (CurrentPhase)
                {
                    case 1:
                        return new TimeSpan(0, 2, 30);
                    case 2:
                        return new TimeSpan(0, 2, 5);
                    default:
                        return new TimeSpan(0, 1, 40);
                }
            }
        }

        public TimeSpan? RemainTime
        {
            get
            {
                if (!LatestPatternTime.HasValue)
                    return null;

                var remainTime = PhasePatternTime + (InternalLatestPatternTime - DateTime.Now);

                return remainTime;
            }
        }

        public string RemainTimeSeconds
        {
            get
            {
                if (!RemainTime.HasValue)
                    return "";

                var totalSeconds = RemainTime.Value.TotalSeconds;

                if (totalSeconds > 1.0f)
                    return totalSeconds.ToString("F0");
                else
                    return totalSeconds.ToString("F1");
            }
        }

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

        private int? beforeSoundTime = 0;
        public int? BeforeSoundTime
        {
            get { return beforeSoundTime; }
            set
            {
                beforeSoundTime = value;
                OnPropertyChanged("BeforeSoundTime");
            }
        }

        private SoundItem beforeSoundItem;
        public SoundItem BeforeSoundItem
        {
            get { return beforeSoundItem; }
            set
            {
                beforeSoundItem = value;
                OnPropertyChanged("BeforeSoundItem");
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

        private float volume = 100;
        public float Volume
        {
            get { return volume; }
            set
            {
                int intValue = Convert.ToInt32(value);

                if (intValue > 100)
                    intValue = 100;
                if (intValue < 0)
                    intValue = 0;

                volume = intValue;
                OnPropertyChanged("Volume");
            }
        }

        public TimerKeyItem SubtractTimeKey
        {
            get { return KeyItems.FirstOrDefault(o => o.Name == "SubtractTimeKey"); }
        }

        public TimerKeyItem BackKey
        {
            get { return KeyItems.FirstOrDefault(o => o.Name == "BackKey"); }
        }

        public TimerKeyItem ScytheKey
        {
            get { return KeyItems.FirstOrDefault(o => o.Name == "ScytheKey"); }
        }

        public TimerKeyItem NextKey
        {
            get { return KeyItems.FirstOrDefault(o => o.Name == "NextKey"); }
        }

        public TimerKeyItem AddTimeKey
        {
            get { return KeyItems.FirstOrDefault(o => o.Name == "AddTimeKey"); }
        }

        public bool IsAlertBeforeTimer { get; set; } = false;
        public bool IsOpenSettingWindow { get; set; } = false;
        public List<Key> PressedKeyList { get; set; } = new List<Key>();

        #region Button Command Variables
        public ICommand SubtractTimeKeyCommand { get; set; }
        public ICommand BackKeyCommand { get; set; }
        public ICommand ScytheKeyCommand { get; set; }
        public ICommand NextKeyCommand { get; set; }
        public ICommand AddTimeKeyCommand { get; set; }
        public ICommand SubtractTimeKeySettingCommand { get; set; }
        public ICommand BackKeySettingCommand { get; set; }
        public ICommand ScytheKeySettingCommand { get; set; }
        public ICommand NextKeySettingCommand { get; set; }
        public ICommand AddTimeKeySettingCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand OpenHillaUIBarCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion

        public ViewModelUCVerusHillaHelper()
        {
            SubtractTimeKeyCommand = new RelayCommand(o => ChangeTime(-5));
            BackKeyCommand = new RelayCommand(o => BackKeyEvent());
            ScytheKeyCommand = new RelayCommand(o => ScytheKeyEvent());
            NextKeyCommand = new RelayCommand(o => NextKeyEvent());
            AddTimeKeyCommand = new RelayCommand(o => ChangeTime(5));
            ResetCommand = new RelayCommand(o => ResetEvent());
            OpenHillaUIBarCommand = new RelayCommand(o => OpenHillaUIBarEvent());
            CloseCommand = new RelayCommand(o => CloseEvent((Window)o));

            KeyItems.Add(new TimerKeyItem("SubtractTimeKey", ChangeTime, -5));
            KeyItems.Add(new TimerKeyItem("BackKey", BackKeyEvent));
            KeyItems.Add(new TimerKeyItem("ScytheKey", ScytheKeyEvent));
            KeyItems.Add(new TimerKeyItem("NextKey", NextKeyEvent));
            KeyItems.Add(new TimerKeyItem("AddTimeKey", ChangeTime, 5));

            CurrentPhase = 1;
            LatestPatternTime = null;
            InternalLatestPatternTime = null;
            IsHelperON = false;
        }

        public void Initialize(SettingItem settingItem)
        {
            UIBarTop = settingItem.HILLA_UIBAR_TOP;
            UIBarLeft = settingItem.HILLA_UIBAR_LEFT;
            UIBarWidth = settingItem.HILLA_UIBAR_WIDTH;
            UIBarHeight = settingItem.HILLA_UIBAR_HEIGHT;
            UIBarTransparency = settingItem.HILLA_UIBAR_TRANSPARENCY;
            Volume = settingItem.HILLA_VOLUME;

            if (settingItem.HillaTimer_KeyItems != null)
            {
                foreach (var keyItem in settingItem.HillaTimer_KeyItems)
                {
                    var matchKeyItem = KeyItems.FirstOrDefault(o => o.Name == keyItem.Name);
                    if (matchKeyItem == null)
                        continue;

                    matchKeyItem.CopyItem(keyItem);
                }
            }
            else // 이전 데이터 호환
            {
                var timerPausedKey = KeyItems.FirstOrDefault(o => o.Name == "BackKey");
                timerPausedKey.Key = settingItem.BackKey;
                timerPausedKey.ModifierKey = settingItem.BackModifierKey;

                var timerLockedKey = KeyItems.FirstOrDefault(o => o.Name == "ScytheKey");
                timerLockedKey.Key = settingItem.ScytheKey;
                timerLockedKey.ModifierKey = settingItem.ScytheModifierKey;

                var timerOnOffKey = KeyItems.FirstOrDefault(o => o.Name == "NextKey");
                timerPausedKey.Key = settingItem.NextKey;
                timerPausedKey.ModifierKey = settingItem.NextModifierKey;
            }
        }

        private void BackKeyEvent()
        {
            if (CurrentPhase > 1)
                CurrentPhase--;
        }

        private void PlaySound(SoundItem item)
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

                wavePlayer.Volume = Volume / 100;
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

        private void ChangeTime(int seconds)
        {
            if (seconds >= 0)
                IsAlertBeforeTimer = false;

            LatestPatternTime = LatestPatternTime.Value.Add(new TimeSpan(0, 0, -seconds));
            InternalLatestPatternTime = InternalLatestPatternTime.Value.Add(new TimeSpan(0, 0, seconds));
        }

        private void ScytheKeyEvent()
        {
            if (LatestPatternTime == null)
            {
                RealPatternTime = LatestPatternTime = new TimeSpan(0, 27, 13);
                InternalLatestPatternTime = DateTime.Now;
            }
            else
            {
                RealPatternTime = LatestPatternTime = LatestPatternTime + (InternalLatestPatternTime - DateTime.Now);
                if (LatestPatternTime.Value.TotalSeconds < 0)
                    ResetEvent();
                else
                    InternalLatestPatternTime = DateTime.Now;
            }

            IsAlertBeforeTimer = false;
        }

        private void NextKeyEvent()
        {
            if (CurrentPhase < 3)
                CurrentPhase++;
        }

        private void ResetEvent()
        {
            CurrentPhase = 1;
            LatestPatternTime = null;
            InternalLatestPatternTime = null;
        }

        public void OpenHillaUIBarEvent()
        {
            var window = WindowVerusHillaUIBar.Instance as WindowVerusHillaUIBar;
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
            OnPropertyChanged("RemainTime");
            OnPropertyChanged("RemainTimeSeconds");

            if(RemainTime != null)
            {
                if (BeforeSoundTime != null)
                {
                    if (!IsAlertBeforeTimer && RemainTime.Value.TotalSeconds <= BeforeSoundTime)
                    {
                        IsAlertBeforeTimer = true;
                        PlaySound(BeforeSoundItem);
                    }
                }

                if (RemainTime.Value.TotalSeconds <= 0)
                    ScytheKeyEvent();
            }
        }

        public void KeyEvent(ModifierKeys modifierKeys, Key inputKey, GlobalKeyboardHookHelper.KeyboardState keyboardState)
        {
            if (IsOpenSettingWindow)
                return;

            CheckHillaKey(modifierKeys, inputKey, keyboardState);
        }

        private void CheckHillaKey(ModifierKeys modifierKeys, Key inputKey, GlobalKeyboardHookHelper.KeyboardState keyboardState)
        {
            if (!IsHelperON)
                return;

            var isKeyupEvent = keyboardState == GlobalKeyboardHookHelper.KeyboardState.KeyUp;

            foreach (var keyItem in KeyItems.Where(o => o.IsKeyupEvent == isKeyupEvent))
            {
                if (!(keyItem.ModifierKey == null && keyItem.Key == null))
                {
                    if (KeyInputHelper.CheckPressModifierAndNormalKey(modifierKeys, inputKey, keyItem.ModifierKey, keyItem.Key))
                        keyItem.KeyCommand.Execute(true);
                }
            }
        }

        public void ItemCheckEvent()
        {
            if (BeforeSoundItem?.IsDisposed == true)
                BeforeSoundItem = null;
        }
    }
}
