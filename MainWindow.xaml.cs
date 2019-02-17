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
using Microsoft.Win32;

namespace SiemensPerformance
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public Func<double, string> Formatter { get; set; }
        private DataGenerator generator;
        private TabControl tabs;

        private string data;

        public MainWindow()
        {

            generator = new DataGenerator();
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
            
            tabs = (TabControl)this.FindName("logNav");
            
            /*
            int numTabs = 0;
            for(int i = 0; i < numTabs; i++)
            {
                TabItem tab = GenerateTabItem(generator.getProcessVars(), "NEW Name");
                tabs.Items.Insert(tabs.Items.Count - 1, tab);
            }
            */
            
        }

        
        private void Info_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

            // Called when a new log tab item is chosen
        private void LogNav_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            TabControl control = sender as TabControl;

            if (control.SelectedIndex == control.Items.Count - 1)
            {
                TabItem tab = GenerateTabItem();
                control.Items.Insert(control.Items.Count - 1, tab);
                control.SelectedIndex = control.Items.Count - 2;
            }

            //TabItem tab = GenerateTabItem(generator.getProcessVars(), "NEW Name");

        }


        // Generate table for queries
        private DataGrid GenerateTable(String[] columns)
        {
            DataGrid grid = new DataGrid();

            for(int i = 0; i < columns.Length; i++)
            {
                DataGridTextColumn col = new DataGridTextColumn();

                // TODO: replace with the column name
                col.Header = columns[i];
                // should be able to bind data to the row
                grid.Columns.Add(col);
            }

            return grid;
        }

        //Generates and returns a new TabItem object
        private TabItem GenerateTabItem()
        {

            TabItem tab = new TabItem();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".utr";
            ofd.Filter = "Text files (*.utr)|*.utr";

            if (ofd.ShowDialog() == true)
            {
                this.data = generator.getJsonString(ofd);
            }
            tab.Header = generator.fileName;
            
            tab.Content = GenerateTable(generator.getProcessVars());

            ScrollViewer sv = new ScrollViewer();
            TextBlock block = new TextBlock();
            block.Text = this.data;
            sv.Content = block;

            tab.Content = sv;

            return tab;
        }
    }
}
