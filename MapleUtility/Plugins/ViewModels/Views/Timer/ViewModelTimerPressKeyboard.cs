using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace MapleUtility.Plugins.ViewModels.Views.Timer
{
    public class ViewModelTimerPressKeyboard : Notifier
    {
        public string PrevKeyText
        {
            get
            {
                if (KeyItems.Count == 0)
                    return "";

                return string.Join(", ", KeyItems.Select(o => o.KeyString));
            }
        }

        public string KeyText
        {
            get
            {
                return KeyTextHelper.ConvertKeyText(ModifierKey, PressedKey, ArrowKeys, "지정할 단축키를 입력해주세요.");
            }
        }

        private List<KeyItem> keyItems = new List<KeyItem>();
        public List<KeyItem> KeyItems
        {
            get { return keyItems; }
            set
            {
                keyItems = value;
                OnPropertyChanged("KeyItems");
            }
        }

        public ModifierKeys? ModifierKey = null;
        public Key? PressedKey = null;
        public List<Key> ArrowKeys = new List<Key>();

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

        #region Button Command Variables
        public ICommand AddKeyCommand { get; set; }
        public ICommand KeyClearCommand { get; set; }
        public ICommand KeySaveCommand { get; set; }
        #endregion

        public ViewModelTimerPressKeyboard()
        {
            AddKeyCommand = new RelayCommand(o => AddKeyEvent());
            KeyClearCommand = new RelayCommand(o => KeyClearEvent());
            KeySaveCommand = new RelayCommand(o => KeySaveEvent((Window)o));
        }

        private void AddKeyEvent()
        {
            KeyItems.Add(new KeyItem(ModifierKey, PressedKey, ArrowKeys));
            ModifierKey = null;
            PressedKey = null;
            ArrowKeys.Clear();
            ChangeKeyText();
        }

        private void KeyClearEvent()
        {
            ModifierKey = null;
            PressedKey = null;
            ArrowKeys.Clear();
            KeyItems.Clear();
            ChangeKeyText();
        }

        private void KeySaveEvent(Window window)
        {
            window.Close();
        }

        public void PressKeyEvent(KeyEventArgs e)
        {
            Key inputKey;

            DebugLogHelper.Write(e.Key.ToString() + " 키( + " + e.SystemKey.ToString() + ")를 바인딩 시도합니다.");

            if (e.SystemKey != Key.None)
                inputKey = e.SystemKey;
            else
                inputKey = e.Key.Equals(Key.ImeProcessed) ? e.ImeProcessedKey : e.Key;

            bool isArrowKey = (inputKey == Key.Up || inputKey == Key.Down || inputKey == Key.Left || inputKey == Key.Right) && !isDisableCommand;

            if(isArrowKey)
            {
                if (PressedKey != null || ModifierKey != null)
                    ArrowKeys.Clear();

                ArrowKeys.Add(inputKey);
                PressedKey = null;
                ModifierKey = null;
            }
            else
            {
                var prevPressedKey = PressedKey;

                if (!(inputKey == Key.LeftCtrl) && !(inputKey == Key.LeftAlt) && !(inputKey == Key.LeftShift)
                && !(inputKey == Key.RightCtrl) && !(inputKey == Key.RightAlt) && !(inputKey == Key.RightShift)
                && !(inputKey == Key.LWin) && !(inputKey == Key.RWin) && !(inputKey == Key.KanaMode))
                {
                    if (PressedKey != null && (PressedKey != inputKey || ModifierKey != null))
                        ArrowKeys.Clear();
                    PressedKey = inputKey;
                }
                else
                    PressedKey = null;

                if (!e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Windows) && e.KeyboardDevice.Modifiers != ModifierKeys.None)
                {
                    if ((ModifierKey != null && !e.KeyboardDevice.Modifiers.HasFlag(ModifierKey)) || (prevPressedKey != null && prevPressedKey != PressedKey))
                        ArrowKeys.Clear();
                    ModifierKey = e.KeyboardDevice.Modifiers;
                }
                else
                    ModifierKey = null;
            }

            ChangeKeyText();
        }

        public void ChangeKeyText()
        {
            OnPropertyChanged("PrevKeyText");
            OnPropertyChanged("KeyText");
        }
    }
}
