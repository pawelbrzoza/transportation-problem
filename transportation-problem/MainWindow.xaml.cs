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
using System.Collections;

namespace transportation_problem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private MainProperties properties;
        public struct Node
        {
            public int val;
            public Point dim;
        };
        Node[] arrCycle = new Node[4];

        private int?[,] transportCosts = new int?[10, 10];
        private int[] supply = new int[10];
        private int[] demand = new int[10];
        private int?[,] firstSolution = new int?[10, 10];
        private int?[,] optimizedSolution = new int?[10, 10];
        private int?[,] potentials = new int?[10, 10];
        private int?[,] firstSolutionWithCost = new int?[10, 10];
        private int?[] Ui = new int?[10]; //alfa
        private int?[] Vj = new int?[10]; //beta

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
            printFistSolution();
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

        private void printFistSolution()
        {
            Console.Out.WriteLine();
            Console.Out.WriteLine("First solution");
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
            {
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                {
                    Console.Out.Write(firstSolution[i, j].Value + " ");
                }
                Console.Out.WriteLine();
            }
            Console.Out.WriteLine();
            
            Console.Out.WriteLine("First solution with costs");
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
            {
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                {
                    Console.Out.Write(firstSolutionWithCost[i, j].Value + " ");
                }
                Console.Out.WriteLine();
            }
            Console.Out.WriteLine();
        }

        private void printOthers()
        {
            Console.Out.WriteLine("Alfa: ");
            for (int i = 0; i < (int)properties.Dimensions.Y; i++)
            {
                Console.Out.Write(Ui[i] + " ");
            }
            Console.Out.WriteLine();

            Console.Out.WriteLine("Beta: ");
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
            {
                Console.Out.Write(Vj[i] + " ");
            }
            Console.Out.WriteLine();

            Console.Out.WriteLine("Optimized Solution");
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
            {
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                {
                    Console.Out.Write(optimizedSolution[i, j].Value + " ");
                }
                Console.Out.WriteLine();
            }
            Console.Out.WriteLine();

            Console.Out.WriteLine("Potentials");
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
            {
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                {
                    Console.Out.Write(potentials[i, j].Value + " ");
                }
                Console.Out.WriteLine();
            }
            Console.Out.WriteLine();

        }

        private void countArrayCosts()
        {
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                    firstSolutionWithCost[i, j] = firstSolution[i, j] * transportCosts[i, j];

            int sum = 0;
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                    sum += (int)firstSolutionWithCost[i, j];
            firstCostLabel.Content = Convert.ToString(sum);
        }

        private void countArrayCostsAfterOpt()
        {
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                    firstSolutionWithCost[i, j] = firstSolution[i, j] * transportCosts[i, j];

            int sum = 0;
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                    sum += (int)firstSolutionWithCost[i, j];
            optimizedCostLabel.Content = Convert.ToString(sum);
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

        private void optimizeSolution_Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                {
                    optimizedSolution[i, j] = 0;
                    potentials[i, j] = 0;
                }

            countSupplyAndDemand_Button_Click(sender, e);
            countPotentials();
            Point? tempPoint = checkPotentials();
            if (tempPoint != null)
            {
                createCycle();
                transformCycle();
                changeSolution();
                countArrayCostsAfterOpt();
                printOthers();
            }
        }

        private void countPotentials()
        {
            //counting Ui and Vj
            for (int i = 0; i < Ui.Length; i++)
                Ui[i] = null;
            for (int i = 0; i < Vj.Length; i++)
                Vj[i] = null;

            Ui[0] = 0;
            int counterUi = 0, counterVj = 0;
            while (counterUi < (int)properties.Dimensions.Y && counterVj < (int)properties.Dimensions.X)
            {
                for (int i = 0; i < (int)properties.Dimensions.Y; i++)
                {
                    for (int j = 0; j < (int)properties.Dimensions.X; j++)
                    {
                        if (firstSolution[j, i] != 0 && Ui[i] != null)
                            Vj[j] = transportCosts[j, i] - Ui[i];
                    }
                }

                for (int i = 0; i < (int)properties.Dimensions.X; i++)
                {
                    for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                    {
                        if (firstSolution[i, j] != 0 && Vj[i] != null)
                            Ui[j] = transportCosts[i, j] - Vj[i]; //
                    }
                }

                counterUi = 0;
                counterVj = 0;
                for (int i = 0; i < Ui.Length; i++)
                    if(Ui[i] != null)
                        counterUi++;
                for (int i = 0; i < Vj.Length; i++)
                    if (Vj[i] != null)
                        counterVj++;
            }

            //counting potentials
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                    if (firstSolution[i, j] == 0)
                        potentials[i, j] = transportCosts[i,j] - (Ui[j] + Vj[i]);
        }

        private Point? checkPotentials()
        {
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                    if (potentials[i, j] < 0)
                    {
                        return new Point{ X = i, Y = j };
                    }

            return null;
        }

        private void createCycle()
        {
            arrCycle[0].val = 0;
            arrCycle[0].dim = (Point)checkPotentials();
            
            for (int i = 0; i < (int)properties.Dimensions.X; i++)
            {
                for (int j = 0; j < (int)properties.Dimensions.Y; j++)
                {
                    if ((arrCycle[0].dim.X == i && arrCycle[0].dim.Y == j) || firstSolution[i, j] == 0)
                        continue;

                    if (arrCycle[0].dim.Y == j || arrCycle[0].dim.X == i) // for Y ?
                    {
                        //1
                        arrCycle[1].val = firstSolution[i, j].Value;
                        arrCycle[1].dim.X = i;
                        arrCycle[1].dim.Y = j;

                        if (arrCycle[0].dim.X == (int)properties.Dimensions.X - 1)
                        {
                            //2
                            arrCycle[2].dim.X = arrCycle[1].dim.X - 1;
                            arrCycle[2].dim.Y = arrCycle[1].dim.Y;
                            arrCycle[2].val = firstSolution[(int)arrCycle[2].dim.X, (int)arrCycle[2].dim.Y].Value;

                            //3
                            arrCycle[3].dim.X = arrCycle[1].dim.X;
                            arrCycle[3].dim.Y = arrCycle[0].dim.Y;
                            arrCycle[3].val = firstSolution[(int)arrCycle[3].dim.X, (int)arrCycle[3].dim.Y].Value;
                        }
                        else
                        {
                            //2
                            arrCycle[2].dim.X = arrCycle[1].dim.X + 1;
                            arrCycle[2].dim.Y = arrCycle[1].dim.Y;
                            arrCycle[2].val = firstSolution[(int)arrCycle[2].dim.X, (int)arrCycle[2].dim.Y].Value;

                            //3
                            arrCycle[3].dim.X = arrCycle[2].dim.X;
                            arrCycle[3].dim.Y = arrCycle[0].dim.Y;
                            arrCycle[3].val = firstSolution[(int)arrCycle[3].dim.X, (int)arrCycle[3].dim.Y].Value;
                        }

                        if (arrCycle[2].val != 0 && arrCycle[3].val != 0)
                            break;

                    }
                    else
                    {
                        //2
                        arrCycle[2].val = firstSolution[i, j].Value;
                        arrCycle[2].dim.X = i;
                        arrCycle[2].dim.Y = j;

                        //1
                        arrCycle[1].dim.X = arrCycle[2].dim.X;
                        arrCycle[1].dim.Y = arrCycle[0].dim.Y;
                        arrCycle[1].val = firstSolution[(int)arrCycle[1].dim.X, (int)arrCycle[1].dim.Y].Value;

                        //3
                        arrCycle[3].dim.X = arrCycle[0].dim.X;
                        arrCycle[3].dim.Y = arrCycle[2].dim.Y;
                        arrCycle[3].val = firstSolution[(int)arrCycle[3].dim.X, (int)arrCycle[3].dim.Y].Value;

                        if (arrCycle[1].val != 0 && arrCycle[3].val != 0)
                            break;
                    }

                }
                if (arrCycle[1].val != 0 && arrCycle[2].val != 0 && arrCycle[3].val != 0)
                    break;
            }
        }

        private void transformCycle()
        {
            //min val
            int min = arrCycle[3].val;
            int minId = 3;
            if (arrCycle[1].val < min)
            {
                min = arrCycle[1].val;
                minId = 1;
            }
            if (arrCycle[2].val < min)
            {
                min = arrCycle[2].val;
                minId = 2;
            }
            //asign
            if (minId == 3)
            {
                arrCycle[3].val -= min;
                arrCycle[2].val += min;
                arrCycle[1].val -= min;
                arrCycle[0].val += min;
            }
            else if (minId == 2)
            {
                arrCycle[3].val += min;
                arrCycle[2].val -= min;
                arrCycle[1].val += min;
            }
            else
            {
                arrCycle[3].val -= min;
                arrCycle[2].val += min;
                arrCycle[1].val -= min;
                arrCycle[0].val += min;
            }
        }

        private void changeSolution()
        {
            for (int i = 0; i < 4; i++)
                firstSolution[(int)arrCycle[i].dim.X, (int)arrCycle[i].dim.Y] = arrCycle[i].val;
        }
    }
}
