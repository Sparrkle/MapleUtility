using MapleUtility.Plugins.ViewModels.UserControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace MapleUtility.Plugins.Views.UserControls
{
    /// <summary>
    /// UserControlTimerHelper.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserControlVerusHillaHelper : UserControl
    {
        public UserControlVerusHillaHelper()
        {
            this.DataContext = new ViewModelUCVerusHillaHelper();
            InitializeComponent();
        }
    }
}
