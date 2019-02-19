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
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using System.Windows.Interactivity;
using System.IO;

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
            //OLD display JSON text
            //TextBlock block = new TextBlock();
            //block.Text = this.data;
            //sv.Content = block;
            //tab.Content = sv;

            //Convert JSON to DataGrid
            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(this.data);
            DataTable firstTable = dataSet.Tables[0];
            DataGrid dataGrid = new DataGrid();
            dataGrid.ItemsSource = firstTable.DefaultView;
            tab.Content = dataGrid;

            //Tab dropdown menu
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem1 = new MenuItem();
            contextMenu.Items.Add(menuItem1);
            menuItem1.Header = "Rename";
            menuItem1.Click += delegate { Rename(tab); };

            MenuItem menuItem2 = new MenuItem();
            contextMenu.Items.Add(menuItem2);
            menuItem2.Header = "Save";
            menuItem2.Click += delegate { Save(tab, this.data); };

            MenuItem menuItem3 = new MenuItem();
            contextMenu.Items.Add(menuItem3);
            menuItem3.Header = "Save As...";
            menuItem3.Click += delegate { SaveAs(tab, this.data); };

            MenuItem menuItem4 = new MenuItem();
            contextMenu.Items.Add(menuItem4);
            menuItem4.Header = "Close";
            menuItem4.Click += delegate { Close(tab); };

            tab.ContextMenu = contextMenu;

            return tab;
        }

        private void Rename(TabItem tab)
        {
            string name = new InputBox("Name").ShowDialog();
            if(name != "") {
                tab.Header = name;
            }
        }

        private void Save(TabItem tab, String json)
        {
            Console.WriteLine("Save dialaog");
        }

        private void SaveAs(TabItem tab, String json)
        {
            //set default file name to tab header
            String defaultName = tab.Header.ToString();
            //Remove .utr extention if present
            if (defaultName.EndsWith(".utr"))
            {
                defaultName = defaultName.Substring(0, defaultName.Length - 4);
            }

            //Create save dialog
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = defaultName;
            dlg.DefaultExt = ".json";
            dlg.Filter = "Json files (.json)|*.json";

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                File.WriteAllText(filename, json);
            }

        }

        private void Close(TabItem tab)
        {
            /*//TODO - fix this
            if (tab != null)
            {
                // find the parent tab control
                TabControl tabControl = tab.Parent as TabControl;

                if (tabControl != null)
                    tabControl.Items.Remove(tab); // remove tabItem
            }*/
        }
    }
}
