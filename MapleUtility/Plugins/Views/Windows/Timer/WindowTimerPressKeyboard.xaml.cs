using MapleUtility.Plugins.UserControls;
using MapleUtility.Plugins.ViewModels.Views.Timer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MapleUtility.Plugins.Views.Windows.Timer
{
    /// <summary>
    /// WindowTimerPressKeyboard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowTimerPressKeyboard : UCUniqueWindow
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private static UCUniqueWindow instance;
        public static UCUniqueWindow Instance
        {
            get
            {
                if (instance == null)
                    instance = new WindowTimerPressKeyboard();
                return instance;
            }
            set => instance = value;
        }

        public static bool IsWindowVisible
        {
            get { return Instance == null ? false : Instance.IsVisible; }
        }

        protected override UCUniqueWindow InternalInstance
        {
            get { return Instance; }
            set => Instance = value;
        }

        public WindowTimerPressKeyboard()
        {
            this.DataContext = new ViewModelTimerPressKeyboard();
            InitializeComponent();

            Loaded += WindowTimerPressKeyboard_Loaded;
        }

        void WindowTimerPressKeyboard_Loaded(object sender, RoutedEventArgs e)
        {
            // Code to remove close box from window
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var vm = this.DataContext as ViewModelTimerPressKeyboard;
            vm.PressKeyEvent(e);
        }
    }
}
