using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.ViewModels.UserControls;
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
            this.DataContext = new ViewModelUCUnionRelocateHelper();
            InitializeComponent();

            // 바인딩 안되는 오류 있음. 강제 설정
            cbCharacterAll.DataContext = this.DataContext;
        }
    }
}
