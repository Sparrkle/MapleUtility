﻿using MapleUtility.Plugins.ViewModels.Views;
using System;
using System.Collections.Generic;
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

namespace MapleUtility.Plugins.Views.Windows.Timer
{
    /// <summary>
    /// WindowTimerSettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowTimerSettingWindow : Window
    {
        public WindowTimerSettingWindow()
        {
            this.DataContext = new ViewModelSettingWindow();
            InitializeComponent();

            // 바인딩 안되는 오류 있음. 강제 설정
            ucPreset.cbPresetAll.DataContext = this.DataContext;
            ucSound.cbSoundAll.DataContext = this.DataContext;
        }
    }
}
