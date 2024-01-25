using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Views.Windows.Timer;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Serialization;

namespace MapleUtility.Plugins.Models
{
    public class TimerItem : Notifier, ICloneable
    {
        [JsonIgnore]
        public IWavePlayer PrevWavePlayer;

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private int priority;
        public int Priority
        {
            get { return priority; }
            set
            {
                priority = value;
                OnPropertyChanged("Prioirty");
            }
        }

        private PresetItem preset;
        public PresetItem Preset
        {
            get { return preset; }
            set
            {
                preset = value;
                OnPropertyChanged("Preset");
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

        [JsonIgnore]
        private bool isLast;
        [JsonIgnore]
        public bool IsLast
        {
            get { return isLast; }
            set
            {
                isLast = value;
                OnPropertyChanged("IsLast");
            }
        }

        [JsonIgnore]
        private bool isChecked;
        [JsonIgnore]
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        [JsonIgnore]
        private DateTime? endTime;
        [JsonIgnore]
        public DateTime? EndTime
        {
            get { return endTime; }
            set
            {
                endTime = value;
                OnPropertyChanged("EndTime");
                IsAlertBeforeTimer = false;
                RefreshRemainTime();
            }
        }

        [JsonIgnore]
        private DateTime? pauseTime;
        [JsonIgnore]
        public DateTime? PauseTime
        {
            get { return pauseTime; }
            set
            {
                pauseTime = value;
                OnPropertyChanged("PauseTime");
                RefreshRemainTime();
            }
        }

        [JsonIgnore]
        public TimeSpan? RemainTime
        {
            get
            {
                if (!EndTime.HasValue)
                    return null;

                if (PauseTime.HasValue)
                    return EndTime - DateTime.Now + (DateTime.Now - PauseTime);

                return EndTime - DateTime.Now;
            }
        }

        [JsonIgnore]
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

        [JsonIgnore]
        public float RemainRadius
        {
            get
            {
                if (!RemainTime.HasValue || !TimerTime.HasValue)
                    return 359.9f;

                var result = (float)(360.0 * (RemainTime.Value.TotalSeconds / TimerTime.Value.TotalSeconds));
                if (result >= 360.0f)
                    return 359.9f;

                return result;
            }
        }

        private TimeSpan? timerTime;
        public TimeSpan? TimerTime
        {
            get { return timerTime; }
            set
            {
                timerTime = value;
                OnPropertyChanged("TimerTime");
            }
        }

        private bool isTimerResetTimeChecked;
        public bool IsTimerResetTimeChecked
        {
            get { return isTimerResetTimeChecked; }
            set
            {
                isTimerResetTimeChecked = value;
                OnPropertyChanged("IsTimerResetTimeChecked");
            }
        }

        [JsonIgnore]
        private bool isTimerLoopChecked;
        [JsonIgnore]
        public bool IsTimerLoopChecked
        {
            get { return isTimerLoopChecked; }
            set
            {
                isTimerLoopChecked = value;
                OnPropertyChanged("IsTimerLoopChecked");
            }
        }

        [JsonIgnore]
        public string KeyString
        {
            get
            {
                return KeyTextHelper.ConvertKeyText(ModifierKey, AlertKey, "없음");
            }
        }

        private ImageItem imageItem;
        public ImageItem ImageItem
        {
            get { return imageItem; }
            set
            {
                imageItem = value;
                OnPropertyChanged("ImageItem");
            }
        }

        private SoundItem soundItem;
        public SoundItem SoundItem
        {
            get { return soundItem; }
            set
            {
                soundItem = value;
                OnPropertyChanged("SoundItem");
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

        private ModifierKeys? modifierKey = null;
        public ModifierKeys? ModifierKey
        {
            get { return modifierKey; }
            set
            {
                modifierKey = value;
                OnPropertyChanged("ModifierKey");
                OnPropertyChanged("KeyString");
            }
        }

        private Key? alertKey = null;
        public Key? AlertKey
        {
            get { return alertKey; }
            set
            {
                alertKey = value;
                OnPropertyChanged("AlertKey");
                OnPropertyChanged("KeyString");
            }
        }

        [JsonIgnore]
        public bool IsAlertBeforeTimer { get; set; } = false;

        public void RefreshRemainTime()
        {
            OnPropertyChanged("RemainTime");
            OnPropertyChanged("RemainTimeSeconds");
            OnPropertyChanged("RemainRadius");
        }

        public void RefreshSoundString()
        {
            OnPropertyChanged("SoundString");
        }

        public TimerItem CreateNewPresetClone(PresetItem preset)
        {
            var clone = this.Clone() as TimerItem;
            clone.Preset = preset;

            return clone;
        }

        public object Clone()
        {
            return new TimerItem()
            {
                Priority = this.Priority,
                AlertKey = this.AlertKey,
                ModifierKey = this.ModifierKey,
                IsTimerLoopChecked = this.IsTimerLoopChecked,
                IsTimerResetTimeChecked = this.IsTimerResetTimeChecked,
                TimerTime = this.TimerTime,
                Volume = this.Volume,
                ImageItem = this.ImageItem,
                SoundItem = this.SoundItem,
                BeforeSoundItem = this.BeforeSoundItem,
                Name = this.Name
            };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
