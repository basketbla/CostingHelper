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
using System.Windows.Shapes;

namespace CostingHelper
{
    ///Lets user type/paste in items instead of reading from a file.
    public partial class DirectEntryWindow : Window
    {
        public static string directText = MainWindow.directText;

        public DirectEntryWindow()
        {
            InitializeComponent();
            DirectEntryTextBox.Text = directText;
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            directText = DirectEntryTextBox.Text;
            this.Visibility = Visibility.Hidden;
            MainWindow.directText = directText;
        }

    }
}
