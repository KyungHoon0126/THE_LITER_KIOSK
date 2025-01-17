﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace THE_LITER_KIOSK.Converters
{
    public class IsSoldOutToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
