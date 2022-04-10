using Prolonger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonger.Services
{
    public class TextToSTANConverter
    {
        public static STAN Transform(string _text)
        {
            STAN _stan = new STAN();
            _stan.Court = Exeption(_text, 0);
            _stan.Case = Exeption(_text, 1);
            _stan.SubCase = Exeption(_text, 2);
            _stan.RegDate = ExeptionDate(_text, 3);
            _stan.Judge = Exeption(_text, 4);
            _stan.SubJudge = Exeption(_text, 5);
            _stan.Littigans = Exeption(_text, 6);
            _stan.Date = ExeptionDate(_text, 7);
            _stan.Decision = Exeption(_text, 8);
            _stan.SubDecision = Exeption(_text, 9);
            _stan.Category = Exeption(_text, 10);
            _stan.Decision = Exeption(_text,11);
            return _stan;
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
