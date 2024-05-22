using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MapleUtility.Plugins.Models
{
    public class SoundItem : Notifier, IDisposable
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

        private int priority = -1;
        public int Priority
        {
            get { return priority; }
            set
            {
                priority = value;
                OnPropertyChanged("Priority");
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

        private string path;
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                OnPropertyChanged("Path");
            }
        }

        private bool isInternalSound = true;
        public bool IsInternalSound
        {
            get { return isInternalSound; }
            set
            {
                isInternalSound = value;
                OnPropertyChanged("IsInternalSound");
            }
        }

        private bool isDisposed = false;
        public bool IsDisposed
        {
            get { return isDisposed; }
            set
            {
                isDisposed = value;
                OnPropertyChanged("IsDisposed");
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
