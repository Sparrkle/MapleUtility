using MapleUtility.Plugins.ViewModels.Views;
using MapleUtility.Plugins.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Helpers
{
    public static class DebugLogHelper
    {
        public static void Write(string text)
        {
            var window = WindowDebug.Instance;
            if(window.IsLoaded)
            {
                var vm = window.DataContext as ViewModelDebug;
                vm.DebugTextList.Add(text);
            }
        }
    }
}
