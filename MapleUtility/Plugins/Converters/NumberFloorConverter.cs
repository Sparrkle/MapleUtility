using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MapleUtility.Plugins.Converters
{
    public class NumberFloorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double dv)
                return Math.Floor(double.Parse(dv.ToString(parameter?.ToString() ?? string.Empty)));
            else if (value is float fv)
                return Math.Floor(double.Parse(fv.ToString(parameter?.ToString() ?? string.Empty)));
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
