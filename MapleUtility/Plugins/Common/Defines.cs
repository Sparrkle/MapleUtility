using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Common
{
    public static class Defines
    {
        public const int SETTING_SIZE = 2;
        public const string SETTING_PW = "EnosisChruko";
        public static string FilePath = AppDomain.CurrentDomain.BaseDirectory + "MapleUtility.setting";
        public static string ChurukoFilePath = AppDomain.CurrentDomain.BaseDirectory + "Churuko.setting";
        public static string ImageFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Images\\";

        public static int UIBAR_WIDTH = 350;
        public static int UIBAR_HEIGHT = 70;

        public static int HILLA_UIBAR_WIDTH = 320;
        public static int HILLA_UIBAR_HEIGHT = 90;
    }
}
