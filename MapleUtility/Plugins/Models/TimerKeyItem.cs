using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.ViewModels.Views.Timer;
using MapleUtility.Plugins.Views.Windows.Timer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MapleUtility.Plugins.Models
{
    public class TimerKeyItem : Notifier
    {
        public string Name { get; set; }

        private float time = 0;
        public float Time
        {
            get { return time; }
            set
            {
                time = Math.Abs(value);
                OnPropertyChanged("Time");
            }
        }

        private bool isKeyupEvent;
        public bool IsKeyupEvent
        {
            get { return isKeyupEvent; }
            set
            {
                isKeyupEvent = value;
                OnPropertyChanged("IsKeyupEvent");
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

        private Key? key = null;
        public Key? Key
        {
            get { return key; }
            set
            {
                key = value;
                OnPropertyChanged("Key");
                OnPropertyChanged("KeyString");
            }
        }

        [JsonIgnore]
        public string KeyString
        {
            get
            {
                return KeyTextHelper.ConvertKeyText(ModifierKey, Key, "없음");
            }
        }

        [JsonIgnore]
        public ICommand KeyCommand { get; set; }
        [JsonIgnore]
        public ICommand KeySettingCommand { get; set; }

        public TimerKeyItem()
        {
        }

        // 세팅 전용 키 복사
        public TimerKeyItem(TimerKeyItem settingKeyItem)
        {
            Name = settingKeyItem.Name;
            CopyItem(settingKeyItem);
            KeySettingCommand = new RelayCommand(o => KeySettingEvent((Window)o));
        }

        public TimerKeyItem(string name, Action action)
        {
            Name = name;
            KeyCommand = new RelayCommand(o => action());
            KeySettingCommand = new RelayCommand(o => KeySettingEvent((Window)o));
        }

        public TimerKeyItem(string name, Action<int> action, int time)
        {
            Name = name;
            KeyCommand = new RelayCommand(o => action(time));
            KeySettingCommand = new RelayCommand(o => KeySettingEvent((Window)o));
        }

        // 칼로스 타이머 전용
        public TimerKeyItem(string name, Action<float> action, bool isSubtract = false)
        {
            Name = name;
            KeyCommand = new RelayCommand(o => action(isSubtract ? -Time : Time));
            KeySettingCommand = new RelayCommand(o => KeySettingEvent((Window)o));
        }

        private void KeySettingEvent(Window window)
        {
            var dialog = new WindowTimerPressKeyboard();
            var vm = dialog.DataContext as ViewModelTimerPressKeyboard;

            dialog.Left = window.Left + (window.ActualWidth - dialog.Width) / 2;
            dialog.Top = window.Top + (window.ActualHeight - dialog.Height) / 2;

            vm.PressedKey = Key;
            vm.ModifierKey = ModifierKey;
            vm.IsKeyupEvent = IsKeyupEvent;
            vm.ChangeKeyText();

            dialog.ShowDialog();

            Key = vm.PressedKey;
            ModifierKey = vm.ModifierKey;
            IsKeyupEvent = vm.IsKeyupEvent;
        }

        public void CopyItem(TimerKeyItem target)
        {
            Time = target.Time;
            ModifierKey = target.ModifierKey;
            Key = target.Key;
            IsKeyupEvent = target.IsKeyupEvent;
        }
    }
}
