using Microsoft.Win32;
using Prolonger.Model;
using Prolonger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Prolonger.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            Items = new ObservableCollection<ERSR>();
            ItemsSTAN = new ObservableCollection<STAN>();
            ItemsCOURT = new ObservableCollection<Courts>();
            Categories = new List<string>();
            CategoriesList = new ObservableCollection<string>();
            foreach (string s in File.ReadAllLines(FileManager.GeneralPath(@"Download\cause_categories.csv")))
            {
                Categories.Add(s.Replace("\"", ""));
            }

            //STANDownload = new List<string>();
            //foreach (string s in File.ReadAllLines(FileManager.GeneralPath(@"Download\download_stan.csv")))
            //{
            //    if (!String.IsNullOrWhiteSpace(s))
            //    {
            //        STANDownload.Add(s);
            //    }
            //}

            Decisions = new List<string> { "Вирок", "Ухвала" };

            Courts = new List<string> { "0415", "0404", "0409", "4803", "0490" };

        }

        #region Properties

        #region ERSR
        public ObservableCollection<ERSR> Items { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Decisions { get; set; }
        public List<string> Courts { get; set; }
        public ObservableCollection<string> CategoriesList { get; set; }

        private ERSR selectedItem;
        public ERSR SelectedItem
        {
            get => selectedItem;
            set
            {
                SetProperty(ref selectedItem, value);
                if (selectedItem != null)
                {
                    LoadDocument(selectedItem.Id);
                }
            }
        }

        private string criminalNumber = "";
        public string CriminalNumber
        {
            get => criminalNumber;
            set
            {
                SetProperty(ref criminalNumber, value);
            }
        }

        private string court = "";
        public string Court
        {
            get => court;
            set
            {
                SetProperty(ref court, value);
            }
        }

        private string stanDescriptions = "";
        public string StanDescriptions
        {
            get => stanDescriptions;
            set
            {
                SetProperty(ref stanDescriptions, value);
            }
        }

        private string stanLittigans = "";
        public string StanLittigans
        {
            get => stanLittigans;
            set
            {
                SetProperty(ref stanLittigans, value);
            }
        }

        private string caseNumber = "";
        public string CaseNumber
        {
            get => caseNumber;
            set
            {
                SetProperty(ref caseNumber, value);
            }
        }

        private string decisionKind = "";
        public string DecisionKind
        {
            get => decisionKind;
            set
            {
                if (value == "Вирок")
                {
                    SetProperty(ref decisionKind, "1");
                }
                else if (value == "Ухвала")
                {
                    SetProperty(ref decisionKind, "5");
                }
                else
                {
                    SetProperty(ref decisionKind, "");
                }

            }
        }

        private string category = "";
        public string Category
        {
            get => category;
            set
            {
                SetProperty(ref category, value);
            }
        }

        private string selectCategory = "";
        public string SelectCategory
        {
            get => selectCategory;
            set
            {
                string[] _array = value.Split("\t");
                if (_array.Length > 1)
                {
                    Category = _array[0];
                }

                SetProperty(ref selectCategory, value);
            }
        }

        private string searchCategory = "";
        public string SearchCategory
        {
            get => searchCategory;
            set
            {
                if (CategoriesList.Count > 0)
                {
                    CategoriesList.Clear();
                }
                if (value.Length > 4)
                {
                    foreach (var item in Categories)
                    {
                        if (item.Contains(value))
                        {
                            CategoriesList.Add(item);
                        }
                    }
                }

                SetProperty(ref searchCategory, value);
            }
        }

        private string text = "";
        public string Text
        {
            get => text;
            set
            {
                SetProperty(ref text, value);
            }
        }

        private string startDate;
        public string StartDate
        {
            get => startDate;
            set
            {
                SetProperty(ref startDate, value);
            }
        }

        private string endDate;
        public string EndDate
        {
            get => endDate;
            set
            {
                SetProperty(ref endDate, value);
            }
        }

        #endregion

        #region STAN

        public ObservableCollection<STAN> ItemsSTAN { get; set; }

        public List<string> STANDownload;

        private string caseNumberSTAN = "";
        public string CaseNumberSTAN
        {
            get => caseNumberSTAN;
            set
            {
                SetProperty(ref caseNumberSTAN, value);
            }
        }

        private string littigansSTAN = "";
        public string LittigansSTAN
        {
            get => littigansSTAN;
            set
            {
                SetProperty(ref littigansSTAN, value);
            }
        }

        #endregion

        #region COURT

        public ObservableCollection<Courts> ItemsCOURT { get; set; }

        private string caseNumberCOURT = "";
        public string CaseNumberCOURT
        {
            get => caseNumberCOURT;
            set
            {
                SetProperty(ref caseNumberCOURT, value);
            }
        }

        private string littigansCOURT = "";
        public string LittigansCOURT
        {
            get => littigansCOURT;
            set
            {
                SetProperty(ref littigansCOURT, value);
            }
        }

        #endregion

        #region Settings

        private int showItemsCount = 250;
        public int ShowItemsCount
        {
            get => showItemsCount;
            set
            {
                if (value == -1)
                {
                    SetProperty(ref showItemsCount, int.MaxValue);
                }
                else
                {
                    SetProperty(ref showItemsCount, value);
                }
            }
        }

        #endregion

        #endregion

        #region Command

        private ICommand _downloadCommand;
        public ICommand DownloadCommand
        {
            get
            {
                return _downloadCommand ?? (_downloadCommand = new CommandHandler(() => MyActionDownload(), true));
            }
        }
        public void MyActionDownload()
        {
            DownloadAsync();
        }

        private ICommand _uploadCommand;
        public ICommand UploadCommand
        {
            get
            {
                return _uploadCommand ?? (_uploadCommand = new CommandHandler(() => MyActionUpload(), true));
            }
        }
        public void MyActionUpload()
        {
            LoadItemsAsync();
        }

        private ICommand _addCommand;
        public ICommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new CommandHandler(() => MyActionAdd(), true));
            }
        }
        public void MyActionAdd()
        {
            AddItemsAsync();
        }

        private ICommand _uploadLastCommand;
        public ICommand UploadLastCommand
        {
            get
            {
                return _uploadLastCommand ?? (_uploadLastCommand = new CommandHandler(() => MyActionUploadLast(), true));
            }
        }
        public void MyActionUploadLast()
        {
            LoadLastItemsAsync();
        }

        private ICommand _printCommand;
        public ICommand PrintCommand
        {
            get
            {
                return _printCommand ?? (_printCommand = new CommandHandler(() => MyActionPrint(), true));
            }
        }
        public void MyActionPrint()
        {
            PrintRTF();
        }

        private ICommand _importCommand;
        public ICommand ImportCommand
        {
            get
            {
                return _importCommand ?? (_importCommand = new CommandHandler(() => MyActionImport(), true));
            }
        }

        private void MyActionImport()
        {
            OnlyImportAsync();
        }

        private ICommand _searchSTANCommand;
        public ICommand _SearchSTANCommand
        {
            get
            {
                return _searchSTANCommand ?? (_searchSTANCommand = new CommandHandler(() => MyActionSearchSTAN(), true));
            }
        }

        private void MyActionSearchSTAN()
        {
            SearchSTANAsync();
        }

        private ICommand _searchCOURTCommand;
        public ICommand _SearchCOURTCommand
        {
            get
            {
                return _searchCOURTCommand ?? (_searchCOURTCommand = new CommandHandler(() => MyActionSearchCOURT(), true));
            }
        }

        private void MyActionSearchCOURT()
        {
            SearchCOURTAsync();
        }

        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new CommandHandler(() => MyActionClear(), true));
            }
        }

        private void MyActionClear()
        {
            Clear();
        }

        private ICommand _exportCommand;
        public ICommand ExportCommand
        {
            get
            {
                return _exportCommand ?? (_exportCommand = new CommandHandler(() => MyActionExport(), true));
            }
        }

        private void MyActionExport()
        {
            ExportCSV();
        }


        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value,
                                      [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Functions

        public string RTFToText(string _path)
        {
            MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(File.ReadAllText(_path)));
            MainWindow.RichText.Document.Blocks.Clear();
            MainWindow.RichText.Selection.Load(stream, DataFormats.Rtf);

            string rtfFromRtb = string.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                TextRange range2 = new TextRange(MainWindow.RichText.Document.ContentStart, MainWindow.RichText.Document.ContentEnd);
                range2.Save(ms, DataFormats.Text);
                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(ms))
                {
                    rtfFromRtb = sr.ReadToEnd();
                }
            }

            return rtfFromRtb;
        }

        private async void DownloadSTAN()
        {
            var list = new List<string>();
            foreach (var s in DownloadManager.GetUrlSTAN(@"https://dsa.court.gov.ua/dsa/inshe/oddata/532/"))
            {
                if (!STANDownload.Contains(s))
                {
                    list.Add(s);
                }
            }

            foreach (var s in list)
            {
                try
                {
                    await DownloadManager.Download(s, FileManager.GeneralPath(@"Download\stan.csv"));
                    using (StreamWriter sw = new StreamWriter(FileManager.GeneralPath(@"Download\download_stan.csv"), true))
                    {
                        sw.WriteLine(s);
                    }
                    await ImportSTAN();
                }
                catch
                {

                }

            }
        }

        public async void LoadItemsAsync()
        {
            Items.Clear();
            List<ERSR> _Items = new List<ERSR>();

            if (StanLittigans != "")
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncLittigans(StanLittigans);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            if (Text != "")
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncText(Text);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
                else
                {
                    var result = _Items.Where(x => x.Text.Contains(Text)).ToList();
                    _Items.Clear();
                    if (result.Count > 0)
                    {
                        result = result.OrderBy(x => x.DecisionDate).ToList();

                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(StartDate) & !String.IsNullOrWhiteSpace(EndDate))
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncDate(StartDate, EndDate);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
                else
                {
                    var result = _Items.Where(x => x.DecisionDate >= Convert.ToDateTime(StartDate) & x.DecisionDate <= Convert.ToDateTime(EndDate)).OrderBy(x => x.DecisionDate).ToList();
                    _Items.Clear();
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            if (CriminalNumber != "")
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncNumber(CriminalNumber);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
                else
                {
                    var result = _Items.Where(x => x.CriminalNumber == CriminalNumber).ToList();
                    _Items.Clear();
                    if (result.Count > 0)
                    {
                        result = result.OrderBy(x => x.DecisionDate).ToList();
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            if (Court != "")
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncCourt(Court);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
                else
                {
                    var result = _Items.Where(x => x.Court == Court).ToList();
                    _Items.Clear();
                    if (result.Count > 0)
                    {
                        result = result.OrderBy(x => x.DecisionDate).ToList();

                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            if (Category != "")
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncCategory(Category);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
                else
                {
                    var result = _Items.Where(x => x.Category == Category).ToList();

                    _Items.Clear();
                    if (result.Count > 0)
                    {
                        result = result.OrderBy(x => x.DecisionDate).ToList();
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            if (DecisionKind != "")
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncDecisionKind(DecisionKind);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
                else
                {
                    var result = _Items.Where(x => x.JusticeDecision == DecisionKind).ToList();
                    _Items.Clear();
                    if (result.Count > 0)
                    {
                        result = result.OrderBy(x => x.DecisionDate).ToList();
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            if (CaseNumber != "")
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncCase(CaseNumber);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
                else
                {
                    var result = _Items.Where(x => x.Case == CaseNumber).ToList();
                    _Items.Clear();
                    if (result.Count > 0)
                    {
                        result = result.OrderBy(x => x.DecisionDate).ToList();
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            int j = 0;

            if (_Items.Count > 0)
            {
                j = _Items.Count;
                _Items = _Items.Take(ShowItemsCount).ToList();
                foreach (var item in _Items)
                {
                    Items.Add(item);
                }
            }

            MessageBox.Show("Готово: " + Items.Count + " из " + j);
        }

        public async void AddItemsAsync()
        {
            List<ERSR> _Items = new List<ERSR>();

            if (StanLittigans != "")
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncLittigans(StanLittigans);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            if (Text != "")
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncText(Text);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
                else
                {
                    var result = _Items.Where(x => x.Text.Contains(Text)).ToList();
                    _Items.Clear();
                    if (result.Count > 0)
                    {
                        result = result.OrderBy(x => x.DecisionDate).ToList();

                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(StartDate) & !String.IsNullOrWhiteSpace(EndDate))
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncDate(StartDate, EndDate);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
                else
                {
                    var result = _Items.Where(x => x.DecisionDate >= Convert.ToDateTime(StartDate) & x.DecisionDate <= Convert.ToDateTime(EndDate)).OrderBy(x => x.DecisionDate).ToList();
                    _Items.Clear();
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            if (CriminalNumber != "")
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncNumber(CriminalNumber);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
                else
                {
                    var result = _Items.Where(x => x.CriminalNumber == CriminalNumber).ToList();
                    _Items.Clear();
                    if (result.Count > 0)
                    {
                        result = result.OrderBy(x => x.DecisionDate).ToList();
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            if (Court != "")
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncCourt(Court);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
                else
                {
                    var result = _Items.Where(x => x.Court == Court).ToList();
                    _Items.Clear();
                    if (result.Count > 0)
                    {
                        result = result.OrderBy(x => x.DecisionDate).ToList();

                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            if (Category != "")
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncCategory(Category);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
                else
                {
                    var result = _Items.Where(x => x.Category == Category).ToList();

                    _Items.Clear();
                    if (result.Count > 0)
                    {
                        result = result.OrderBy(x => x.DecisionDate).ToList();
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            if (DecisionKind != "")
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncDecisionKind(DecisionKind);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
                else
                {
                    var result = _Items.Where(x => x.JusticeDecision == DecisionKind).ToList();
                    _Items.Clear();
                    if (result.Count > 0)
                    {
                        result = result.OrderBy(x => x.DecisionDate).ToList();
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            if (CaseNumber != "")
            {
                if (_Items.Count == 0)
                {
                    var result = await App.DataBase.GetERSRAsyncCase(CaseNumber);
                    if (result.Count > 0)
                    {
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
                else
                {
                    var result = _Items.Where(x => x.Case == CaseNumber).ToList();
                    _Items.Clear();
                    if (result.Count > 0)
                    {
                        result = result.OrderBy(x => x.DecisionDate).ToList();
                        foreach (ERSR item in result)
                        {
                            _Items.Add(item);
                        }
                    }
                }
            }

            int j = 0;

            if (_Items.Count > 0)
            {
                j = _Items.Count;
                _Items = _Items.Take(ShowItemsCount).ToList();
                foreach (var item in _Items)
                {
                    Items.Add(item);
                }

                List<ERSR> list = new List<ERSR>(Items);
                Items.Clear();
                list = list.Distinct().ToList();
                foreach (ERSR item in list)
                {
                    Items.Add(item);
                }
            }

            MessageBox.Show("Добавлено: " + Items.Count + " из " + j);
        }


        public async void LoadItemsSTANAsync()
        {
            ItemsSTAN.Clear();

            if (CaseNumberSTAN != "")
            {
                var result = await App.DataBase.GetSTANAsyncCase(CaseNumberSTAN);
                result = result.OrderBy(x => x.Date).ToList();
                if (result.Count > 0)
                {
                    foreach (STAN item in result)
                    {
                        ItemsSTAN.Add(item);
                    }
                }
                MessageBox.Show("Готово: " + ItemsSTAN.Count);
                return;
            }
            else if (LittigansSTAN != "")
            {
                var result = await App.DataBase.GetSTANAsyncLittigans(LittigansSTAN);
                result = result.OrderBy(x => x.Date).ToList();
                if (result.Count > 0)
                {
                    foreach (STAN item in result)
                    {
                        ItemsSTAN.Add(item);
                    }
                }
                MessageBox.Show("Готово: " + ItemsSTAN.Count);
                return;
            }
            else
            {
                MessageBox.Show("Готово: " + ItemsSTAN.Count);
                return;
            }
        }

        public async void LoadItemsCOURTAsync()
        {
            ItemsCOURT.Clear();

            var r = await App.DataBase.GetCOURTAsync();
            Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + r.Count);
            Debug.WriteLine(CaseNumberCOURT);
            if (CaseNumberCOURT != "")
            {
                var result = await App.DataBase.GetCOURTAsyncCase(CaseNumberCOURT);
                result = result.OrderBy(x => x.Date).ToList();
                if (result.Count > 0)
                {
                    foreach (Courts item in result)
                    {
                        ItemsCOURT.Add(item);
                    }
                }
                MessageBox.Show("Готово: " + ItemsCOURT.Count);
                return;
            }
            else if (LittigansCOURT != "")
            {
                var result = await App.DataBase.GetCOURTAsyncLittigans(LittigansCOURT);
                result = result.OrderBy(x => x.Date).ToList();
                if (result.Count > 0)
                {
                    foreach (Courts item in result)
                    {
                        ItemsCOURT.Add(item);
                    }
                }
                MessageBox.Show("Готово: " + ItemsCOURT.Count);
                return;
            }
            else
            {
                MessageBox.Show("Готово: " + ItemsCOURT.Count);
                return;
            }
        }


        public async void LoadLastItemsAsync()
        {
            Items.Clear();
            var result = await App.DataBase.GetERSRAsyncLastByDecending();
            if (result.Count > 0)
            {
                foreach (ERSR item in result)
                {
                    Items.Add(item);
                }
            }

            MessageBox.Show("Готово: " + Items.Count);
        }

        private async void LoadDocument(string _id)
        {
            string _fileName = FileManager.GeneralPath(@"Download\Docs\" + _id.ToString() + ".rtf");
            if (File.Exists(_fileName))
            {
                MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(File.ReadAllText(_fileName)));
                MainWindow.RichText.Document.Blocks.Clear();
                MainWindow.RichText.Selection.Load(stream, DataFormats.Rtf);
            }
            else
            {
                MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(File.ReadAllText(FileManager.GeneralPath(@"Download\Docs\empty.rtf"))));
                MainWindow.RichText.Document.Blocks.Clear();
                MainWindow.RichText.Selection.Load(stream, DataFormats.Rtf);
            }

            var result = await App.DataBase.GetSTANAsyncCase(selectedItem.Case);
            if (result.Count > 0)
            {
                StanDescriptions = result.FirstOrDefault().Littigans;
            }
            else
            {
                StanDescriptions = "";
            }
        }

        public async void DownloadAsync()
        {
            try
            {
                await DownloadManager.Download(DownloadManager.GetUrl("https://dsa.court.gov.ua/dsa/inshe/oddata/763/"), FileManager.GeneralPath(@"Download\ersr.zip"));
            }
            catch
            {
                MessageBox.Show("Ошибка скачивания");
            }

            try
            {
                ZipFile.ExtractToDirectory(FileManager.GeneralPath(@"Download\ersr.zip"), FileManager.GeneralPath(@"Download\"), true);
                await Import();

            }
            catch
            {
                MessageBox.Show("Ошибка импорта");

            }

            try
            {
                DownloadSTAN();
            }
            catch
            {
                MessageBox.Show("Ошибка скачивания СТАН");
            }

            MessageBox.Show("Готово");
        }

        public async void OnlyImportAsync()
        {
            try
            {
                await Import();
            }
            catch
            {
                MessageBox.Show("Ошибка импорта");
            }

            MessageBox.Show("Готово");
        }

        public async Task Import()
        {
            int k = 0;
            List<ERSR> import = new List<ERSR>();
            List<string> _courts = new List<string>(new string[] { "\t0415\t", "\t0404\t", "\t0409\t", "\t4803\t", "\t0490\t" });
            using (StreamReader sr = new StreamReader(FileManager.GeneralPath(@"Download\documents.csv")))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    foreach (var _court in _courts)
                    {
                        if (line.Contains(_court))
                        {
                            var _ersr = TextToERSRConverter.Transform(line);
                            if (_ersr.JusticeKind == "2")
                            {
                                if (_ersr.Case.Contains("207/") | _ersr.Case.Contains("208/") | _ersr.Case.Contains("209/"))
                                {
                                    try
                                    {
                                        var already = await App.DataBase.GetERSRAsync(_ersr.Id);
                                        if (already == null)
                                        {
                                            if (_ersr.Status == "1")
                                            {
                                                await DownloadManager.Download(_ersr.Url, FileManager.GeneralPath(@"Download\Docs\" + _ersr.Id + ".rtf"));
                                                _ersr.CriminalNumber = TextManager.GetCriminalnumber(File.ReadAllText(FileManager.GeneralPath(@"Download\Docs\" + _ersr.Id + ".rtf")));
                                                _ersr.Status = "download";
                                                _ersr.Text = RTFToText(FileManager.GeneralPath(@"Download\Docs\" + _ersr.Id + ".rtf"));

                                            }
                                            import.Add(_ersr);
                                            k++;
                                        }
                                        else
                                        {
                                        }

                                        if (k == 1000)
                                        {
                                            await App.DataBase.SaveERSRAsync(import);
                                            import.Clear();
                                            k = 0;
                                        }
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            File.Delete(FileManager.GeneralPath(@"Download\Docs\" + _ersr.Id + ".rtf"));
                                        }
                                        catch
                                        {

                                        }
                                    }

                                }

                            }
                        }
                    }
                }

                if (import.Count > 0)
                {
                    await App.DataBase.SaveERSRAsync(import);
                    import.Clear();
                }
            }

        }

        public async Task ImportSTAN()
        {
            List<string> _courts = new List<string>(new string[] { "Заводський районний суд м. Дніпродзержинська", "Дніпровський районний суд м. Дніпродзержинська", "Баглійський районний суд м. Дніпродзержинська", "Дніпровський апеляційний суд" });
            using (StreamReader sr = new StreamReader(FileManager.GeneralPath(@"Download\stan.csv")))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    foreach (var _court in _courts)
                    {
                        if (line.Contains(_court))
                        {
                            var _stan = TextToSTANConverter.Transform(line);
                            if (_stan.Case.Contains("207/") | _stan.Case.Contains("208/") | _stan.Case.Contains("209/"))
                            {
                                try
                                {
                                    await App.DataBase.SaveSTANAsync(_stan);
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                }
            }
        }

        private void SearchSTANAsync()
        {
            LoadItemsSTANAsync();
        }

        private void SearchCOURTAsync()
        {
            LoadItemsCOURTAsync();
        }

        public void PrintRTF()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "RTF file (*.rtf)|*.rtf";
            saveFileDialog.AddExtension = true;
            saveFileDialog.FileName = selectedItem.Id;
            if (selectedItem != null)
            {
                string fileName = FileManager.GeneralPath(@"Download\Docs\" + selectedItem.Id + ".rtf");
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.Copy(fileName, saveFileDialog.FileName, true);

                }
            }

        }

        private void Clear()
        {
            CriminalNumber = "";
            Category = "";
            SelectCategory = "";
            SearchCategory = "";
            StartDate = "";
            EndDate = "";
            CategoriesList.Clear();
            Text = "";
            CaseNumber = "";
            Court = "";
            DecisionKind = "";
            StanLittigans = "";
            StanDescriptions = "";

            LittigansSTAN = "";
            CaseNumberSTAN = "";
          
            LittigansCOURT = "";
            CaseNumberCOURT = "";

            Items.Clear();
            ItemsSTAN.Clear();
            ItemsCOURT.Clear();


            MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(File.ReadAllText(FileManager.GeneralPath(@"Download\Docs\clear.rtf"))));
            MainWindow.RichText.Document.Blocks.Clear();
            MainWindow.RichText.Selection.Load(stream, DataFormats.Rtf);
        }

        public void ExportCSV()
        {
            if (Items.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV file (*.csv)|*.csv";
                saveFileDialog.AddExtension = true;
                saveFileDialog.FileName = "export";


                if (saveFileDialog.ShowDialog() == true)
                {

                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                    {
                        foreach (var item in Items)
                        {
                            sw.WriteLine($"{item.Id}\t{item.Court}\t{item.JusticeDecision}\t{item.JusticeKind}\t{item.Category}\t{item.Case}\t{item.DecisionDate}\t{item.PublicDate}\t{item.CriminalNumber}\t{item.Text.Replace("\t", "").Replace("\n", "").Replace("\r", "")}\t");
                        }

                    }

                }
            }

            MessageBox.Show("Готово");
        }

        #endregion
    }
}
