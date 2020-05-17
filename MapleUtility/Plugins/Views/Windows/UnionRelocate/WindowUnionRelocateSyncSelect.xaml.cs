using MapleUtility.Plugins.ViewModels.Views.UnionRelocate;
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

namespace MapleUtility.Plugins.Views.Windows.UnionRelocate
{
    /// <summary>
    /// WindowUnionRelocateSyncSelect.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowUnionRelocateSyncSelect : Window
    {
        public WindowUnionRelocateSyncSelect()
        {
            this.DataContext = new ViewModelUnionRelocateSyncSelect();
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }
    }
}
