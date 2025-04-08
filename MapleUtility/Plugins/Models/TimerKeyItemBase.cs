using MapleUtility.Plugins.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace MapleUtility.Plugins.Models
{
    public abstract class TimerKeyItemBase : Notifier
    {
        private List<KeyItem> keyItems = new List<KeyItem>();
        public List<KeyItem> KeyItems
        {
            get { return keyItems; }
            set
            {
                keyItems = value;
                OnPropertyChanged("KeyItems");
                OnPropertyChanged("KeyString");
            }
        }

        [JsonIgnore]
        public string KeyString
        {
            get
            {
                if (KeyItems.Count() == 0)
                    return "없음";
                return string.Join(", ", KeyItems.Select(o => o.KeyString));
            }
        }

        public abstract TimerKeyItemBase Copy();

        public void AddKeyItem(KeyItem keyItem)
        {
            KeyItems.Add(keyItem);
            OnPropertyChanged("KeyString");
        }
    }
}
