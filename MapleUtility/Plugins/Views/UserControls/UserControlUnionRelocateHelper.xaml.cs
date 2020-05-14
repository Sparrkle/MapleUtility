using MapleUtility.Plugins.Helpers;
using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace MapleUtility.Plugins.Views.UserControls
{
    /// <summary>
    /// UserControlUnionRelocateHelper.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserControlUnionRelocateHelper : UserControl
    {
        public UserControlUnionRelocateHelper()
        {
            InitializeComponent();

            var a = MapleDataHelper.GetUnionData("Uffiex");
        }
    }
}
