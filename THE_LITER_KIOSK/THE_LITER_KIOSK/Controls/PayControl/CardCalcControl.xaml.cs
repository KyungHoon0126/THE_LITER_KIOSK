﻿using System.Windows;
using THE_LITER_KIOSK.UIManager;

namespace THE_LITER_KIOSK.Controls.PayControl
{
    /// <summary>
    /// CardCalcControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CardCalcControl : CustomControlModel
    {
        public CardCalcControl()
        {
            InitializeComponent();
        }

        #region UserControl Transition
        private void btnTablePrev_Click(object sender, RoutedEventArgs e)
        {
            App.uIStateManager.SwitchCustomControl(CustomControlType.PLACE);
        }
        #endregion
    }
}