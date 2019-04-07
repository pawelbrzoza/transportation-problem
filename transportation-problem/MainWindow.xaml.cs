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
using transportation_problem.Common;
using System.Data;

namespace transportation_problem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private MainProperties properties;

        public MainWindow()
        {
            InitializeComponent();
            properties = new MainProperties();
        }
    
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {}

        private void CreateTable()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            System.Data.DataTable dtPodaz = new System.Data.DataTable();
            System.Data.DataTable dtPopyt = new System.Data.DataTable();
            //main
            DataColumn[] dostawcy = new DataColumn[(int)properties.Dimensions.X];
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
            {
                dostawcy[i] = new DataColumn("D" + (i + 1), typeof(string));

                dt.Columns.Add(dostawcy[i]);
            }

            DataRow[] odbiorcy = new DataRow[(int)properties.Dimensions.Y];
            for (int i = 0; i < (int)properties.Dimensions.Y; i++)
            {
                odbiorcy[i] = dt.NewRow();
                for (int j = 0; j < (int)properties.Dimensions.X; j++)
                    odbiorcy[i][j] = "0";

                dt.Rows.Add(odbiorcy[i]);
            }
            //podaz
            DataColumn[] podazCol = new DataColumn[(int)properties.Dimensions.X];
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
            {
                podazCol[i] = new DataColumn("D" + (i+1), typeof(string));
                dtPodaz.Columns.Add(podazCol[i]);
            }
                
            DataRow podazRow = dtPodaz.NewRow();
            for (int j = 0; j < (int)properties.Dimensions.X; j++)
                podazRow[j] = "0";
            dtPodaz.Rows.Add(podazRow);

            //podaz
            DataColumn popytCol = new DataColumn("O1", typeof(string));
            dtPopyt.Columns.Add(popytCol);

            DataRow[] popytRow = new DataRow[(int)properties.Dimensions.Y];
            for (int i = 0; i < (int)properties.Dimensions.Y; i++)
            {
                popytRow[i] = dtPopyt.NewRow();
                popytRow[i][0] = "0";
                dtPopyt.Rows.Add(popytRow[i]);
            }

            myDataGridPodaz.ItemsSource = dtPodaz.DefaultView;
            myDataGridPopyt.ItemsSource = dtPopyt.DefaultView;
            myDataGrid.ItemsSource = dt.DefaultView;
        }

        private void createTable_Button_Click(object sender, RoutedEventArgs e)
        {
            properties.Dimensions.X = Convert.ToDouble(columns.Text);
            properties.Dimensions.Y = Convert.ToDouble(rows.Text);
            CreateTable();
        }

        private void dg_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = ("O" + (e.Row.GetIndex() + 1)).ToString();
        }

        private void countSupplyAndDemand_Button_Click(object sender, RoutedEventArgs e)
        {
            int sum = 0;

            foreach (DataRowView row in myDataGridPopyt.Items)
            {
                sum += Convert.ToInt32(row.Row.ItemArray[0].ToString());
            }
            allDemand.Content = Convert.ToString(sum);

            sum = 0;

            DataRowView row2 = (DataRowView)myDataGridPodaz.Items.GetItemAt(0);
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
            {
                sum += Convert.ToInt32(row2.Row.ItemArray[i].ToString());
            }

            allSupply.Content = Convert.ToString(sum);
        }
    }
}
