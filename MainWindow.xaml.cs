using System;
using System.Data;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MahApps.Metro.Controls;

namespace SiemensPerformance
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public Func<double, string> Formatter { get; set; }

        public MainWindow()
        {
            
            InitializeComponent();

            // Graph setup
            /* SeriesCollection CPU = new SeriesCollection
             {
             new LineSeries
             {
                 Title = "CPU Usage",

                 Values = new ChartValues<double> { 3, 5, 7, 4 },

                 Fill = Brushes.Transparent,

             },
         };
             RankGraph.Series = CPU;


             SeriesCollection Memory = new SeriesCollection
             {
             new LineSeries
             {
                 Title = "Memory Usage",

                 Values = new ChartValues<double> { 30, 5, 20, 4 },

                 Fill = Brushes.Transparent,

             },
         };
             RankGraph2.Series = Memory;*/

            // Tabs setup

            TabControl tabs = (TabControl)this.FindName("logNav");
            // TODO: retrieve from backend
            int numTabs = 3;
            for(int i = 0; i < numTabs; i++)
            {
                TabItem tab = GenerateTabItem(i+1);
                tabs.Items.Insert(tabs.Items.Count - 1, tab);
            }
        }

        private void Info_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        // Called when a new log tab item is chosen
        private void LogNav_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl control = sender as TabControl;
            int currTab = control.SelectedIndex;
            if(currTab == control.Items.Count-1)
            {
                TabItem tab = GenerateTabItem(control.Items.Count);

                control.Items.Insert(control.Items.Count - 1, tab);
                control.SelectedIndex = control.Items.Count - 2;
            }
        }

        // Generate table for queries
        private DataGrid GenerateTable()
        {
            DataGrid grid = new DataGrid();

            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Field 1";
            grid.Columns.Add(col1);
            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Field 2";
            grid.Columns.Add(col2);
            DataGridTextColumn co3 = new DataGridTextColumn();
            co3.Header = "Field 3";
            grid.Columns.Add(co3);


            return grid;
        }

        //Generates and returns a new TabItem object
        private TabItem GenerateTabItem(int tabNum)
        {
            TabItem tab = new TabItem();
            tab.Header = "Log " + (tabNum).ToString();

            tab.Content = GenerateTable();

            return tab;
        }
    }
}
