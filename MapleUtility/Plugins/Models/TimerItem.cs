using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Views.Windows.Timer;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Windows;
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

        private float volume = 100;
        [JsonIgnore]
        public string Volume
        {
            get { return Convert.ToInt32(volume).ToString(); }
            set
            {
                int intValue = 0;
                try
                {
                    intValue = Int32.Parse(RegexHelper.CleanStringOfNonDigits(value));
                }
                catch (Exception e) { intValue = 0; }

                if (intValue > 100)
                    intValue = 100;
                if (intValue < 0)
                    intValue = 0;

                volume = intValue;
                OnPropertyChanged("Volume");
                OnPropertyChanged("TimerVolume");
            }
        }

        [JsonIgnore]
        public float TimerVolume
        {
            get { return volume; }
            set
            {
                volume = value;
                OnPropertyChanged("Volume");
                OnPropertyChanged("TimerVolume");
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
                RefreshRemainTime();
            }
        }

        [JsonIgnore]
        public TimeSpan? RemainTime
        {
            get
            {
                var remainTime = EndTime - DateTime.Now.AddSeconds(-1);
                return remainTime;
            }
        }

        [JsonIgnore]
        public string RemainTimeSeconds
        {
            get
            {
                if (!RemainTime.HasValue)
                    return "";

                var totalSeconds = RemainTime.Value.TotalSeconds - 1;

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
                OnPropertyChanged("SoundItemName");
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
                AlertKey = this.AlertKey,
                TimerTime = this.TimerTime,
                Volume = this.Volume,
                ImageItem = this.ImageItem,
                SoundItem = this.SoundItem,
                Name = this.Name
            };
        }
    }
}
