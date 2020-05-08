using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Helpers;
using System.Windows;
using System.Windows.Input;

namespace MapleUtility.Plugins.ViewModels.Views.Timer
{
    public class ViewModelTimerPressKeyboard : Notifier
    {
        public string KeyText
        {
            get
            {
                return KeyTextHelper.ConvertKeyText(ModifierKey, PressedKey, "지정할 단축키를 입력해주세요.");
            }
        }

        public ModifierKeys? ModifierKey = null;
        public Key? PressedKey = null;

        #region Button Command Variables
        public ICommand KeyClearCommand { get; set; }
        public ICommand KeySaveCommand { get; set; }
        #endregion

        public ViewModelTimerPressKeyboard()
        {
            KeyClearCommand = new RelayCommand(o => KeyClearEvent());
            KeySaveCommand = new RelayCommand(o => KeySaveEvent((Window)o));
        }

        private void KeyClearEvent()
        {
            ModifierKey = null;
            PressedKey = null;
            OnPropertyChanged("KeyText");
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

            if (!(inputKey == Key.LeftCtrl) && !(inputKey == Key.LeftAlt) && !(inputKey == Key.LeftShift)
                && !(inputKey == Key.RightCtrl) && !(inputKey == Key.RightAlt) && !(inputKey == Key.RightShift)
                && !(inputKey == Key.LWin) && !(inputKey == Key.RWin) && !(inputKey == Key.KanaMode))
                PressedKey = inputKey;
            else
                PressedKey = null;

            if (!e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Windows) && e.KeyboardDevice.Modifiers != ModifierKeys.None)
                ModifierKey = e.KeyboardDevice.Modifiers;
            else
                ModifierKey = null;

            ChangeKeyText();
        }

        public void ChangeKeyText()
        {
            OnPropertyChanged("KeyText");
        }
    }
}
