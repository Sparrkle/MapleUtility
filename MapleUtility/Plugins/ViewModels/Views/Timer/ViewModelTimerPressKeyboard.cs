using MapleUtility.Plugin.Lib;
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
                if (ModifierKey == null && PressedKey == null)
                    return "지정할 단축키를 입력해주세요.";

                var resultText = "";
                if(ModifierKey.HasValue)
                {
                    if (ModifierKey.Value.HasFlag(ModifierKeys.Control))
                        resultText += "Ctrl + ";
                    if (ModifierKey.Value.HasFlag(ModifierKeys.Alt))
                        resultText += "Alt + ";
                    if (ModifierKey.Value.HasFlag(ModifierKeys.Shift))
                        resultText += "Shift + ";
                }

                if (PressedKey == null && resultText.Length >= 2)
                    return resultText.Substring(0, resultText.Length - 2);
                else
                    return resultText + PressedKey.ToString();
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
            Key inputKey = e.Key.Equals(Key.ImeProcessed) ? e.ImeProcessedKey : e.Key;

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
