using MapleUtility.Plugins.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace MapleUtility.Plugins.Models
{
    public class KeyItem : Notifier
    {
        private ModifierKeys? modifierKey = null;
        public ModifierKeys? ModifierKey
        {
            get { return modifierKey; }
            set
            {
                modifierKey = value;
                OnPropertyChanged("ModifierKey");
                OnPropertyChanged("KeyString");
            }
        }

        private Key? key = null;
        public Key? Key
        {
            get { return key; }
            set
            {
                key = value;
                OnPropertyChanged("Key");
                OnPropertyChanged("KeyString");
            }
        }

        private List<Key> arrowKeys = new List<Key>();
        public List<Key> ArrowKeys
        {
            get { return arrowKeys; }
            set
            {
                arrowKeys = value;
                OnPropertyChanged("ArrowKeys");
                OnPropertyChanged("KeyString");
            }
        }

        [JsonIgnore]
        public string KeyString
        {
            get
            {
                return KeyTextHelper.ConvertKeyText(ModifierKey, Key, ArrowKeys, "없음");
            }
        }

        public KeyItem()
        {

        }

        public KeyItem(ModifierKeys? modifierKey, Key? key)
        {
            ModifierKey = modifierKey;
            Key = key;
            ArrowKeys = arrowKeys.ToList();
        }

        public KeyItem(ModifierKeys? modifierKey, Key? key, List<Key> arrowKeys) : this(modifierKey, key)
        {
            ArrowKeys = arrowKeys.ToList();
        }

        public KeyItem Clone()
        {
            var clone = new KeyItem();
            clone.ModifierKey = ModifierKey;
            clone.Key = Key;
            clone.ArrowKeys = ArrowKeys.ToList();

            return clone;
        }
    }
}
