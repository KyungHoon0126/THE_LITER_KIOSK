﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using THE_LITER_KIOSK.UIManager;
using TheLiter.Core.Order.Model;

namespace THE_LITER_KIOSK.Controls.OrderControl
{
    /// <summary>
    /// Interaction logic for OrderControl.xaml
    /// </summary>
    public partial class OrderControl : CustomControlModel, INotifyPropertyChanged
    {
        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private CollectionViewSource _collectionViewSource = new CollectionViewSource();
        public CollectionViewSource CollectionViewSource
        {
            get => _collectionViewSource;
            set
            {
                _collectionViewSource = value;
                NotifyPropertyChanged(nameof(CollectionViewSource));
            }
        }

        private ObservableCollection<MenuModel> _menus = new ObservableCollection<MenuModel>();
        public ObservableCollection<MenuModel> Menus
        {
            get => _menus;
            set
            {
                _menus = value;
                NotifyPropertyChanged(nameof(Menus));
            }
        }

        //CollectionViewSource CollectionViewSource = new CollectionViewSource();
        //ObservableCollection<TheLiter.Core.Order.Model.Menu> Menus = new ObservableCollection<TheLiter.Core.Order.Model.Menu>();
        int currentPageIdx = 0;
        int itemPerPage = 12;
        int totalPage = 0;
        #endregion

        public OrderControl()
        {
            InitializeComponent();
            Loaded += OrderControl_Loaded;
        }

        private void OrderControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = App.orderData.orderViewModel;

            // DispatcherPriority.Noraml :  보통 우선 순위로 작업이 처리됩니다. 일반적인 애플리케이션 우선 순위이다.
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                // #1
                //for (int i = 0; i < App.orderData.orderViewModel.MenuItems.Count; i++)
                //{
                //    menus.Add(App.orderData.orderViewModel.MenuItems[i].Clone() as MenuModel);
                //}

                // #2
                Menus = App.orderData.orderViewModel.MenuItems;
            }));

            lvCategory.SelectedIndex = 0;
            // SetMenuPage();   
        }

        #region MenuItemsPage
        private void SetMenuPage()
        {
            if (lvMenuList.Items.Count > 0)
            {
                // App.orderData.orderViewModel.MenuItems.Clear();
                //lvMenuList.ClearValue(ItemsControl.ItemsSourceProperty);
            }

            int itemCnt = Menus.Count;
            totalPage = (itemCnt / itemPerPage);
            if (itemCnt % itemPerPage != 0) 
            {
                totalPage += 1;
            }

            CollectionViewSource.Source = Menus;
            CollectionViewSource.Filter += CollectionViewSource_Filter;
            this.lvMenuList.DataContext = CollectionViewSource;
            CollectionViewSource.View.Refresh();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            int idx = Menus.IndexOf((MenuModel)e.Item);

            if (idx >= itemPerPage * currentPageIdx && idx < itemPerPage * (currentPageIdx + 1))
            {
                e.Accepted = true;
                Debug.WriteLine(((MenuModel)e.Item).Name + e.Accepted);
            }
            else
            {
                e.Accepted = false;
                Debug.WriteLine(((MenuModel)e.Item).Name + e.Accepted);
            }    
        }
        #endregion

        private void lvCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilteringMenuItems(((CategoryModel)lvCategory.SelectedItem).ECategory);
        }

        private void FilteringMenuItems(ECategory category)
        {
            // TODO : ALL, THELITERSPECIAL 선택 시 페이징 기능 추가.
            if (category == ECategory.ALL)
            {
                SetMenuPage();
                // lvMenuList.ItemsSource = App.orderData.orderViewModel.MenuItems;
            }
            else
            {
                //lvMenuList.ItemsSource = App.orderData.orderViewModel.MenuItems.Where(x => x.MenuCategory == category);
                lvMenuList.ItemsSource = Menus.Where(x => x.MenuCategory == category);
            }
        }

        #region MenuItemsPage
        private void btnPreviousMenu_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIdx > 0)
            {
                currentPageIdx--;
                CollectionViewSource.View.Refresh();
                return;
            }

            MessageBox.Show("첫 페이지 입니다.");
        }

        private void btnNextMenu_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIdx < totalPage - 1)
            {
                currentPageIdx++;
                CollectionViewSource.View.Refresh();
                return;
            }

            MessageBox.Show("마지막 페이지 입니다.");
        }
        #endregion

        private void lvMenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvMenuList.SelectedItem == null)
            {
                return;
            }

            MenuModel selectedMenu = (MenuModel)lvMenuList.SelectedItem;

            if (selectedMenu != null && IsDuplicateMenu(selectedMenu))
            {
                IncreaseMenuCount(selectedMenu);
            }
            else
            {
                IncreaseMenuCount(selectedMenu);
                AddOrderedMenuItems(selectedMenu);
            }

            lvMenuList.SelectedIndex = -1;
        }

        private bool IsDuplicateMenu(MenuModel selectedMenu)
        {
            for (int i = 0; i < lvOrderList.Items.Count; i++)
            {
                if (selectedMenu.Name == (lvOrderList.Items[i] as MenuModel).Name)
                {
                    return true;
                }
            }
            return false;
        }

        private void btnAddMenu_Click(object sender, RoutedEventArgs e)     
        {
            IncreaseMenuCount(((ListViewItem)lvOrderList.ContainerFromElement(sender as Button)).Content as MenuModel);
        }

        private void btnSubMenu_Click(object sender, RoutedEventArgs e)
        {
            var selectedMenu = ((ListViewItem)lvOrderList.ContainerFromElement(sender as Button)).Content as MenuModel;
            if (IsQuantityValid(selectedMenu))
            {
                DecreaseMenuCount(selectedMenu);
                RemoveSelectedMenu(selectedMenu);
                return;
            }
            DecreaseMenuCount(selectedMenu);
        }

        private bool IsQuantityValid(MenuModel selectedMenu)
        {
            if (selectedMenu.Count == 1)
            {
                return true;
            }
            return false;
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            MenuModel selectedMenu = ((ListViewItem)lvOrderList.ContainerFromElement(sender as Button)).Content as MenuModel;
            ClearSelectedMenuItems(selectedMenu);
        }

        private void ClearSelectedMenuItems(MenuModel selectedMenu)
        {
            App.orderData.orderViewModel.ClearSelectedMenuItems(selectedMenu);
        }

        private void AddOrderedMenuItems(MenuModel menuModel)
        {
            App.orderData.orderViewModel.AddOrderedMenuItems(menuModel);
        }

        private void IncreaseMenuCount(MenuModel selectedMenu)
        {
            App.orderData.orderViewModel.IncreaseMenuCount(selectedMenu);
        }

        private void DecreaseMenuCount(MenuModel selectedMenu)
        {
            App.orderData.orderViewModel.DecreaseMenuCount(selectedMenu);
        }

        private void RemoveSelectedMenu(MenuModel selectedMenu)
        {
            App.orderData.orderViewModel.RemoveSelectedMenu(selectedMenu);
        }

        private void btnClearOrderList_Click(object sender, RoutedEventArgs e)
        {
            ShowCancelPopup("정말 모두 삭제하시겠습니까?", "모두 삭제되었습니다.");
            lvMenuList.SelectedItem = null;
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            if (IsOrderedMenuListValid())
            {
                App.uIStateManager.SwitchCustomControl(CustomControlType.PLACE);
            }
            else
            {
                MessageBox.Show("주문할 음식을 선택해 주세요.");
            }
        }

        private bool IsOrderedMenuListValid()
        {
            return App.orderData.orderViewModel.IsOrderedMenuListValid();
        }

        private void btnMoveToHome_Click(object sender, RoutedEventArgs e)
        {
            ShowCancelPopup("정말 주문을 취소하시겠습니까?", "주문이 취소되었습니다.");
            App.uIStateManager.SwitchCustomControl(CustomControlType.HOME);
        }

        private void ShowCancelPopup(string popupMsg, string resultMsg)
        {
            if (IsOrderedMenuListValid())
            {
                MessageBoxResult result = MessageBox.Show(popupMsg, "주문 목록", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        MessageBox.Show(resultMsg);
                        InitData();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        private void InitData()
        {
            App.orderData.InitData();
        }
    }
}
