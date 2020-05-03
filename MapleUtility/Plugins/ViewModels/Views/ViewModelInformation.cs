using MapleUtility.Plugins.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.ViewModels.Views
{
    public class ViewModelInformation
    {
        private int ClickCount = 0;

        public void ClickEasterEggEvent()
        {
            var debugWindow = WindowDebug.Instance;
            if (debugWindow.IsLoaded)
                return;

            ClickCount++;
            if (ClickCount >= 5)
            {
                debugWindow.Show();
                ClickCount = 0;
            }
        }
    }
}
