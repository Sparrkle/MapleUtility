﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace MapleUtility.Plugins.Helpers
{
    public class KeyTextHelper
    {
        public static string ConvertKeyText(ModifierKeys? modifierKey, Key? pressedKey, List<Key> arrowKeys, string nullText = "")
        {
            if (modifierKey == null && pressedKey == null && arrowKeys?.Count() == 0)
                return nullText;

            var resultText = "";
            if(arrowKeys != null)
            {
                foreach(var arrowKey in arrowKeys)
                {
                    switch(arrowKey)
                    {
                        case Key.Up:
                            resultText += "🡑";
                            break;
                        case Key.Down:
                            resultText += "🡓";
                            break;
                        case Key.Left:
                            resultText += "🡐";
                            break;
                        case Key.Right:
                            resultText += "🡒";
                            break;
                    }
                }
            }

            if (modifierKey.HasValue)
            {
                if (modifierKey.Value.HasFlag(ModifierKeys.Control))
                    resultText += "Ctrl + ";
                if (modifierKey.Value.HasFlag(ModifierKeys.Alt))
                    resultText += "Alt + ";
                if (modifierKey.Value.HasFlag(ModifierKeys.Shift))
                    resultText += "Shift + ";
            }

            if (pressedKey == null && modifierKey.HasValue)
                return resultText.Substring(0, resultText.Length - 2);
            else
            {
                string keyText;

                switch (pressedKey)
                {
                    case Key.Capital:
                        keyText = "CapsLock";
                        break;
                    case Key.Oem3:
                        keyText = "~";
                        break;
                    case Key.D0:
                        keyText = "0";
                        break;
                    case Key.D1:
                        keyText = "1";
                        break;
                    case Key.D2:
                        keyText = "2";
                        break;
                    case Key.D3:
                        keyText = "3";
                        break;
                    case Key.D4:
                        keyText = "4";
                        break;
                    case Key.D5:
                        keyText = "5";
                        break;
                    case Key.D6:
                        keyText = "6";
                        break;
                    case Key.D7:
                        keyText = "7";
                        break;
                    case Key.D8:
                        keyText = "8";
                        break;
                    case Key.D9:
                        keyText = "9";
                        break;
                    case Key.OemMinus:
                        keyText = "-";
                        break;
                    case Key.OemPlus:
                        keyText = "+";
                        break;
                    case Key.Back:
                        keyText = "Backspace";
                        break;
                    case Key.OemOpenBrackets:
                        keyText = "[";
                        break;
                    case Key.Oem6:
                        keyText = "]";
                        break;
                    case Key.Oem5:
                        keyText = "\\";
                        break;
                    case Key.Oem1:
                        keyText = ";";
                        break;
                    case Key.OemQuotes:
                        keyText = "'";
                        break;
                    case Key.OemComma:
                        keyText = ",";
                        break;
                    case Key.OemPeriod:
                        keyText = ".";
                        break;
                    case Key.OemQuestion:
                        keyText = "/";
                        break;
                    case Key.Next:
                        keyText = "PageDown";
                        break;
                    case Key.Divide:
                        keyText = "NumPad/";
                        break;
                    case Key.Multiply:
                        keyText = "NumPad*";
                        break;
                    case Key.Subtract:
                        keyText = "NumPad-";
                        break;
                    case Key.Add:
                        keyText = "NumPad+";
                        break;
                    case Key.Decimal:
                        keyText = "NumPad.";
                        break;
                    case Key.Escape:
                        keyText = "Esc";
                        break;
                    case Key.HanjaMode:
                        keyText = "한자";
                        break;
                    default:
                        keyText = pressedKey.ToString();
                        break;
                }

                return resultText + keyText;
            }
        }
    }
}
