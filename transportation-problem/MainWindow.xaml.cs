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
        private int[,] transportCosts = new int[10, 10];
        private int[] supply = new int[10];
        private int[] demand = new int[10];
        private int[,] firstSolution = new int[10, 10];
        private int[,] firstSolutionWithCost = new int[10, 10];

        public MainWindow()
        {
            InitializeComponent();
            properties = new MainProperties();
        }

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
                podazCol[i] = new DataColumn("D" + (i + 1), typeof(string));
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

            myDataGridSupply.ItemsSource = dtPodaz.DefaultView;
            myDataGridDemand.ItemsSource = dtPopyt.DefaultView;
            myDataGrid.ItemsSource = dt.DefaultView;
        }

        private void createTable_Button_Click(object sender, RoutedEventArgs e)
        {
            properties.Dimensions.X = Convert.ToDouble(columnsTextBox.Text);
            properties.Dimensions.Y = Convert.ToDouble(rowsTextBox.Text);
            CreateTable();
        }

        private void dg_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = ("O" + (e.Row.GetIndex() + 1)).ToString();
        }

        private void countSupplyAndDemand_Button_Click(object sender, RoutedEventArgs e)
        {
            parseToArrays();

            int sum = 0;
            foreach (int x in demand)
                sum += x;
            allDemandLabel.Content = Convert.ToString(sum);

            sum = 0;
            foreach (int x in supply)
                sum += x;

            allSupplyLabel.Content = Convert.ToString(sum);
            IsBalanced();
            countFirstSolution();
            if (IsPremitted())
            {
                countArrayCosts();
            }
        }

        private void IsBalanced()
        {
            if (allSupplyLabel.Content.Equals(allDemandLabel.Content))
                isBalancedLabel.Content = "YES";
            else
                isBalancedLabel.Content = "NO";
        }

        private void parseToArrays()
        {
            // supply
            DataRowView row = (DataRowView)myDataGridSupply.Items.GetItemAt(0);
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
            {
                supply[i] = Convert.ToInt32(row.Row.ItemArray[i].ToString());
            }

            // demand
            int p = 0;
            foreach (DataRowView column in myDataGridDemand.Items)
            {
                demand[p++] += Convert.ToInt32(column.Row.ItemArray[0].ToString());
            }

            //transportCosts
            for (int i = 0; i < (int)properties.Dimensions.Y; i++)
            {
                DataRowView tempRow = (DataRowView)myDataGrid.Items.GetItemAt(i);
                for (int j = 0; j < (int)properties.Dimensions.X; j++)
                {
                    transportCosts[j, i] = Convert.ToInt32(tempRow.Row.ItemArray[j].ToString());
                }
            }
        }

        private void countFirstSolution()
        {
            int[] tempSupply = new int[10];
            int[] tempDemand = new int[10];
            tempSupply = supply;
            tempDemand = demand;

            int i = 0, j = 0, guardI = 0, guardJ = 0;

            while( guardI < (int)properties.Dimensions.X && guardJ < (int)properties.Dimensions.Y )
            {
                if(tempSupply[i] > tempDemand[j])
                {
                    firstSolution[i, j] = tempDemand[j];
                    tempSupply[i] -= tempDemand[j];
                    tempDemand[j] = 0;
                    i++;
                }
                else
                {
                    firstSolution[i, j] = tempSupply[i];
                    tempDemand[j] -= tempSupply[i];
                    tempSupply[i] = 0;
                    j++;
                }

                if(j == (int)properties.Dimensions.Y)
                {
                    j = guardJ;
                    guardI++;
                    i++;
                }

                if (i == (int)properties.Dimensions.X)
                {
                    i = guardI;
                    guardJ++;
                    j++;
                }
            }
        }

        private void countArrayCosts()
        {
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                    firstSolutionWithCost[i, j] = firstSolution[i, j] * transportCosts[i, j];

            int sum = 0;
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                    sum += firstSolutionWithCost[i, j];
            costLabel.Content = Convert.ToString(sum);
        }

        private bool IsPremitted()
        {
            int N = 0;
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                    if (firstSolution[i, j] != 0)
                        N++;
            if (N == ((int)properties.Dimensions.X + (int)properties.Dimensions.Y - 1))
            {
                conditionLabel.Content = "YES";
                return true;
            }
            else
            {
                conditionLabel.Content = "NO";
                return false;
            }
        }
    }
}
