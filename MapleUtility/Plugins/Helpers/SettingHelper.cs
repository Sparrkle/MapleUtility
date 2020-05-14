using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

namespace MapleUtility.Plugins.Helpers
{
    public class SettingHelper
    {

        public static bool SaveSettingFile(SettingItem settingItem)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(settingItem, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                var encryptBytes = AES256EncryptHelper.AESEncrypt256(jsonString, Defines.SETTING_PW);
                File.WriteAllBytes(Defines.FilePath, encryptBytes);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static SettingItem LoadSettingFile()
        {
            try
            {
                var encryptByes = File.ReadAllBytes(Defines.FilePath);
                var decryptString = AES256EncryptHelper.AESDecrypt256(encryptByes, Defines.SETTING_PW);

                var settingObjects = JsonConvert.DeserializeObject<SettingItem>(decryptString, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                return settingObjects;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool SaveSettingChurukoLab(DateTime? lastestDate)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(lastestDate);
                var encryptBytes = AES256EncryptHelper.AESEncrypt256(jsonString, Defines.SETTING_PW);
                File.WriteAllBytes(Defines.ChurukoFilePath, encryptBytes);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static DateTime? LoadSettingChurukoLab()
        {
            try
            {
                var encryptByes = File.ReadAllBytes(Defines.ChurukoFilePath);
                var decryptString = AES256EncryptHelper.AESDecrypt256(encryptByes, Defines.SETTING_PW);

                var churukoObjects = JsonConvert.DeserializeObject<DateTime?>(decryptString);
                return churukoObjects;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class SettingItem
    {
        public ObservableCollection<TimerItem> TimerList;
        public ObservableCollection<PresetItem> PresetList;
        public ObservableCollection<ImageItem> ImageList;
        public ObservableCollection<SoundItem> SoundList;
        public ObservableCollection<ColumnItem> ColumnList;
        public PresetItem SelectedPreset;
        public Color RemainSquareColor = Color.FromArgb(88, 0, 50, 100);
        public float RemainBackAlpha = 78;
        public int AlertDuration = 2000;
        public bool IsShowUIBarTimerName = true;
        public bool IsAlertShowScreenChecked = false;
        public bool IsTimerResetChecked = false;
        public Key? TimerOnOffKey = null;
        public ModifierKeys? TimerOnOffModifierKey = null;
        public Key? PauseAllKey = null;
        public ModifierKeys? PauseAllModifierKey = null;
        public Key? TimerLockKey = null;
        public ModifierKeys? TimerLockModifierKey = null;

        public int UIBAR_WIDTH = 400;
        public int UIBAR_HEIGHT = 110;
    }
}
