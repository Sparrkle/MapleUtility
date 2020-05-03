using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Models;
using MapleUtility.Plugins.UserControls;
using MapleUtility.Plugins.ViewModels.UserControls;
using MapleUtility.Plugins.ViewModels.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MapleUtility.Plugins.Views.Windows
{
    /// <summary>
    /// WindowMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowDebug : UCUniqueWindow
    {
        private static UCUniqueWindow instance;
        public static UCUniqueWindow Instance
        {
            get
            {
                if (instance == null)
                    instance = new WindowDebug();
                return instance;
            }
            set => instance = value;
        }

        protected override UCUniqueWindow InternalInstance
        {
            get { return Instance; }
            set => Instance = value;
        }

        public WindowDebug()
        {
            var vm = new ViewModelDebug();
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
