﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
            
namespace THE_LITER_KIOSK.Controls.HomeControl
{
    /// <summary>
    /// HomeControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HomeControl : UserControl
    {
        public HomeControl()
        {
            InitializeComponent();
            MedaiElementVideoPlayer();
        }

        private void MedaiElementVideoPlayer()
        {
            Console.WriteLine(media.Position);
            media.Stop();
            media.Position = TimeSpan.FromSeconds(0);
            media.Play();
        }

       

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("OrderButton");
        }
    }
}
