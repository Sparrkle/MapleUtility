using Newtonsoft.Json;

namespace MapleUtility.Plugins.Models
{
    public class PresetItem : Notifier
    {
        [JsonIgnore]
        private bool isChecked;
        [JsonIgnore]
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
    }
}
