using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Models;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using Telerik.Windows.Controls;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace MapleUtility.Plugins.ViewModels.Views
{
    public class ViewModelSettingWindow : Notifier, IDisposable
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

        private ObservableCollection<TimerItem> timerList;
        public ObservableCollection<TimerItem> TimerList
        {
            get { return timerList; }
            set
            {
                timerList = value;
                OnPropertyChanged("TimerList");
                OnPropertyChanged("OrderedTimerList");
            }
        }

        public IEnumerable<TimerItem> OrderedTimerList
        {
            get { return TimerList.OrderBy(o => o.Priority); }
        }

        public IEnumerable<SoundItem> OrderedSoundList
        {
            get { return SoundList.OrderBy(o => o.Priority); }
        }

        private List<string> uiBarStyleList;
        public List<string> UIBarStyleList
        {
            get { return uiBarStyleList; }
            set
            {
                uiBarStyleList = value;
                OnPropertyChanged("UIBarStyleList");
            }
        }

        private string selectedUIBarStyle;
        public string SelectedUIBarStyle
        {
            get { return selectedUIBarStyle; }
            set
            {
                selectedUIBarStyle = value;
                OnPropertyChanged("SelectedUIBarStyle");
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

        private Color remainSquareColor;
        public Color RemainSquareColor
        {
            get { return remainSquareColor; }
            set
            {
                remainSquareColor = value;
                OnPropertyChanged("RemainSquareColor");
                OnPropertyChanged("RemainSquareResultColor");
            }
        }

        private Color? remainSquareTempColor;
        public Color? RemainSquareTempColor
        {
            get { return remainSquareTempColor; }
            set
            {
                remainSquareTempColor = value;
                OnPropertyChanged("RemainSquareTempColor");
                OnPropertyChanged("RemainSquareResultColor");
            }
        }

        public Color RemainSquareResultColor
        {
            get
            {
                if (RemainSquareTempColor != null)
                    return RemainSquareTempColor.Value;
                else
                    return RemainSquareColor;
            }
            set
            {
                RemainSquareColor = value;
                OnPropertyChanged("RemainSquareResultColor");
            }
        }

        private float remainBackAlpha;
        public float RemainBackAlpha
        {
            get { return remainBackAlpha; }
            set
            {
                int intValue = Convert.ToInt32(value);

                if (intValue > 100)
                    intValue = 100;
                if (intValue < 0)
                    intValue = 0;

                remainBackAlpha = intValue;
                OnPropertyChanged("RemainBackAlpha");
                OnPropertyChanged("RemainBackColor");
            }
        }

        public Color RemainBackColor
        {
            get
            {
                var alpha = Convert.ToByte(Math.Truncate(255 / 100 * remainBackAlpha));
                return Color.FromArgb(alpha, 0, 0, 0);
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
            }
        }

        private FontFamily selectedUIBarFont;
        public FontFamily SelectedUIBarFont
        {
            get { return selectedUIBarFont; }
            set
            {
                selectedUIBarFont = value;
                OnPropertyChanged("SelectedUIBarFont");
            }
        }

        private int uIBarFontSize;
        public int UIBarFontSize
        {
            get { return uIBarFontSize; }
            set
            {
                if (value <= 12)
                    uIBarFontSize = 12;
                else if (value >= 26)
                    uIBarFontSize = 26;
                else
                    uIBarFontSize = value;
                OnPropertyChanged("UIBarFontSize");
                OnPropertyChanged("UIBarNameFontSize");
            }
        }

        public int UIBarNameFontSize
        {
            get
            {
                return UIBarFontSize - 6;
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

        public string OnOffKeyString
        {
            get
            {
                return KeyTextHelper.ConvertKeyText(TimerOnOffModifierKey, TimerOnOffKey, "없음");
            }
        }

        private ModifierKeys? pauseAllModifierKey = null;
        public ModifierKeys? PauseAllModifierKey
        {
            get { return pauseAllModifierKey; }
            set
            {
                pauseAllModifierKey = value;
                OnPropertyChanged("PauseAllModifierKey");
                OnPropertyChanged("PauseAllKeyString");
            }
        }

        private Key? pauseAllKey = null;
        public Key? PauseAllKey
        {
            get { return pauseAllKey; }
            set
            {
                pauseAllKey = value;
                OnPropertyChanged("PauseAllKey");
                OnPropertyChanged("PauseAllKeyString");
            }
        }

        public string PauseAllKeyString
        {
            get
            {
                return KeyTextHelper.ConvertKeyText(PauseAllModifierKey, PauseAllKey, "없음");
            }
        }

        private ModifierKeys? timerLockModifierKey = null;
        public ModifierKeys? TimerLockModifierKey
        {
            get { return timerLockModifierKey; }
            set
            {
                timerLockModifierKey = value;
                OnPropertyChanged("TimerLockModifierKey");
                OnPropertyChanged("TimerLockKeyString");
            }
        }

        private Key? timerLockKey = null;
        public Key? TimerLockKey
        {
            get { return timerLockKey; }
            set
            {
                timerLockKey = value;
                OnPropertyChanged("TimerLockKey");
                OnPropertyChanged("TimerLockKeyString");
            }
        }

        public string TimerLockKeyString
        {
            get
            {
                return KeyTextHelper.ConvertKeyText(TimerLockModifierKey, TimerLockKey, "없음");
            }
        }

        public RadTabItem SelectedTab
        {
            set
            {
                if(WavePlayer != null)
                    WavePlayer.Stop();
            }
        }

        public PresetItem CurrentPreset = null;
        public IWavePlayer WavePlayer;
        private bool disposedValue;

        #region Button Command Variables
        public ICommand OnOffSettingKeyCommand { get; set; }
        public ICommand PauseAllSettingKeyCommand { get; set; }
        public ICommand TimerLockSettingKeyCommand { get; set; }
        public ICommand OpenColorPickerCommand { get; set; }
        public ICommand CopyCurrentPresetCommand { get; set; }
        public ICommand AddPresetCommand { get; set; }
        public ICommand RemovePresetCommand { get; set; }
        public ICommand SyncImageFilesCommand { get; set; }
        public ICommand AddSoundCommand { get; set; }
        public ICommand RemoveSoundCommand { get; set; }
        public ICommand PresetCheckCommand { get; set; }
        public ICommand SoundCheckCommand { get; set; }
        public ICommand PlayTestSoundCommand { get; set; }
        public ICommand LoadDefaultSettingCommand { get; set; }
        public ICommand SelectSoundCommand { get; set; }
        public ICommand PriorityUpCommand { get; set; }
        public ICommand PriorityDownCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion

        public ViewModelSettingWindow()
        {
            OnOffSettingKeyCommand = new RelayCommand(o => OnOffSettingKeyEvent((Window)o));
            PauseAllSettingKeyCommand = new RelayCommand(o => PauseAllSettingKeyEvent((Window)o));
            TimerLockSettingKeyCommand = new RelayCommand(o => TimerLockSettingKeyEvent((Window)o));
            OpenColorPickerCommand = new RelayCommand(o => OpenColorPickerEvent());
            CopyCurrentPresetCommand = new RelayCommand(o => CopyCurrentPresetEvent());
            AddPresetCommand = new RelayCommand(o => AddPresetEvent());
            RemovePresetCommand = new RelayCommand(o => RemovePresetEvent());
            SyncImageFilesCommand = new RelayCommand(o => SyncImageFilesEvent());
            AddSoundCommand = new RelayCommand(o => AddSoundEvent());
            RemoveSoundCommand = new RelayCommand(o => RemoveSoundEvent());
            PresetCheckCommand = new RelayCommand(o => PresetCheckEvent());
            SoundCheckCommand = new RelayCommand(o => SoundCheckEvent());
            PlayTestSoundCommand = new RelayCommand(o => PlaySound((SoundItem)o));
            LoadDefaultSettingCommand = new RelayCommand(o => LoadDefaultSettingEvent());
            SelectSoundCommand = new RelayCommand(o => SelectSoundEvent((SoundItem)o));
            PriorityUpCommand = new RelayCommand(o => PriorityUpEvent((TimerItem)o));
            PriorityDownCommand = new RelayCommand(o => PriorityDownEvent((TimerItem)o));
            CloseCommand = new RelayCommand(o => CloseEvent((Window)o));

            UIBarStyleList = new List<string>()
            {
                "스택형",
                "고정형",
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (WavePlayer != null)
                    {
                        WavePlayer.Stop();
                        WavePlayer.Dispose();
                        WavePlayer = null;
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~ViewModelSettingWindow()
        {
            Dispose();
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

        private void PauseAllSettingKeyEvent(Window window)
        {
            var dialog = new WindowTimerPressKeyboard();
            var vm = dialog.DataContext as ViewModelTimerPressKeyboard;

            dialog.Left = window.Left + (window.ActualWidth - dialog.Width) / 2;
            dialog.Top = window.Top + (window.ActualHeight - dialog.Height) / 2;

            vm.PressedKey = PauseAllKey;
            vm.ModifierKey = PauseAllModifierKey;
            vm.ChangeKeyText();

            dialog.ShowDialog();

            PauseAllKey = vm.PressedKey;
            PauseAllModifierKey = vm.ModifierKey;
        }

        private void TimerLockSettingKeyEvent(Window window)
        {
            var dialog = new WindowTimerPressKeyboard();
            var vm = dialog.DataContext as ViewModelTimerPressKeyboard;

            dialog.Left = window.Left + (window.ActualWidth - dialog.Width) / 2;
            dialog.Top = window.Top + (window.ActualHeight - dialog.Height) / 2;

            vm.PressedKey = TimerLockKey;
            vm.ModifierKey = TimerLockModifierKey;
            vm.ChangeKeyText();

            dialog.ShowDialog();

            TimerLockKey = vm.PressedKey;
            TimerLockModifierKey = vm.ModifierKey;
        }

        private void OpenColorPickerEvent()
        {
            var window = new WindowTimerColorEditor();
            window.DataContext = this;

            RemainSquareTempColor = RemainSquareColor;

            window.ShowDialog();

            if(window.DialogResult == true)
                RemainSquareColor = RemainSquareTempColor.Value;
            RemainSquareTempColor = null;
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

        private void SyncImageFilesEvent()
        {
            var images = Directory.GetFiles(Defines.ImageFolderPath).ToList();
            ImageList.Clear();

            foreach(var image in images)
            {
                var fileName = Path.GetFileName(image);
                var ext = Path.GetExtension(fileName).ToLower();

                if (ext != ".png" && ext != ".jpg" && ext != ".gif")
                    continue;

                if (ImageList.Any(o => o.FileName == fileName))
                    continue;

                ImageList.Add(new ImageItem(fileName));
            }

            ImageList = new ObservableCollection<ImageItem>(ImageList.OrderBy(o => o.FileName));
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

            var currentPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var filePath = openFileDialog.FileName.Replace(currentPath, ".");

            var soundItem = new SoundItem()
            {
                Priority = SoundList.Count() > 0 ? SoundList.Max(o => o.Priority) + 1 : 1,
                Name = Path.GetFileNameWithoutExtension(filePath),
                Path = filePath,
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

            var currentPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var filePath = openFileDialog.FileName.Replace(currentPath, ".");

            item.Path = filePath;
        }

        private void PriorityUpEvent(TimerItem item)
        {
            PriorityEvent(item, item.Priority - 1);
        }

        private void PriorityDownEvent(TimerItem item)
        {
            PriorityEvent(item, item.Priority + 1);
        }

        private void PriorityEvent(TimerItem item, int resultPriority)
        {
            if(OrderedTimerList.Count() > 0)
                OrderedTimerList.LastOrDefault().IsLast = false;
            var targetTimer = TimerList.Where(o => o.Priority == resultPriority).FirstOrDefault();
            targetTimer.Priority = item.Priority;
            item.Priority = resultPriority;

            if (OrderedTimerList.Count() > 0)
                OrderedTimerList.LastOrDefault().IsLast = true;

            OnPropertyChanged("TimerList");
            OnPropertyChanged("OrderedTimerList");
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
            foreach (var sound in SoundList.Where(o => o.IsChecked).ToList())
                SoundList.Remove(sound);

            var priority = 1;
            foreach (var sound in OrderedSoundList.ToList())
                sound.Priority = priority++;

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
            RefreshSoundList();
        }

        private void PlaySound(SoundItem soundItem)
        {
            try
            {
                if (soundItem == null || soundItem.Path == null)
                    return;

                if (WavePlayer != null)
                {
                    WavePlayer.Stop();
                    WavePlayer.Dispose();
                    WavePlayer = null;
                }

                var wavePlayer = new WaveOut();

                WaveStream waveStream;

                if (soundItem.IsInternalSound)
                {
                    StreamResourceInfo resource = System.Windows.Application.GetResourceStream(new Uri("MapleUtility;component/Plugins/Sounds/" + soundItem.Path, UriKind.Relative));
                    waveStream = new Mp3FileReader(resource.Stream);
                }
                else
                    waveStream = new MediaFoundationReader(soundItem.Path);

                WaveChannel32 inputStream = new WaveChannel32(waveStream);
                inputStream.PadWithZeroes = false;

                wavePlayer.Volume = 1.0f;
                wavePlayer.Init(inputStream);
                wavePlayer.Play();

                wavePlayer.PlaybackStopped += delegate (object sender, StoppedEventArgs e)
                {
                    wavePlayer.Dispose();
                    waveStream.Dispose();
                };

                WavePlayer = wavePlayer;
            }
            catch (Exception)
            {
                DebugLogHelper.Write(soundItem.Path + " 타이머 사운드 재생 중 오류가 발생했습니다.");
            }
        }

        private void LoadDefaultSettingEvent()
        {
            if (MessageBox.Show("알림 사운드 리스트가 기본값으로 설정되며, 타이머에 설정된 알림 사운드가 모두 초기화됩니다. 정말 하시겠습니까?", "Load Default Setting", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                SoundList = InitialHelper.InitializeSoundList();
        }

        public void RefreshSoundList()
        {
            OnPropertyChanged("OrderedSoundList");
        }

        private void CloseEvent(Window window)
        {
            window.Close();
            Dispose();
        }
    }
}
