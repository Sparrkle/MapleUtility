using FMUtils.KeyboardHook;
using MapleUtility.Plugins.Models;
using MapleUtility.Plugins.Views.Windows;
using System;
using System.Threading;
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

        #region Constants and Fields

        /// <summary>The event mutex name.</summary>
        private const string UniqueEventName = "{4566FA72-DB67-44DC-96D0-8B817426A0C3}";

        /// <summary>The unique mutex name.</summary>
        private const string UniqueMutexName = "{66889730-06FC-4177-9CAD-D7839A6A8A24}";

        /// <summary>The event wait handle.</summary>
        private EventWaitHandle eventWaitHandle;

        /// <summary>The mutex.</summary>
        private Mutex mutex;

        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            bool isOwned;
            this.mutex = new Mutex(true, UniqueMutexName, out isOwned);
            this.eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);

            // So, R# would not give a warning that this variable is not used.
            GC.KeepAlive(this.mutex);

            if (isOwned)
            {
                // Spawn a thread which will be waiting for our event
                var thread = new Thread(
                    () =>
                    {
                        while (this.eventWaitHandle.WaitOne())
                        {
                            Current.Dispatcher.BeginInvoke(
                                (Action)(() => ((WindowMain)Current.MainWindow).BringToForeground()));
                        }
                    });

                // It is important mark it as background otherwise it will prevent app from exiting.
                thread.IsBackground = true;

                thread.Start();
                return;
            }

            // Notify other instance so it could bring itself to foreground.
            this.eventWaitHandle.Set();

            // Terminate this instance.
            this.Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            App.ni.Visible = false;
            App.ni.Dispose();
        }
    }
}
