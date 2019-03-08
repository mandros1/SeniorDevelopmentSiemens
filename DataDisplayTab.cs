using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Controls;
using Newtonsoft.Json;
using LiveCharts.Wpf;
using LiveCharts;
using LiveCharts.Configurations;

namespace SiemensPerformance
{
    class DataDisplayTab : TabItem
    {
        private DataGenerator generator = new DataGenerator();
        private DataGrid dataGrid;
        private DataTable dataTable;
        private CartesianChart ch;
        public Boolean displayable {get; set;}
        private ComboBox box;
        private ComboBox id_box;
        
        public DataDisplayTab()
        {
            //Open File
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".utr";
            ofd.Filter = "Text files (*.utr)|*.utr";

            //Get Data
            if (ofd.ShowDialog() == true)
            {
                generator.getJsonString(ofd);
                displayable = true;
            }
            else
            {
                displayable = false;
                return;
            }

            TabControl tc = new TabControl();
            TabItem table = new TabItem();

            //Create table tab
            this.Header = generator.fileName;
            table.Header = "Table";

            table.Content = GenerateTable(generator.processVariables);

            ScrollViewer sv = new ScrollViewer();
            dataTable = ConvertListToDataTable(generator.dlist, generator.processVariables);
            dataGrid = new DataGrid();
            dataGrid.ItemsSource = dataTable.DefaultView;
            dataGrid.IsReadOnly = true;
            table.Content = dataGrid;
   
            //Create query tab
            TabItem query = new TabItem();

            Grid queryGrid = new Grid();
            queryGrid.Height = 330;
            queryGrid.Width = 681;
            queryGrid.Margin = new System.Windows.Thickness(0, 0, 0, 0);
            queryGrid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            queryGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            Label label = new Label();
            label.Content = "Select:";
            label.Margin = new System.Windows.Thickness(29, 15, 0, 0);
            label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            queryGrid.Children.Add(label);

            box = new ComboBox();
            box.ItemsSource = generator.getDistinctProcessNames();
            box.Name = "procName";
            box.Margin = new System.Windows.Thickness(86, 15, 0, 0);
            box.Width = 120;
            box.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            box.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            box.SelectionChanged += procName_SelectionChanged;
            queryGrid.Children.Add(box);

            id_box = new ComboBox();
            id_box.ItemsSource = generator.getDistinctProcessIDs(null);
            id_box.Name = "procID";
            id_box.Margin = new System.Windows.Thickness(268, 15, 0, 0);
            id_box.Width = 120;
            id_box.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            id_box.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            queryGrid.Children.Add(id_box);
            
            Label label2 = new Label();
            label2.Content = "Between:";
            label2.Margin = new System.Windows.Thickness(15, 46, 0, 0);
            label2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            label2.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            label2.Width = 120;
            queryGrid.Children.Add(label2);

            TextBox txtBox = new TextBox();
            txtBox.Height = 23;
            txtBox.TextWrapping = System.Windows.TextWrapping.Wrap;
            txtBox.Text = "Starting time";
            txtBox.Width = 120;
            txtBox.Margin = new System.Windows.Thickness(86, 46, 475, 258);
            queryGrid.Children.Add(txtBox);
               

            TextBox txtBox1 = new TextBox();
            txtBox1.Height = 23;
            txtBox1.TextWrapping = System.Windows.TextWrapping.Wrap;
            txtBox1.Text = "Ending time";
            txtBox1.Width = 120;
            txtBox1.Margin = new System.Windows.Thickness(268, 46, 293, 258);
            queryGrid.Children.Add(txtBox1);

            Label label3 = new Label();
            label3.Content = "and";
            label3.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            label3.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            label3.Margin = new System.Windows.Thickness(224, 46, 0, 0);
            label3.RenderTransformOrigin = new System.Windows.Point(-0.044, 0.538);
            queryGrid.Children.Add(label3);

            Label label4 = new Label();
            label4.Content = "Where";
            label4.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            label4.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            label4.Margin = new System.Windows.Thickness(26, 77, 0, 0);
            label4.RenderTransformOrigin = new System.Windows.Point(0.477, 2.731);
            queryGrid.Children.Add(label4);
            

            ComboBox box2 = new ComboBox();
            box2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            box2.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            box2.Width = 120;
            box2.Margin = new System.Windows.Thickness(86, 77, 0, 0);
            queryGrid.Children.Add(box2);


            Label label5 = new Label();
            label5.Content = "equals";
            label5.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            label5.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            label5.Margin = new System.Windows.Thickness(217, 77, 0, 0);
            label5.RenderTransformOrigin = new System.Windows.Point(0.477, 2.731);
            queryGrid.Children.Add(label5);
            

            TextBox txtBox2 = new TextBox();
            txtBox2.Height = 23;
            txtBox2.TextWrapping = System.Windows.TextWrapping.Wrap;
            txtBox2.Text = "Value";
            txtBox2.Width = 120;
            txtBox2.Margin = new System.Windows.Thickness(268, 77, 293, 227);
            queryGrid.Children.Add(txtBox2);

            Button but = new Button();
            but.IsDefault = true;
            but.Content = "Run";
            but.Margin = new System.Windows.Thickness(174, 133, 425, 168);
            queryGrid.Children.Add(but);

            query.Header = "Query";
            query.Content = queryGrid;


            //Create Graph tab
            TabItem graph = new TabItem();

            graph.Content = PopulateGraph("Cocos", "26204", "CPU");
            graph.Header = "Graph";

            //Tab dropdown menu
            //Rename Tab
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem1 = new MenuItem();
            contextMenu.Items.Add(menuItem1);
            menuItem1.Header = "Rename";
            menuItem1.Click += delegate { Rename(); };

            /* TODO Make this work correctly
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
            //graph.ContextMenu = contextMenu2;
            graph.ContextMenu = new ContextMenu();
            table.ContextMenu = new ContextMenu();
            query.ContextMenu = new ContextMenu();
            tc.Items.Insert(0, table);
            tc.Items.Insert(1, graph);
            tc.Items.Insert(2, query);
            this.Content = tc;
        }

        private void procName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            string procName = (string) combo.SelectedItem;
            id_box.ItemsSource = generator.getDistinctProcessIDs(procName);
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

        private Wpf.CartesianChart.Using_DateTime.DateTime PopulateGraph(string processName, string processID, string variable)
        {
            ChartValues<DateModel> data = new ChartValues<DateModel>();
            
            int variableIndex = Array.IndexOf(generator.processVariables, variable);
            

            //Get data
            foreach (var array in generator.getProcessData(processName, processID))
            {
                try
                {
                    Double value = Double.Parse(array[variableIndex]);
                    DateTime timeStamp = DateTime.ParseExact(array[0], "yyyy/MM/dd-HH:mm:ss.ffffff", null);
                    data.Add(new DateModel
                    {
                        DateTime = timeStamp,
                        Value = value
                    });
                }
                catch (IndexOutOfRangeException t) { }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return new Wpf.CartesianChart.Using_DateTime.DateTime(data);
        }
    }
}
