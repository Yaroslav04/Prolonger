using Prolonger.Model;
using Prolonger.Services;
using Prolonger.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Prolonger
{
    public partial class MainWindow : Window
    {
        public static RichTextBox RichText;
        public MainWindow()
        {
            InitializeComponent();           
            FileManager.FileInit();      
            RichText = RTB;
            this.DataContext = new MainViewModel();
        }
    }
}
