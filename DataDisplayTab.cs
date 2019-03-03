using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using LiveCharts.Wpf;
using LiveCharts;
using LiveCharts.Configurations;
using System.Windows.Media;
using System.Runtime.Serialization;

namespace SiemensPerformance
{
    class DataDisplayTab : TabItem
    {
        private DataGenerator generator = new DataGenerator();
        private DataGrid dataGrid;
        private DataTable dataTable;

        public DataDisplayTab()
        {
            TabControl tc = new TabControl();
            TabItem table = new TabItem();

            //Open File
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".utr";
            ofd.Filter = "Text files (*.utr)|*.utr";

            //Get Data
            if (ofd.ShowDialog() == true)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                generator.getJsonString(ofd);
                watch.Stop();
                Console.WriteLine("TIME ELAPSED: " + watch.ElapsedMilliseconds);
            }

            //Create table tab
            this.Header = generator.fileName;
            table.Header = "Table";

            table.Content = GenerateTable(generator.getProcessVars());

            ScrollViewer sv = new ScrollViewer();
            dataTable = ConvertListToDataTable(generator.dlist, generator.processVariables);
            //dataTable = ConvertListToDataTable(generator.getProcessData("CM.DMMonitoringTaskflow_2feaaba2c07a43d0b41f92319eac8bfe.DMMonitoringTaskBE(29668)"), generator.processVariables);
            //dataTable = ConvertListToDataTable(generator.getProcessData("syngo.MR.SaveLogHookMrawp(27696)"), generator.processVariables);
            dataGrid = new DataGrid();
            dataGrid.ItemsSource = dataTable.DefaultView;
            dataGrid.IsReadOnly = true;
            table.Content = dataGrid;
            
            //GRAPH
            ChartValues <DateModel> data = new ChartValues<DateModel>();
            foreach (var array in generator.getProcessData("syngo.MR.SaveLogHookMrawp(27696)"))
            {
                try
                {
                    //Console.WriteLine(array);
                    Double cpuL = Double.Parse(array[8]);
                    DateTime timeStamp = DateTime.ParseExact(array[0], "yyyy/MM/dd-HH:mm:ss.ffffff", null);
                    data.Add(new DateModel
                    {
                        DateTime = timeStamp,
                        Value = cpuL
                    });
                }
                catch (IndexOutOfRangeException t) { }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            var dayConfig = Mappers.Xy<DateModel>()
                .X(dayModel => dayModel.DateTime.Ticks)
                .Y(dayModel => dayModel.Value);

            //Create Graph tab
            TabItem graph = new TabItem();
            CartesianChart ch = new CartesianChart();
            ch.Series = new SeriesCollection(dayConfig);

            LineSeries line = new LineSeries
            {
                Values = data
            };

            ch.Series.Add(line);
            graph.Header = "Graph";
            graph.Content = ch;

            //ch.AxisX[0].LabelFormatter = value => new System.DateTime((long)(value * TimeSpan.FromHours(1).Ticks)).ToString("t");

            //Tab dropdown menu
            //Rename Tab
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem1 = new MenuItem();
            contextMenu.Items.Add(menuItem1);
            menuItem1.Header = "Rename";
            menuItem1.Click += delegate { Rename(); };

            /* TODO Move to only for queries
            //Save data to Json
            MenuItem menuItem2 = new MenuItem();
            contextMenu.Items.Add(menuItem2);
            menuItem2.Header = "Save";
            menuItem2.Click += delegate { Save(); };
            */

            //Close Tab
            MenuItem menuItem2 = new MenuItem();
            contextMenu.Items.Add(menuItem2);
            menuItem2.Header = "Close";
            menuItem2.Click += delegate { Close(); };

            //TODO - figure out why this is also being applied to child tab elements
            this.ContextMenu = contextMenu;
            table.ContextMenu = null;//Not working
            graph.ContextMenu = null;
            tc.Items.Insert(0, table);
            tc.Items.Insert(1, graph);
            this.Content = tc;
        }


        //Generate DataGrid from data
        private DataGrid GenerateTable(String[] columns)
        {
            DataGrid grid = new DataGrid();

            for (int i = 0; i < columns.Length; i++)
            {
                DataGridTextColumn col = new DataGridTextColumn();

                // TODO: replace with the column name
                col.Header = columns[i];
                // should be able to bind data to the row
                grid.Columns.Add(col);
            }
            return grid;
        }

        //Converts list to data table
        private static DataTable ConvertListToDataTable(List<string[]> list, string[] columns)
        {
            DataTable table = new DataTable();

            // Get max columns.
            int columnsNum = columns.Length;
            for (int i = 0; i < columnsNum; i++)
            {
                table.Columns.Add(columns[i]);
            }

            foreach (var array in list)
            {
                if (array.Length == columnsNum)
                {
                    table.Rows.Add(array);
                }
            }
            return table;
        }

        //Renames a Tab
        private void Rename()
        {
            string name = new InputBox("Name").ShowDialog();
            if (name != "")
            {
                this.Header = name;
            }
        }

        //Saves data from a tab to json file
        private void Save()
        {
            //set default file name to tab header
            String defaultName = this.Header.ToString();
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

            string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                File.WriteAllText(filename, json);
            }
        }

        //Close Tab
        private void Close()
        {
            if (this != null)
            {
                // find the parent tab control
                TabControl tabControl = this.Parent as TabControl;

                if (tabControl != null)
                {
                    tabControl.SelectedIndex = tabControl.Items.IndexOf(this) - 1;  // Selects the tab before the closing tab
                    tabControl.Items.Remove(this); // Removes the current tab
                }
            }
        }
    }
}
