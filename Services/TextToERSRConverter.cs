using Prolonger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonger.Services
{
    public class TextToERSRConverter
    {
        public static ERSR Transform(string _text)
        {
            ERSR _ersr = new ERSR();
            _ersr.Id = Exeption(_text, 0);
            _ersr.Court = Exeption(_text, 1);
            _ersr.JusticeDecision = Exeption(_text, 2);
            _ersr.JusticeKind = Exeption(_text, 3);
            _ersr.Category = Exeption(_text, 4);
            _ersr.Case = Exeption(_text, 5);
            _ersr.DecisionDate = ExeptionDate(_text, 6);
            _ersr.PublicDate = ExeptionDate(_text, 7);
            _ersr.Url = Exeption(_text, 9);
            _ersr.Status = Exeption(_text, 10);
            _ersr.CriminalNumber = "";
            _ersr.DownloadDate = DateTime.Now;
            return _ersr;
        }

        private static string Exeption(string _text, int _index)
        {
            try
            {
                string[] _array = _text.Split('\t');
                return _array[_index].Replace("\"", "");
            }
            catch
            {
                return "";
            }
        }

        private static DateTime ExeptionDate(string _text, int _index)
        {
            try
            {
                string[] _array = _text.Split('\t');
                return Convert.ToDateTime(_array[_index].Replace("\"", ""));
            }
            catch
            {
                return Convert.ToDateTime("01.01.2000 00:00:00");
            }
        }
    }

}
