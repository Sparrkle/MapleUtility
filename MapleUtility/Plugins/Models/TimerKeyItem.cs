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
    public class TimerKeyItem : TimerKeyItemBase
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

        private bool isDisableCommand;
        public bool IsDisableCommand
        {
            get { return isDisableCommand; }
            set
            {
                isDisableCommand = value;
                OnPropertyChanged("IsDisableCommand");
            }
        }

        //이전버전 호환
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
        public ICommand KeyCommand { get; set; }
        [JsonIgnore]
        public ICommand KeySettingCommand { get; set; }

        public TimerKeyItem()
        {

        }

        public TimerKeyItem(string name)
        {
            Name = name;
            KeySettingCommand = new RelayCommand(o => KeySettingEvent((Window)o));
        }

        public TimerKeyItem(string name, Action action) : this(name)
        {
            KeyCommand = new RelayCommand(o => action());
        }

        public TimerKeyItem(string name, Action<int> action, int time) : this(name)
        {
            KeyCommand = new RelayCommand(o => action(time));
        }

        // 칼로스 타이머 전용
        public TimerKeyItem(string name, Action<float> action, bool isSubtract = false) : this(name)
        {
            KeyCommand = new RelayCommand(o => action(isSubtract ? -Time : Time));
        }

        private void KeySettingEvent(Window window)
        {
            var dialog = WindowTimerPressKeyboard.Instance as WindowTimerPressKeyboard;
            var vm = dialog.DataContext as ViewModelTimerPressKeyboard;

            dialog.Left = window.Left + (window.ActualWidth - dialog.Width) / 2;
            dialog.Top = window.Top + (window.ActualHeight - dialog.Height) / 2;

            if (KeyItems.Count() > 0)
            {
                var firstKey = KeyItems.FirstOrDefault().Clone();
                vm.ModifierKey = firstKey.ModifierKey;
                vm.PressedKey = firstKey.Key;
                vm.ArrowKeys = firstKey.ArrowKeys;

                vm.KeyItems = KeyItems.Skip(1).Select(o => o.Clone()).ToList();
            }
            vm.IsKeyupEvent = IsKeyupEvent;
            vm.IsDisableCommand = IsDisableCommand;
            vm.ChangeKeyText();

            dialog.ShowDialog();

            if(vm.ModifierKey != null || vm.PressedKey != null || vm.ArrowKeys.Count() > 0)
                vm.KeyItems.Add(new KeyItem(vm.ModifierKey, vm.PressedKey, vm.ArrowKeys));

            KeyItems = vm.KeyItems.Select(o => o.Clone()).ToList();
            IsKeyupEvent = vm.IsKeyupEvent;
            IsDisableCommand = vm.IsDisableCommand;
        }

        public override TimerKeyItemBase Copy()
        {
            var clone = new TimerKeyItem(Name);
            clone.Time = Time;
            clone.KeyItems = KeyItems.Select(o => o.Clone()).ToList();
            clone.IsKeyupEvent = IsKeyupEvent;

            // 구버전 호환
            if(ModifierKey != null || Key != null)
                clone.AddKeyItem(new KeyItem(ModifierKey, Key));

            return clone;
        }
    }
}
