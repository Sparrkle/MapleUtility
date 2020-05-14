using MapleUtility.Plugins.ViewModels.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    /// WindowInformation.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowInformation : Window
    {
        public WindowInformation()
        {
            this.DataContext = new ViewModelInformation();
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var vm = this.DataContext as ViewModelInformation;
            vm.ClickEasterEggEvent();
        }
    }
}
