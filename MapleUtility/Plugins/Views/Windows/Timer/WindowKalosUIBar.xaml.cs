using MapleUtility.Plugins.UserControls;
using MapleUtility.Plugins.ViewModels.Views.Timer;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MapleUtility.Plugins.Views.Windows.Timer
{
    /// <summary>
    /// WindowKalosUIBar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowKalosUIBar : UCUniqueWindow
    {
        private static UCUniqueWindow instance;
        public static UCUniqueWindow Instance
        {
            get
            {
                if (instance == null)
                    instance = new WindowKalosUIBar();
                return instance;
            }
            set => instance = value;
        }

        protected override UCUniqueWindow InternalInstance
        {
            get { return Instance; }
            set => Instance = value;
        }

        public WindowKalosUIBar()
        {
            InitializeComponent();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
