using FMUtils.KeyboardHook;
using MapleUtility.Plugins.Models;
using System.Windows;
using System.Windows.Forms;
using Telerik.Windows.Controls;

namespace MapleUtility
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static Hook KeyboardHook = new Hook("Global Keyboard Hook");
        public static BlockManagerItem BlockManager = new BlockManagerItem();
        internal static NotifyIcon ni = new NotifyIcon();

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            App.ni.Visible = false;
            App.ni.Dispose();
        }
    }
}
