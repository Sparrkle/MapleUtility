using MapleUtility.Plugins.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapleUtility.Plugins.Views.Windows
{
    /// <summary>
    /// WindowChurukoLab.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowChurukoLab : Window
    {
        public WindowChurukoLab()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var now = DateTime.Now;
            SettingHelper.SaveSettingChurukoLab(new DateTime(now.Year, now.Month, now.Day));
            this.Close();
        }
    }
}
