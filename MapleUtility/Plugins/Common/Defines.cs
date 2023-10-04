using System;
using System.Diagnostics;
using System.Reflection;

namespace MapleUtility.Plugins.Common
{
    public static class Defines
    {
        public const int SETTING_SIZE = 2;
        public const string SETTING_PW = "EnosisChruko";
        public static string FilePath = AppDomain.CurrentDomain.BaseDirectory + "MapleUtility.setting";
        public static string ChurukoFilePath = AppDomain.CurrentDomain.BaseDirectory + "Churuko.setting";
        public static string ImageFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Images\\";

        public static string Version
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion.ToString();
            }
        }
    }
}
