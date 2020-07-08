using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Models;
using MapleUtility.Plugins.ViewModels.Views.Timer;
using MapleUtility.Plugins.Views.Windows.Timer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace MapleUtility.Plugins.ViewModels.UserControls
{
    public class ViewModelUCVerusHillaHelper : Notifier
    {
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

        public int UIBarWidth
        {
            get { return Defines.HILLA_UIBAR_WIDTH; }
            set
            {
                Defines.HILLA_UIBAR_WIDTH = value;
                OnPropertyChanged("UIBarWidth");
            }
        }

        public int UIBarHeight
        {
            get { return Defines.HILLA_UIBAR_HEIGHT; }
            set
            {
                Defines.HILLA_UIBAR_HEIGHT = value;
                OnPropertyChanged("UIBarHeight");
            }
        }

        public bool IsOpenSettingWindow = false;
        public List<Key> PressedKeyList = new List<Key>();

        #region Button Command Variables
        public ICommand BackKeyCommand { get; set; }
        public ICommand ScytheKeyCommand { get; set; }
        public ICommand NextKeyCommand { get; set; }
        public ICommand BackKeySettingCommand { get; set; }
        public ICommand ScytheKeySettingCommand { get; set; }
        public ICommand NextKeySettingCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand OpenHillaUIBarCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion

        public ViewModelUCVerusHillaHelper()
        {
            BackKeyCommand = new RelayCommand(o => BackKeyEvent());
            ScytheKeyCommand = new RelayCommand(o => ScytheKeyEvent());
            NextKeyCommand = new RelayCommand(o => NextKeyEvent());
            BackKeySettingCommand = new RelayCommand(o => BackKeySettingEvent((Window)o));
            ScytheKeySettingCommand = new RelayCommand(o => ScytheKeySettingEvent((Window)o));
            NextKeySettingCommand = new RelayCommand(o => NextKeySettingEvent((Window)o));
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
        }

        private void BackKeyEvent()
        {
            if (CurrentPhase > 1)
                CurrentPhase--;
        }

        private void ScytheKeyEvent()
        {
            if (LatestPatternTime == null)
            {
                LatestPatternTime = new TimeSpan(0, 27, 13);
                InternalLatestPatternTime = DateTime.Now;
            }
            else
            {
                LatestPatternTime = LatestPatternTime + (InternalLatestPatternTime - DateTime.Now);
                if (LatestPatternTime.Value.TotalSeconds < 0)
                    ResetEvent();
                else
                    InternalLatestPatternTime = DateTime.Now;
            }
        }

        private void NextKeyEvent()
        {
            if (CurrentPhase < 3)
                CurrentPhase++;
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
                if (RemainTime.Value.TotalSeconds <= 0)
                    ScytheKeyEvent();
            }
        }

        public void KeyDownEvent()
        {
            if (IsOpenSettingWindow)
                return;

            CheckHillaKey();
        }

        private void CheckHillaKey()
        {
            if (!IsHelperON)
                return;

            if (!(BackModifierKey == null && BackKey == null))
            {
                if (KeyInputHelper.CheckPressModifierAndNormalKey(BackModifierKey, BackKey))
                    BackKeyEvent();
            }

            if (!(ScytheModifierKey == null && ScytheKey == null))
            {
                if (KeyInputHelper.CheckPressModifierAndNormalKey(ScytheModifierKey, ScytheKey))
                    ScytheKeyEvent();
            }

            if (!(NextModifierKey == null && NextKey == null))
            {
                if (KeyInputHelper.CheckPressModifierAndNormalKey(NextModifierKey, NextKey))
                    NextKeyEvent();
            }
        }
    }
}
