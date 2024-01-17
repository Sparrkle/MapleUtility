using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Helpers;
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
    public class ViewModelUCVerusHillaHelper : Notifier
    {
        public IWavePlayer PrevWavePlayer;
        private ModifierKeys? subtractTimeModifierKey = null;
        public ModifierKeys? SubtractTimeModifierKey
        {
            get { return subtractTimeModifierKey; }
            set
            {
                subtractTimeModifierKey = value;
                OnPropertyChanged("SubtractTimeModifierKey");
                OnPropertyChanged("SubtractTimeKeyString");
            }
        }

        private Key? subtractTimeKey = null;
        public Key? SubtractTimeKey
        {
            get { return subtractTimeKey; }
            set
            {
                subtractTimeKey = value;
                OnPropertyChanged("SubtractTimeKey");
                OnPropertyChanged("SubtractTimeKeyString");
            }
        }

        public string SubtractTimeKeyString
        {
            get
            {
                return KeyTextHelper.ConvertKeyText(SubtractTimeModifierKey, SubtractTimeKey, "없음");
            }
        }

        private ModifierKeys? backModifierKey = null;
        public ModifierKeys? BackModifierKey
        {
            get { return backModifierKey; }
            set
            {
                backModifierKey = value;
                OnPropertyChanged("BackModifierKey");
                OnPropertyChanged("BackKeyString");
            }
        }

        private Key? backKey = null;
        public Key? BackKey
        {
            get { return backKey; }
            set
            {
                backKey = value;
                OnPropertyChanged("BackKey");
                OnPropertyChanged("BackKeyString");
            }
        }

        public string BackKeyString
        {
            get
            {
                return KeyTextHelper.ConvertKeyText(BackModifierKey, BackKey, "없음");
            }
        }

        private ModifierKeys? scytheModifierKey = null;
        public ModifierKeys? ScytheModifierKey
        {
            get { return scytheModifierKey; }
            set
            {
                scytheModifierKey = value;
                OnPropertyChanged("ScytheModifierKey");
                OnPropertyChanged("ScytheKeyString");
            }
        }

        private Key? scytheKey = null;
        public Key? ScytheKey
        {
            get { return scytheKey; }
            set
            {
                scytheKey = value;
                OnPropertyChanged("ScytheKey");
                OnPropertyChanged("ScytheKeyString");
            }
        }

        public string ScytheKeyString
        {
            get
            {
                return KeyTextHelper.ConvertKeyText(ScytheModifierKey, ScytheKey, "없음");
            }
        }

        private ModifierKeys? nextModifierKey = null;
        public ModifierKeys? NextModifierKey
        {
            get { return nextModifierKey; }
            set
            {
                nextModifierKey = value;
                OnPropertyChanged("NextModifierKey");
                OnPropertyChanged("NextKeyString");
            }
        }

        private Key? nextKey = null;
        public Key? NextKey
        {
            get { return nextKey; }
            set
            {
                nextKey = value;
                OnPropertyChanged("NextKey");
                OnPropertyChanged("NextKeyString");
            }
        }

        public string NextKeyString
        {
            get
            {
                return KeyTextHelper.ConvertKeyText(NextModifierKey, NextKey, "없음");
            }
        }

        private ModifierKeys? addTimeModifierKey = null;
        public ModifierKeys? AddTimeModifierKey
        {
            get { return addTimeModifierKey; }
            set
            {
                addTimeModifierKey = value;
                OnPropertyChanged("AddTimeModifierKey");
                OnPropertyChanged("AddTimeKeyString");
            }
        }

        private Key? addTimeKey = null;
        public Key? AddTimeKey
        {
            get { return addTimeKey; }
            set
            {
                addTimeKey = value;
                OnPropertyChanged("AddTimeKey");
                OnPropertyChanged("AddTimeKeyString");
            }
        }

        public string AddTimeKeyString
        {
            get
            {
                return KeyTextHelper.ConvertKeyText(AddTimeModifierKey, AddTimeKey, "없음");
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
            SubtractTimeKeySettingCommand = new RelayCommand(o => SubtractTimeKeySettingEvent((Window)o));
            BackKeySettingCommand = new RelayCommand(o => BackKeySettingEvent((Window)o));
            ScytheKeySettingCommand = new RelayCommand(o => ScytheKeySettingEvent((Window)o));
            NextKeySettingCommand = new RelayCommand(o => NextKeySettingEvent((Window)o));
            AddTimeKeySettingCommand = new RelayCommand(o => AddTimeKeySettingEvent((Window)o));
            ResetCommand = new RelayCommand(o => ResetEvent());
            OpenHillaUIBarCommand = new RelayCommand(o => OpenHillaUIBarEvent());
            CloseCommand = new RelayCommand(o => CloseEvent((Window)o));

            CurrentPhase = 1;
            LatestPatternTime = null;
            InternalLatestPatternTime = null;
            IsHelperON = false;
        }

        public void Initialize(SettingItem settingItem)
        {
            BackKey = settingItem.BackKey;
            BackModifierKey = settingItem.BackModifierKey;
            ScytheKey = settingItem.ScytheKey;
            ScytheModifierKey = settingItem.ScytheModifierKey;
            NextKey = settingItem.NextKey;
            NextModifierKey = settingItem.NextModifierKey;
            UIBarTop = settingItem.HILLA_UIBAR_TOP;
            UIBarLeft = settingItem.HILLA_UIBAR_LEFT;
            UIBarWidth = settingItem.HILLA_UIBAR_WIDTH;
            UIBarHeight = settingItem.HILLA_UIBAR_HEIGHT;
            UIBarTransparency = settingItem.HILLA_UIBAR_TRANSPARENCY;
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

        private void SubtractTimeKeySettingEvent(Window window)
        {
            var dialog = new WindowTimerPressKeyboard();
            var vm = dialog.DataContext as ViewModelTimerPressKeyboard;

            dialog.Left = window.Left + (window.ActualWidth - dialog.Width) / 2;
            dialog.Top = window.Top + (window.ActualHeight - dialog.Height) / 2;

            vm.PressedKey = SubtractTimeKey;
            vm.ModifierKey = SubtractTimeModifierKey;
            vm.ChangeKeyText();

            dialog.ShowDialog();

            SubtractTimeKey = vm.PressedKey;
            SubtractTimeModifierKey = vm.ModifierKey;
        }

        private void BackKeySettingEvent(Window window)
        {
            var dialog = new WindowTimerPressKeyboard();
            var vm = dialog.DataContext as ViewModelTimerPressKeyboard;

            dialog.Left = window.Left + (window.ActualWidth - dialog.Width) / 2;
            dialog.Top = window.Top + (window.ActualHeight - dialog.Height) / 2;

            vm.PressedKey = BackKey;
            vm.ModifierKey = BackModifierKey;
            vm.ChangeKeyText();

            dialog.ShowDialog();

            BackKey = vm.PressedKey;
            BackModifierKey = vm.ModifierKey;
        }

        private void ScytheKeySettingEvent(Window window)
        {
            var dialog = new WindowTimerPressKeyboard();
            var vm = dialog.DataContext as ViewModelTimerPressKeyboard;

            dialog.Left = window.Left + (window.ActualWidth - dialog.Width) / 2;
            dialog.Top = window.Top + (window.ActualHeight - dialog.Height) / 2;

            vm.PressedKey = ScytheKey;
            vm.ModifierKey = ScytheModifierKey;
            vm.ChangeKeyText();

            dialog.ShowDialog();

            ScytheKey = vm.PressedKey;
            ScytheModifierKey = vm.ModifierKey;
        }

        private void NextKeySettingEvent(Window window)
        {
            var dialog = new WindowTimerPressKeyboard();
            var vm = dialog.DataContext as ViewModelTimerPressKeyboard;

            dialog.Left = window.Left + (window.ActualWidth - dialog.Width) / 2;
            dialog.Top = window.Top + (window.ActualHeight - dialog.Height) / 2;

            vm.PressedKey = NextKey;
            vm.ModifierKey = NextModifierKey;
            vm.ChangeKeyText();

            dialog.ShowDialog();

            NextKey = vm.PressedKey;
            NextModifierKey = vm.ModifierKey;
        }

        private void AddTimeKeySettingEvent(Window window)
        {
            var dialog = new WindowTimerPressKeyboard();
            var vm = dialog.DataContext as ViewModelTimerPressKeyboard;

            dialog.Left = window.Left + (window.ActualWidth - dialog.Width) / 2;
            dialog.Top = window.Top + (window.ActualHeight - dialog.Height) / 2;

            vm.PressedKey = AddTimeKey;
            vm.ModifierKey = AddTimeModifierKey;
            vm.ChangeKeyText();

            dialog.ShowDialog();

            AddTimeKey = vm.PressedKey;
            AddTimeModifierKey = vm.ModifierKey;
        }

        private void ResetEvent()
        {
            CurrentPhase = 1;
            LatestPatternTime = null;
            InternalLatestPatternTime = null;
        }

        private void OpenHillaUIBarEvent()
        {
            var window = WindowVerusHillaUIBar.Instance;
            window.DataContext = this;

            window.Show();
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

        public void KeyDownEvent(ModifierKeys modifierKeys, Key inputKey)
        {
            if (IsOpenSettingWindow)
                return;

            CheckHillaKey(modifierKeys, inputKey);
        }

        private void CheckHillaKey(ModifierKeys modifierKeys, Key inputKey)
        {
            if (!IsHelperON)
                return;

            if (!(SubtractTimeModifierKey == null && SubtractTimeKey == null))
            {
                if (KeyInputHelper.CheckPressModifierAndNormalKey(modifierKeys, inputKey, SubtractTimeModifierKey, SubtractTimeKey))
                    ChangeTime(-5);
            }

            if (!(BackModifierKey == null && BackKey == null))
            {
                if (KeyInputHelper.CheckPressModifierAndNormalKey(modifierKeys, inputKey, BackModifierKey, BackKey))
                    BackKeyEvent();
            }

            if (!(ScytheModifierKey == null && ScytheKey == null))
            {
                if (KeyInputHelper.CheckPressModifierAndNormalKey(modifierKeys, inputKey, ScytheModifierKey, ScytheKey))
                    ScytheKeyEvent();
            }

            if (!(NextModifierKey == null && NextKey == null))
            {
                if (KeyInputHelper.CheckPressModifierAndNormalKey(modifierKeys, inputKey, NextModifierKey, NextKey))
                    NextKeyEvent();
            }

            if (!(AddTimeModifierKey == null && AddTimeKey == null))
            {
                if (KeyInputHelper.CheckPressModifierAndNormalKey(modifierKeys, inputKey, AddTimeModifierKey, AddTimeKey))
                    ChangeTime(5);
            }
        }
    }
}
