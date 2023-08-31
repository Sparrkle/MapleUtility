using MapleUtility.Plugins.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace MapleUtility.Plugins.Helpers
{
    public class KeyInputHelper
    {
        public static ModifierKeys GetModifierKeys(Keys key)
        {
            ModifierKeys keys = ModifierKeys.None;

            if (key.HasFlag(Keys.Control))
                keys |= ModifierKeys.Control;
            if (key.HasFlag(Keys.Alt))
                keys |= ModifierKeys.Alt;
            if (key.HasFlag(Keys.Shift) || key.HasFlag(Keys.ShiftKey) || key.HasFlag(Keys.LShiftKey) || key.HasFlag(Keys.RShiftKey))
                keys |= ModifierKeys.Shift;

            return keys;
        }

        public static bool CheckPressModifierAndNormalKey(ModifierKeys inputModifierKeys, Key inputKey, ModifierKeys? modifierKey, Key? pressKey)
        {
            if (modifierKey.HasValue)
            {
                if ((modifierKey & inputModifierKeys) != modifierKey)
                    return false;
            }

            if (pressKey.HasValue)
            {
                if (inputKey == pressKey.Value)
                    return true;
                else
                    return false;
            }

            return true;
        }
    }
}
