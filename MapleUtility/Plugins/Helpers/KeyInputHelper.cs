using MapleUtility.Plugins.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace MapleUtility.Plugins.Helpers
{
    public class KeyInputHelper
    {
        public static List<Key> PressedKeyList = new List<Key>();

        public static bool CheckPressModifierAndNormalKey(ModifierKeys? ModifierKey, Key? pressKey)
        {
            if (ModifierKey.HasValue)
            {
                if(ModifierKey == ModifierKeys.Control)
                {
                    if (!PressedKeyList.Any(o => o == Key.LeftCtrl || o == Key.RightCtrl))
                        return false;
                }
                if (ModifierKey == ModifierKeys.Alt)
                {
                    if (!PressedKeyList.Any(o => o == Key.LeftAlt || o == Key.RightAlt))
                        return false;
                }
                if (ModifierKey == ModifierKeys.Shift)
                {
                    if (!PressedKeyList.Any(o => o == Key.LeftShift || o == Key.RightShift))
                        return false;
                }
            }

            if (pressKey.HasValue)
            {
                if (PressedKeyList.Any(o => o == pressKey.Value))
                    return true;
                else
                    return false;
            }

            return true;
        }
    }
}
