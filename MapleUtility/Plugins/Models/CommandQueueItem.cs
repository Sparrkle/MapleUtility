using EnumsNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace MapleUtility.Plugins.Models
{
    public class CommandQueueItem
    {
        public TimeSpan COMBO_RECOGNIZE_TIME = new TimeSpan(0, 0, 0, 0, 1000);

        private List<CommandItem> Queue = new List<CommandItem>();

        public void Push(Key key)
        {
            var command = new CommandItem
            {
                PressedTime = DateTime.Now.Add(COMBO_RECOGNIZE_TIME),
                Key = key
            };
            Queue.Add(command);
        }

        public void Update()
        {
            Queue.RemoveAll(o => o.PressedTime <= DateTime.Now);
        }

        public bool Contains(KeyItem item, ModifierKeys modifierKey)
        {
            if (item.PressedKeys.Count() == 0 && item.ModifierKey == null)
                return false;

            var findIndex = -1;
            var index = Queue.Count() - 1;
            for (int i = item.PressedKeys.Count() - 1; i >= 0; index--)
            {
                if (item.PressedKeys[i] == Queue[index].Key)
                    findIndex = index;
                else
                    return false;

                if (findIndex >= 0)
                    i--;
                if (index == 0)
                    break;
            }

            if (item.PressedKeys.Count() > 0 && findIndex == -1)
                return false;

            if(item.ModifierKey != null)
            {
                var itemModifierKey = item.ModifierKey.Value;
                if (findIndex == 0) // PressedKey는 있는데 매칭할게 없으니 false
                    return false;
                else if(findIndex == -1) // PressedKey는 없으니 처음부터 찾아야함
                    index = Queue.Count() - 1;
                else
                    index = findIndex - 1;

                for(; index >= 0; index--)
                {
                    switch(Queue[index].Key)
                    {
                        case Key.LeftCtrl:
                        case Key.RightCtrl:
                            itemModifierKey = ~ModifierKeys.Control & itemModifierKey;
                            break;
                        case Key.LeftAlt:
                        case Key.RightAlt:
                            itemModifierKey = ~ModifierKeys.Alt & itemModifierKey;
                            break;
                        case Key.LeftShift:
                        case Key.RightShift:
                            itemModifierKey = ~ModifierKeys.Shift & itemModifierKey;
                            break;
                        case Key.LWin:
                        case Key.RWin:
                            itemModifierKey = ~ModifierKeys.Windows & itemModifierKey;
                            break;
                        default:
                            return false;
                    }

                    if (!itemModifierKey.HasAnyFlags())
                        break;
                }

                if (itemModifierKey.HasAnyFlags())
                    return false;
            }

            return true;
        }
    }
}
