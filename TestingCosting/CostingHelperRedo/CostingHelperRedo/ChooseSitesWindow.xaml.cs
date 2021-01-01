using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CostingHelper
{
    ///Window that lets the user choose which sites (only google shopping included for now)
    ///and how many search results from each site (current max of 20).
    public partial class ChooseSitesWindow : Window
    {
        public bool starting = true;
        public ChooseSitesWindow()
        {
            InitializeComponent();

            //Default, google 3. I need to get that from 2 arrays from the main screen. I'll just read to
            //this page every time I open it. When I change a value I'll update mainwindow.array

            List<int> selected = MainWindow.selectedSites;
            List<int> nums = MainWindow.numResultsList;

            starting = true;
            Check1.IsChecked = selected.Contains(0);
            ChooseNum1.SelectedIndex = nums[0];

            /*Check2.IsChecked = selected.Contains(1);
            Check3.IsChecked = selected.Contains(2);
            Check4.IsChecked = selected.Contains(3);
            Check5.IsChecked = selected.Contains(4);

            ChooseNum2.SelectedIndex = nums[1];
            ChooseNum3.SelectedIndex = nums[2];
            ChooseNum4.SelectedIndex = nums[3];
            ChooseNum5.SelectedIndex = nums[4];*/

            starting = false;

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.selectedSites.Add(0);
            }
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.selectedSites.Remove(0);
            }
        }

        /*
        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.selectedSites.Add(1);
            }

        }
        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.selectedSites.Remove(1);
            }
        }

        private void CheckBox_Checked_2(object sender, RoutedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.selectedSites.Add(2);
            }

        }
        private void CheckBox_Unchecked_2(object sender, RoutedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.selectedSites.Remove(2);
            }
        }

        private void CheckBox_Checked_3(object sender, RoutedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.selectedSites.Add(3);
            }

        }
        private void CheckBox_Unchecked_3(object sender, RoutedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.selectedSites.Remove(3);
            }
        }

        private void CheckBox_Checked_4(object sender, RoutedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.selectedSites.Add(4);
            }

        }
        private void CheckBox_Unchecked_4(object sender, RoutedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.selectedSites.Remove(4);
            }
        }*/

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.numResultsList[0] = ChooseNum1.SelectedIndex;
            }

        }
        /*
        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.numResultsList[1] = ChooseNum2.SelectedIndex;
            }
        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.numResultsList[2] = ChooseNum3.SelectedIndex;
            }
        }

        private void ComboBox_SelectionChanged_3(object sender, SelectionChangedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.numResultsList[3] = ChooseNum4.SelectedIndex;
            }
        }

        private void ComboBox_SelectionChanged_4(object sender, SelectionChangedEventArgs e)
        {
            if (!starting)
            {
                MainWindow.numResultsList[4] = ChooseNum5.SelectedIndex;
            }
        }
        */
    }
}