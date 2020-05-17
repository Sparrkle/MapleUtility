using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.UserControls;

namespace MapleUtility.Plugins.Views.Windows
{
    /// <summary>
    /// Interaction logic for WindowProgress.xaml
    /// </summary>
    public partial class WindowProgress : UCUniqueWindow
    {
        private static UCUniqueWindow instance;
        public static UCUniqueWindow Instance
        {
            get
            {
                if (instance == null)
                    instance = new WindowProgress();
                return instance;
            }
            set => instance = value;
        }

        protected override UCUniqueWindow InternalInstance
        {
            get { return Instance; }
            set => Instance = value;
        }

        public BackgroundWorker Worker;
        public bool IsCanceled = false;
        public DateTime lastUpdateTime;
        private ProgressType currentType;
        private string BusyContent;

        public WindowProgress()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            IsCanceled = false;
            lastUpdateTime = DateTime.Now;
            BusyContent = "";
        }

        public void Update(ProgressType Type, string Content)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    currentType = Type;
                    BusyContent = Content;
                    UpdateEvent();
                }, DispatcherPriority.Background);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public void UpdateEvent()
        {
            if (IsCanceled)
                Worker.CancelAsync();

            string resultContent;
            if (currentType == ProgressType.UPDATE)
                resultContent = BusyContent;
            else
            {
                resultContent = (currentType == ProgressType.START ? "Start " : "End ") + BusyContent;
                lbLog.Items.Add(DateTime.Now.ToString("[yy/MM/dd HH:mm:ss] - ") + resultContent);
            }

            Indicator.BusyContent = resultContent;
            lastUpdateTime = DateTime.Now;
        }

        private void Detail_Click(object sender, RoutedEventArgs e)
        {
            if (Height == 220)
                Height = 420;
            else
                Height = 220;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var resultContent = DateTime.Now.ToString("[yy/MM/dd HH:mm:ss] - ") + "User Canceled..";
                Indicator.BusyContent = resultContent;
                lbLog.Items.Add(resultContent);
                IsCanceled = true;
            }, DispatcherPriority.Background);
        }
    }
}
