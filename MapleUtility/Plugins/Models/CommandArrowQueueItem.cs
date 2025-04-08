using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapleUtility.Plugins.Models
{
    public class CommandArrowQueueItem
    {
        public TimeSpan COMBO_RECOGNIZE_TIME = new TimeSpan(0, 0, 0, 0, 1000);

        private List<CommandArrowItem> Queue = new List<CommandArrowItem>();

        public void Push(Key key)
        {
            var command = new CommandArrowItem
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

        public bool Contains(KeyItem item)
        {
            if (item.ArrowKeys.Count() == 0)
                return false;

            if (item.ArrowKeys.Count() > Queue.Count())
                return false;

            var index = Queue.Count() - 1;
            for (int i = item.ArrowKeys.Count() - 1; i >= 0; i--, index--)
            {
                if (item.ArrowKeys[i] != Queue[index].Key)
                    return false;
            }

            return true;
        }
    }
}
