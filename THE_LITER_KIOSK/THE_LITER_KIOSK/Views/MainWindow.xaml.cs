﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using THE_LITER_KIOSK.UIManager;

namespace THE_LITER_KIOSK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            #region DispatcherTimer
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
            #endregion

            LoadData();
            SetCustomControls();
            SetStartCustomControl();

            App.adminData.adminViewModel.StartTime = DateTime.Now;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            tbClock.Text = DateTime.Now.ToString("tt H시 mm분 ss초 dddd");
        }

        private void LoadData()
        {
            App.orderData.LoadData();
            App.placeData.LoadTableData();
        }

        private void SetCustomControls()
        {
            App.uIStateManager.SetCustomCtrl(CtrlAdmin, CustomControlType.ADMIN);
            App.uIStateManager.SetCustomCtrl(CtrlHome, CustomControlType.HOME);
            App.uIStateManager.SetCustomCtrl(CtrlTable, CustomControlType.TABLE);
            App.uIStateManager.SetCustomCtrl(CtrlOrder, CustomControlType.ORDER);
            App.uIStateManager.SetCustomCtrl(CtrlPlace, CustomControlType.PLACE);
            App.uIStateManager.SetCustomCtrl(CtrlPaySelect, CustomControlType.PAY);
            App.uIStateManager.SetCustomCtrl(CtrlPayCash, CustomControlType.PAYCASH);
            App.uIStateManager.SetCustomCtrl(CtrlPayCard, CustomControlType.PAYCARD);
            App.uIStateManager.SetCustomCtrl(CtrlPayComplete, CustomControlType.PAYCOMPLETE);
            App.uIStateManager.SetCustomCtrl(CtrlLogin, CustomControlType.LOGIN);
            App.uIStateManager.SetCustomCtrl(CtrlSignup, CustomControlType.SIGNUP);
        }

        private void SetStartCustomControl()
        {
            App.uIStateManager.PushCustomCtrl(CtrlLogin);
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            if (App.orderData.orderViewModel.IsOrderedMenuListValid())
            {
                MessageBoxResult result = MessageBox.Show("주문을 취소하시겠습니까?", "Order", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        MoveOrderToHome();
                        break;
                    case MessageBoxResult.No:
                        return;
                }
            }
            
            MoveOrderToHome();
        }

        private void MoveOrderToHome()
        {
            App.orderData.InitData();
            App.uIStateManager.SwitchCustomControl(CustomControlType.HOME);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (App.uIStateManager.customCtrlStack.Count > 0)
            {
                if (e.Key == Key.F2 && App.uIStateManager.customCtrlStack.Peek() == CtrlHome)
                {
                    App.adminData.SynchronizationOperationTime();
                    App.uIStateManager.SwitchCustomControl(CustomControlType.ADMIN);
                }
            }
        }

        private void CtrlLogin_OnLoginResultRecieved(object sender, bool success)
        {
            if (success)
            {
                MessageBox.Show("로그인에 성공하셨습니다!");
                CtrlLogin.Visibility = Visibility.Collapsed;
                App.uIStateManager.SwitchCustomControl(CustomControlType.HOME);
                App.memberData.GetMemberData();
            }
            else
            {
                MessageBox.Show("로그인에 실패하셨습니다.");
            }
        }

        private void CtrlPay_OnCompletePay()
        {
            CtrlPayComplete.dispatcherTimer.Start();

            var selectedTable = App.placeData.tableViewModel.SelectedTable;
            if (selectedTable != null)
            {
                selectedTable.DispatcherTimer = new DispatcherTimer();
                selectedTable.DispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                selectedTable.DispatcherTimer.Tick += selectedTable.DispatcherTimer_Tick;
                selectedTable.DispatcherTimer.Start();

                if (selectedTable.IsUsed)
                {
                    MessageBox.Show("이미 사용중인 테이블 입니다.");
                    return;
                }
                else
                {
                    selectedTable.IsUsed = true;
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.adminData.SynchronizationOperationTime();
            App.adminData.adminViewModel.SaveProgramTotalUsageTime();
        }
    }
}
