using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace transportation_problem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<List<int>> list = new List<List<int>>(Int32.Parse(rows.Text));

            for (int i = 0; i < Int32.Parse(rows.Text); i++)
            {
                for (int j = 0; j < Int32.Parse(columns.Text); j++)
                {
                    //list[i] = new List<int>();
                    list[i].Add(j);
                }
            }

            this.dataGrid1.ItemsSource = list;
        }
    }
}
