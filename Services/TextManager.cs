using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Prolonger.Services
{
    public static class TextManager
    {

        public static string GetCriminalnumber(string _value)
        {
            Regex regex = new Regex(@"\d(20)\d\d\d\d\d\d\d\d\d\d\d\d\d\d");
            MatchCollection matches = regex.Matches(_value);
            if (matches.Count > 0)
            {
                return matches[0].Value;
            }

            return "";
        }

        public static string GetUrl(string _value)
        {

            Regex regex = new Regex(@"(/763/)\w+");
            MatchCollection matches = regex.Matches(_value);
            if (matches.Count > 0)
            {             
                return "https://dsa.court.gov.ua/open_data_files/91509/763/" + matches[0].Value.Replace("/763/","") + ".zip";
                
            }

            return "";

        }

        public static string GetUrlSTAN(string _value)
        {
            Regex regex = new Regex(@"(/532/)\w+");
            MatchCollection matches = regex.Matches(_value);
            if (matches.Count > 0)
            {
                return "https://dsa.court.gov.ua/open_data_files/91509/532/" + matches[0].Value.Replace("/532/", "") + ".csv";
            }
            return "";
        }
    }
}
