using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Models;
using MapleUtility.Plugins.ViewModels.Views.Timer;
using MapleUtility.Plugins.Views.Windows.Timer;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace MapleUtility.Plugins.ViewModels.Views
{
    public class ViewModelSettingWindow : Notifier
    {
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

        private bool isTimerResetChecked;
        public bool IsTimerResetChecked
        {
            get { return isTimerResetChecked; }
            set
            {
                isTimerResetChecked = value;
                OnPropertyChanged("IsTimerResetChecked");
            }
        }

        public bool IsPresetAllChecked
        {
            get
            {
                if (PresetList == null || PresetList.Count() == 0)
                    return false;
                else
                    return PresetList.Where(o => o.IsChecked).Count() == PresetList.Count();
            }
            set
            {
                if (PresetList == null || PresetList.Count() == 0)
                    return;

                foreach (var Timer in PresetList)
                    Timer.IsChecked = value;

                PresetCheckEvent();
            }
        }

        public bool IsSoundAllChecked
        {
            get
            {
                if (SoundList == null || SoundList.Count() == 0)
                    return false;
                else
                    return SoundList.Where(o => o.IsChecked).Count() == SoundList.Count();
            }
            set
            {
                if (SoundList == null || SoundList.Count() == 0)
                    return;

                foreach (var Timer in SoundList)
                    Timer.IsChecked = value;

                SoundCheckEvent();
            }
        }

        public bool IsRemovePresetEnabled
        {
            get
            {
                if (PresetList == null || PresetList.Count() == 0 || PresetList.Where(o => o.IsChecked).Count() == 0)
                    return false;
                else
                    return true;
            }
        }

        public bool IsRemoveSoundEnabled
        {
            get
            {
                if (SoundList == null || SoundList.Count() == 0 || SoundList.Where(o => o.IsChecked).Count() == 0)
                    return false;
                else
                    return true;
            }
        }

        public string OnOffKeyString
        {
            get
            {
                if (TimerOnOffModifierKey == null && TimerOnOffKey == null)
                    return "없음";

                var resultText = "";
                if (TimerOnOffModifierKey.HasValue)
                {
                    if (TimerOnOffModifierKey.Value.HasFlag(ModifierKeys.Control))
                        resultText += "Ctrl + ";
                    if (TimerOnOffModifierKey.Value.HasFlag(ModifierKeys.Alt))
                        resultText += "Alt + ";
                    if (TimerOnOffModifierKey.Value.HasFlag(ModifierKeys.Shift))
                        resultText += "Shift + ";
                }

                if (TimerOnOffKey == null && resultText.Length >= 2)
                    return resultText.Substring(0, resultText.Length - 2);
                else
                    return resultText + TimerOnOffKey.ToString();
            }
        }

        private bool isAlertShowScreenChecked;
        public bool IsAlertShowScreenChecked
        {
            get { return isAlertShowScreenChecked; }
            set
            {
                isAlertShowScreenChecked = value;
                OnPropertyChanged("IsAlertShowScreenChecked");
            }
        }

        private int alertDuration;
        public int AlertDuration
        {
            get { return alertDuration; }
            set
            {
                alertDuration = value;
                OnPropertyChanged("AlertDuration");
            }
        }

        public float AlertDurationSecond
        {
            get { return AlertDuration / 1000f; }
            set
            {
                AlertDuration = Convert.ToInt32(value * 1000);
                OnPropertyChanged("AlertDurationSecond");
            }
        }

        private ModifierKeys? timerOnOffModifierKey = null;
        public ModifierKeys? TimerOnOffModifierKey
        {
            get { return timerOnOffModifierKey; }
            set
            {
                timerOnOffModifierKey = value;
                OnPropertyChanged("TimerOnOffModifierKey");
                OnPropertyChanged("OnOffKeyString");
            }
        }

        private Key? timerOnOffKey = null;
        public Key? TimerOnOffKey
        {
            get { return timerOnOffKey; }
            set
            {
                timerOnOffKey = value;
                OnPropertyChanged("TimerOnOffKey");
                OnPropertyChanged("OnOffKeyString");
            }
        }

        public PresetItem CurrentPreset = null;
        public ObservableCollection<TimerItem> TimerList = null;

        #region Button Command Variables
        public ICommand OnOffSettingKeyCommand { get; set; }
        public ICommand CopyCurrentPresetCommand { get; set; }
        public ICommand AddPresetCommand { get; set; }
        public ICommand RemovePresetCommand { get; set; }
        public ICommand AddSoundCommand { get; set; }
        public ICommand RemoveSoundCommand { get; set; }
        public ICommand PresetCheckCommand { get; set; }
        public ICommand SoundCheckCommand { get; set; }
        public ICommand LoadDefaultSettingCommand { get; set; }
        public ICommand SelectSoundCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion

        public ViewModelSettingWindow()
        {
            OnOffSettingKeyCommand = new RelayCommand(o => OnOffSettingKeyEvent((Window)o));
            CopyCurrentPresetCommand = new RelayCommand(o => CopyCurrentPresetEvent());
            AddPresetCommand = new RelayCommand(o => AddPresetEvent());
            RemovePresetCommand = new RelayCommand(o => RemovePresetEvent());
            AddSoundCommand = new RelayCommand(o => AddSoundEvent());
            RemoveSoundCommand = new RelayCommand(o => RemoveSoundEvent());
            PresetCheckCommand = new RelayCommand(o => PresetCheckEvent());
            SoundCheckCommand = new RelayCommand(o => SoundCheckEvent());
            LoadDefaultSettingCommand = new RelayCommand(o => LoadDefaultSettingEvent());
            SelectSoundCommand = new RelayCommand(o => SelectSoundEvent((SoundItem)o));
            CloseCommand = new RelayCommand(o => CloseEvent((Window)o));
        }

        private void OnOffSettingKeyEvent(Window window)
        {
            var dialog = new WindowTimerPressKeyboard();
            var vm = dialog.DataContext as ViewModelTimerPressKeyboard;

            dialog.Left = window.Left + (window.ActualWidth - dialog.Width) / 2;
            dialog.Top = window.Top + (window.ActualHeight - dialog.Height) / 2;

            vm.PressedKey = TimerOnOffKey;
            vm.ModifierKey = TimerOnOffModifierKey;
            vm.ChangeKeyText();

            dialog.ShowDialog();

            TimerOnOffKey = vm.PressedKey;
            TimerOnOffModifierKey = vm.ModifierKey;
        }

        private void CopyCurrentPresetEvent()
        {
            var presetItem = new PresetItem()
            {
                Name = CurrentPreset.Name + "_Clone " + DateTime.Now.ToString("hh-mm-ss")
            };

            foreach(var timer in TimerList.Where(o => o.Preset == CurrentPreset).ToList())
            {
                TimerList.Add(timer.CreateNewPresetClone(presetItem));
            }

            PresetList.Add(presetItem);

            PresetCheckEvent();
        }

        private void AddPresetEvent()
        {
            var presetItem = new PresetItem()
            {
                Name = ""
            };

            PresetList.Add(presetItem);

            PresetCheckEvent();
        }

        private void AddSoundEvent()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Sound Files (*.wav, *.mp3)|*.wav; *.mp3";

            var result = openFileDialog.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;

            var soundItem = new SoundItem()
            {
                Name = Path.GetFileNameWithoutExtension(openFileDialog.FileName),
                Path = openFileDialog.FileName,
                IsInternalSound = false
            };

            SoundList.Add(soundItem);

            SoundCheckEvent();
        }

        private void SelectSoundEvent(SoundItem item)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Sound Files (*.wav, *.mp3)|*.wav; *.mp3";

            var result = openFileDialog.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;

            item.Path = openFileDialog.FileName;
        }

        private void RemovePresetEvent()
        {
            foreach (var timer in PresetList.Where(o => o.IsChecked).ToList())
                PresetList.Remove(timer);

            if(PresetList.Count() == 0)
            {
                PresetList.Add(new PresetItem()
                {
                    Name = "Default"
                });
            }

            PresetCheckEvent();
        }

        private void RemoveSoundEvent()
        {
            foreach (var timer in SoundList.Where(o => o.IsChecked).ToList())
                SoundList.Remove(timer);

            SoundCheckEvent();
        }

        private void PresetCheckEvent()
        {
            OnPropertyChanged("IsPresetAllChecked");
            OnPropertyChanged("IsRemovePresetEnabled");
        }

        private void SoundCheckEvent()
        {
            OnPropertyChanged("IsSoundAllChecked");
            OnPropertyChanged("IsRemoveSoundEnabled");
        }

        private void LoadDefaultSettingEvent()
        {
            if (MessageBox.Show("알림 사운드 리스트가 기본값으로 설정되며, 타이머에 설정된 알림 사운드가 모두 초기화됩니다. 정말 하시겠습니까?", "Load Default Setting", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                SoundList = InitialHelper.InitializeSoundList();
        }

        private void CloseEvent(Window window)
        {
            window.Close();
        }
    }
}
