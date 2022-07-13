﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MapleUtility.Plugins.Converters
{
    public class IsEqualTargetItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var firstItem = values[0];
            var secondItem = values[1];

            if (firstItem == null || secondItem == null)
                return false;

            if (firstItem.GetHashCode() == secondItem.GetHashCode())
                return true;

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
