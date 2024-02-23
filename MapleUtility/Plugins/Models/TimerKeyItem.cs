using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.ViewModels.Views.Timer;
using MapleUtility.Plugins.Views.Windows.Timer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MapleUtility.Plugins.Models
{
    public class TimerKeyItem : Notifier
    {
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

        public TimerKeyItem(Action action)
        {
            KeyCommand = new RelayCommand(o => action());
            KeySettingCommand = new RelayCommand(o => KeySettingEvent((Window)o));
        }

        public TimerKeyItem(Action<float> action, bool isSubtract = false)
        {
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
            vm.ChangeKeyText();

            dialog.ShowDialog();

            Key = vm.PressedKey;
            ModifierKey = vm.ModifierKey;
        }
    }
}
