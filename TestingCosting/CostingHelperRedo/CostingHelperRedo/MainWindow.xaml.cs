using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace CostingHelper
{
    /// Main window. Lets user enter input/output files and search preferences.
    public partial class MainWindow : Window
    {
        public static string directText = "Enter Items Here";
        public static string readFile = "";
        public static string writeFile = "";
        public static bool usingFile = true;

        public static List<int> selectedSites = new List<int> { 0 };
        public static List<int> numResultsList = new List<int> { 2, 2, 2, 2, 2 };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChooseReadFile_Click(object sender, RoutedEventArgs e)
        {
            usingFile = true;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.ShowDialog();
            ReadFileBox.Text = openFileDialog1.FileName;
        }

        private void ChooseWriteFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.ShowDialog();
            WriteFileBox.Text = saveFileDialog1.FileName;
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {

            if ((usingFile && (!ReadFileBox.Text.Contains(".txt"))) || (!WriteFileBox.Text.Contains(".txt")))
            {
                ErrorWindow err = new ErrorWindow();
                err.Show();
            }
            else
            {
                this.Visibility = Visibility.Hidden;
                Window1 objWindow1 = new Window1();
                objWindow1.Show();
            }
        }

        private void ReadFileBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            readFile = ReadFileBox.Text;
        }

        private void WriteFileBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            writeFile = WriteFileBox.Text;
        }

        private void ChooseSites_Click(object sender, RoutedEventArgs e)
        {
            ChooseSitesWindow toOpen = new ChooseSitesWindow();
            toOpen.Show();
        }

        private void EnterDirect_Click(object sender, RoutedEventArgs e)
        {
            usingFile = false;
            DirectEntryWindow toOpen = new DirectEntryWindow();
            toOpen.Show();
        }
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow toOpen = new HelpWindow();
            toOpen.Show();
        }

    }
}