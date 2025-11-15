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

        public static bool CheckPressModifierAndNormalKey(CommandQueueItem commandQueueItem, ModifierKeys inputModifierKeys, TimerKeyItemBase timerKeyItem)
        {
            if (timerKeyItem.KeyItems.Count() == 0)
                return false;

            foreach(var keyItem in timerKeyItem.KeyItems)
            {
                var modifierKey = keyItem.ModifierKey;

                if (!commandQueueItem.Contains(keyItem, inputModifierKeys))
                    continue;

                return true;
            }

            return false;
        }
    }
}
