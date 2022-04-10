using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prolonger.Model;
using SQLite;

namespace Prolonger.Data
{
    public class DataBase
    {
        readonly SQLiteAsyncConnection _ersrDataBase;
        readonly SQLiteAsyncConnection _stanDataBase;
        readonly SQLiteAsyncConnection _courtDataBase;

        public DataBase(string _connectionString, List<string> _dataBaseName)
        {
            _ersrDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[0]));
            _ersrDataBase.CreateTableAsync<ERSR>().Wait();

            _stanDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[1]));
            _stanDataBase.CreateTableAsync<STAN>().Wait();

            _courtDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[2]));
            _courtDataBase.CreateTableAsync<Courts>().Wait();
        }

        #region ERSR

        public Task<int> SaveERSRAsync(List<ERSR> list)
        {
            try
            {
                return _ersrDataBase.InsertAllAsync(list);
            }
            catch
            {
                return null;
            }
        }


        public Task<int> SaveERSRAsync(ERSR _ersr)
        {
            try
            {
                return _ersrDataBase.InsertAsync(_ersr);
            }
            catch
            {
                return null;
            }

        }

        public Task<int> UpdateERSRAsync(ERSR _ersr)
        {
            try
            {
                return _ersrDataBase.UpdateAsync(_ersr);
            }
            catch
            {
                return null;
            }

        }

        public Task<List<ERSR>> GetERSRAsync()
        {
            return _ersrDataBase.Table<ERSR>().ToListAsync();
        }
        public async Task<ERSR> GetERSRAsync(string _id)
        {

            return await _ersrDataBase.Table<ERSR>().Where(x => x.Id == _id).FirstOrDefaultAsync();

        }
        public async Task<List<ERSR>> GetERSRAsyncCase(string _case)
        {

            return await _ersrDataBase.Table<ERSR>().Where(x => x.Case == _case).OrderBy(x => x.DecisionDate).ToListAsync();

        }
        public Task<List<ERSR>> GetERSRAsyncNumber(string _value)
        {
            return _ersrDataBase.Table<ERSR>().Where(x => x.CriminalNumber == _value).OrderBy(x => x.DecisionDate).ToListAsync();
        }

        public Task<List<ERSR>> GetERSRAsyncCourt(string _value)
        {
            return _ersrDataBase.Table<ERSR>().Where(x => x.Court == _value).OrderBy(x => x.DecisionDate).ToListAsync();
        }
        public Task<List<ERSR>> GetERSRAsyncCategory(string _value)
        {
            return _ersrDataBase.Table<ERSR>().Where(x => x.Category == _value).OrderBy(x => x.DecisionDate).ToListAsync();
        }

        public async Task<List<ERSR>> GetERSRAsyncLittigans(string _value)
        {
            List<STAN> result = await _stanDataBase.Table<STAN>().Where(x => x.Littigans.Contains(_value)).ToListAsync();
            List<string> _cases = new List<string>();
            if (result.Count > 0)
            {
                foreach (STAN stan in result)
                {
                    _cases.Add(stan.Case);
                }
                _cases = _cases.Distinct().ToList();
                //получили номера всех судебных дел

                List<ERSR> result2 = new List<ERSR>();
                foreach (string _case in _cases)
                {
                    List<ERSR> r = await App.DataBase.GetERSRAsyncCase(_case);
                    if (r.Count > 0)
                    {
                        result2.AddRange(r);
                    }
                }
                //получили все карточки ЕРСР по номерам судебных дел
                
                if (result2.Count > 0)
                { 
                    result2 = result2.OrderBy(x => x.DecisionDate).ToList();
                    result2 = result2.ToList();               
                    return result2;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
        }
        public Task<List<ERSR>> GetERSRAsyncDecisionKind(string _value)
        {
            return _ersrDataBase.Table<ERSR>().Where(x => x.JusticeDecision == _value).OrderBy(x => x.DecisionDate).ToListAsync();
        }
        public Task<List<ERSR>> GetERSRAsyncText(string _value)
        {
            return _ersrDataBase.Table<ERSR>().Where(x => x.Text.Contains(_value)).OrderBy(x => x.DecisionDate).ToListAsync();
        }
        public Task<List<ERSR>> GetERSRAsyncDate(string start, string end)
        {
            var startDate = Convert.ToDateTime(start);
            var endDate = Convert.ToDateTime(end);
            return _ersrDataBase.Table<ERSR>().Where(x => x.DecisionDate >= startDate & x.DecisionDate <= endDate).OrderBy(x => x.DecisionDate).ToListAsync();
        }

        public async Task<List<ERSR>> GetERSRAsyncLast()
        {
            List<ERSR> result = new List<ERSR>();
            int i = await _ersrDataBase.Table<ERSR>().CountAsync() - 1;
            bool sw = false;
            int j = 0;
            while (sw == false)
            {

                var m = await _ersrDataBase.Table<ERSR>().ElementAtAsync(i - j);
                if (m.Status == "download")
                {
                    result.Add(m);
                }
                j++;

                if (result.Count == 100)
                {
                    sw = true;
                }
            }

            return result;
        }

        public Task<List<ERSR>> GetERSRAsyncLastByDecending()
        {
            return _ersrDataBase.Table<ERSR>().OrderByDescending(x => x.DecisionDate).Take(100).ToListAsync();
        }

        #endregion

        #region STAN

        public Task<int> SaveSTANAsync(List<STAN> list)
        {
            try
            {
                return _stanDataBase.InsertAllAsync(list);
            }
            catch
            {
                return null;
            }
        }


        public Task<int> SaveSTANAsync(STAN _stan)
        {
            try
            {
                return _stanDataBase.InsertAsync(_stan);
            }
            catch
            {
                return null;
            }

        }

        public Task<int> UpdateSTANAsync(STAN _stan)
        {
            try
            {
                return _stanDataBase.UpdateAsync(_stan);
            }
            catch
            {
                return null;
            }

        }

        public Task<List<STAN>> GetSTANAsync()
        {
            return _stanDataBase.Table<STAN>().ToListAsync();
        }

        public Task<List<STAN>> GetSTANAsyncCase(string _case)
        {
            return _stanDataBase.Table<STAN>().Where(x => x.Case == _case).ToListAsync();
        }

        public Task<List<STAN>> GetSTANAsyncLittigans(string _value)
        {
            return  _stanDataBase.Table<STAN>().Where(x => x.Littigans.Contains(_value)).ToListAsync();
           
        }

        #endregion

        #region COURT

        public Task<int> SaveCOURTAsync(List<Courts> list)
        {
            try
            {
                return _courtDataBase.InsertAllAsync(list);
            }
            catch
            {
                return null;
            }
        }


        public Task<int> SaveCOURTAsync(Courts _court)
        {
            try
            {
                return _courtDataBase.InsertAsync(_court);
            }
            catch
            {
                return null;
            }

        }

        public Task<int> UpdateCOURTAsync(Courts _court)
        {
            try
            {
                return _courtDataBase.UpdateAsync(_court);
            }
            catch
            {
                return null;
            }

        }

        public Task<List<Courts>> GetCOURTAsync()
        {
            return _courtDataBase.Table<Courts>().ToListAsync();
        }

        public Task<List<Courts>> GetCOURTAsyncCase(string _case)
        {
            return _courtDataBase.Table<Courts>().Where(x => x.Case == _case).ToListAsync();
        }

        public Task<List<Courts>> GetCOURTAsyncLittigans(string _value)
        {
            return _courtDataBase.Table<Courts>().Where(x => x.Littigans.Contains(_value)).ToListAsync();

        }


        #endregion
    }
}
