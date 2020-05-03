using System;
using System.Windows;

namespace MapleUtility.Plugins.UserControls
{
    public abstract class UCUniqueWindow : Window
    {
        protected abstract UCUniqueWindow InternalInstance
        {
            get; set;
        }

        protected UCUniqueWindow()
        {
            Closed += UniqueWindow_Closed;
        }

        private void UniqueWindow_Closed(object sender, EventArgs e)
        {
            InternalInstance = null;
        }

        public new void Show()
        {
            if (!InternalInstance.IsVisible)
                base.Show();

            if (InternalInstance.WindowState == WindowState.Minimized)
                InternalInstance.WindowState = WindowState.Normal;

            InternalInstance.Activate();
            InternalInstance.Topmost = true;
            InternalInstance.Topmost = false;
            InternalInstance.Focus();
        }
    }
}
