using System;
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
            Dispatcher.Invoke(() =>
            {
                currentType = Type;
                BusyContent = Content;
                UpdateEvent();
            }, DispatcherPriority.Background);
        }

        public void UpdateEvent()
        {
            if (IsCanceled)
                throw new OperationCanceledException("Cancel Progress");

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
            if (Height == 200)
                Height = 400;
            else
                Height = 200;
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
