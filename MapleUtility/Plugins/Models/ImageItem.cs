using MapleUtility.Plugins.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Models
{
    public class ImageItem : Notifier
    {
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

        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                OnPropertyChanged("FileName");
                OnPropertyChanged("DisplayedImage");
            }
        }

        [JsonIgnore]
        public string DisplayedImage
        {
            get { return Defines.ImageFolderPath + FileName; }
        }

        public ImageItem (string fileName)
        {
            Name = Path.GetFileNameWithoutExtension(fileName);
            FileName = fileName;
        }
    }
}
