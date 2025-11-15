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

        // 구버전 호환
        private Key? key = null;
        public Key? Key
        {
            get { return key; }
            set
            {
                key = value;
                OnPropertyChanged("Key");
                OnPropertyChanged("KeyString");

                if(value != null)
                    KeyDataMigration();
            }
        }

        // 구버전 호환
        private List<Key> arrowKeys = new List<Key>();
        public List<Key> ArrowKeys
        {
            get { return arrowKeys; }
            set
            {
                arrowKeys = value;
                OnPropertyChanged("ArrowKeys");
                OnPropertyChanged("KeyString");
                KeyDataMigration();
            }
        }

        private List<Key> pressedKeys = new List<Key>();
        public List<Key> PressedKeys
        {
            get { return pressedKeys; }
            set
            {
                pressedKeys = value;
                OnPropertyChanged("PressedKeys");
                OnPropertyChanged("KeyString");
            }
        }

        [JsonIgnore]
        public string KeyString
        {
            get
            {
                return KeyTextHelper.ConvertKeyText(ModifierKey, PressedKeys, "없음");
            }
        }

        public KeyItem()
        {

        }

        public KeyItem(ModifierKeys? modifierKey, List<Key> pressedKeys)
        {
            ModifierKey = modifierKey;
            PressedKeys = pressedKeys.ToList();
        }

        // 구버전 호환
        public KeyItem(ModifierKeys? modifierKey, Key? key)
        {
            ModifierKey = modifierKey;

            if (PressedKeys == null)
                PressedKeys = new List<Key>();

            if (key != null)
                PressedKeys.Add(key.Value);
        }

        // 구버전 호환
        public KeyItem(ModifierKeys? modifierKey, Key? key, List<Key> arrowKeys) : this(modifierKey, key)
        {
            PressedKeys = arrowKeys.ToList();
        }

        public KeyItem Clone()
        {
            var clone = new KeyItem();
            clone.ModifierKey = ModifierKey;
            clone.PressedKeys = PressedKeys.ToList();

            return clone;
        }

        public void KeyDataMigration()
        {
            if (arrowKeys.Count() > 0)
                PressedKeys = arrowKeys.ToList();
            else
                PressedKeys = new List<Key>();

            if (key != null)
                PressedKeys.Add(key.Value);

            ArrowKeys.Clear();
            Key = null;
        }
    }
}
