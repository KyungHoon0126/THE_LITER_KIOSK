﻿using TheLiter.Core.Admin.ViewModel;

namespace TheLiter.Core.Admin
{
    public class AdminData
    {
        public AdminViewModel adminViewModel = new AdminViewModel();

        public void LoadData()
        {
            adminViewModel.LoadChartDatas();
        }
    }
}
