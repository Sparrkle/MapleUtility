using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
                File.WriteAllText(Defines.FilePath, jsonString);

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
                var decryptString = File.ReadAllText(Defines.FilePath);
                var settingObjects = JsonConvert.DeserializeObject<SettingItem>(decryptString, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                return settingObjects;
            }
            catch (Exception)
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
        }

        public static bool SaveSettingChurukoLab(DateTime? lastestDate)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(lastestDate);
                File.WriteAllText(Defines.ChurukoFilePath, jsonString);

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
                var decryptString = File.ReadAllText(Defines.ChurukoFilePath);
                var churukoObjects = JsonConvert.DeserializeObject<DateTime?>(decryptString);
                return churukoObjects;
            }
            catch (Exception)
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
    }

    public class SettingItem
    {
        public ObservableCollection<TimerItem> TimerList;
        public ObservableCollection<PresetItem> PresetList;
        public ObservableCollection<ImageItem> ImageList;
        public ObservableCollection<SoundItem> SoundList;
        public ObservableCollection<ColumnItem> ColumnList;
        //public ObservableCollection<CharacterItem> CharacterList;
        //public ObservableCollection<CapturePriorityItem> CapturePriorityList;
        //public BlockManagerItem BlockManager;
        public PresetItem SelectedPreset;
        public string SelectedUIBarStyle = "스택형";
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

        public Key? BackKey = null;
        public ModifierKeys? BackModifierKey = null;
        public Key? ScytheKey = null;
        public ModifierKeys? ScytheModifierKey = null;
        public Key? NextKey = null;
        public ModifierKeys? NextModifierKey = null;

        public List<TimerKeyItem> MainTimer_KeyItems { get; set; } = null;
        public List<TimerKeyItem> HillaTimer_KeyItems { get; set; } = null;
        public List<TimerKeyItem> KALOS_InstanceKeyItems { get; set; } = null;

        public int? UIBarFontSize;
        public string SelectedUIBarFontName;

        public float UIBAR_TRANSPARENCY = 19;
        public int UIBAR_WIDTH = 400;
        public int UIBAR_HEIGHT = 110;

        public int UIBAR_TOP;
        public int UIBAR_LEFT;

        public float HILLA_UIBAR_TRANSPARENCY = 19;
        public int HILLA_UIBAR_WIDTH = 320;
        public int HILLA_UIBAR_HEIGHT = 90;

        public int HILLA_UIBAR_TOP;
        public int HILLA_UIBAR_LEFT;
        public float HILLA_VOLUME = 100;

        public float KALOS_UIBAR_TRANSPARENCY = 19;
        public int KALOS_UIBAR_WIDTH = 320;
        public int KALOS_UIBAR_HEIGHT = 90;

        public int KALOS_UIBAR_TOP;
        public int KALOS_UIBAR_LEFT;
        public float KALOS_INSTANT_VOLUME = 100;

        public bool IsTray = false;
        public bool IsShowUIBar = false;
        public bool IsShowHillaUIBar = false;
        public bool IsShowKalosUIBar = false;

        //public RankItem SelectedRank = null;
    }
}
