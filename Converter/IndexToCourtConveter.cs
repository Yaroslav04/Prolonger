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
    public class IndexToCourtConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string _index = (string)value;
            Dictionary<string,string> _courts = new Dictionary<string,string>();
            foreach (string s in File.ReadAllLines(FileManager.GeneralPath(@"Download\courts.csv")))
            {
                string[] _array = s.Split("\t");
                if (_array.Length > 1)
                {
                    _courts.Add(_array[0], _array[1].Replace("\"", ""));
                }
            }
            return _courts[_index];    
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           return value;
        }
    }
}
