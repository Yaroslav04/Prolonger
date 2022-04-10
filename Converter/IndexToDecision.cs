using Prolonger.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Prolonger.Converter
{
    public class IndexToDecision : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string _index = (string)value;
            
            if (_index == "1")
            {
                return "Вирок";
            }
            else if (_index == "5")
            {
                return "Ухвала";
            }
            else
            {
                return _index;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
