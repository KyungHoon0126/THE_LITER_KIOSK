﻿using System.Threading.Tasks;
using System.Windows;
using THE_LITER_KIOSK.UIManager;

namespace THE_LITER_KIOSK.Controls.HomeControl
{
    /// <summary>
    /// HomeControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HomeControl : CustomControlModel
    {
        public HomeControl()
        {
            InitializeComponent();
        }

        #region UserControl Transition
        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            App.orderData.orderViewModel.IsEnabledOrderAndClearAllMenuBtn();
            App.uIStateManager.SwitchCustomControl(CustomControlType.ORDER);
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            Parallel.Invoke(() =>
            {
                App.adminData.SetStatisticData();
                App.adminData.LoadChartData();
            });
            App.memberData.GetAllMemberData();
            App.uIStateManager.SwitchCustomControl(CustomControlType.ADMIN);
        }
        #endregion
    }
}
