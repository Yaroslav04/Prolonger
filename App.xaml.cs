using Prolonger.Data;
using Prolonger.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Prolonger
{
    public partial class App : Application
    {

        private static DataBase dataBase;
        public static DataBase DataBase
        {
            get
            {
                if (dataBase == null)
                {
                    dataBase = new DataBase(FileManager.GeneralPath(), new List<string> { "ERSRDataBase.db3", "STANDataBase.db3", "COURTDataBase.db3" });
                }
                return dataBase;
            }
        }

        private static int showItemsCount = 200;
        public static int ShowItemsCount
        {
            get
            {
                return showItemsCount;
            }
            set
            {
                showItemsCount = value;
            }
        }
    }
}
