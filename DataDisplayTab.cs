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
        private string[] filterByArray = { "Process", "Global(0)", "Global(_Total)" };
        private List<ComboBox> parameterNamesComboBox = new List<ComboBox>();

        public Boolean displayable {get; set;}
        private StackPanel mainStackPanel;
        private StackPanel filterStackPanel;
        private ComboBox processNameCB;
        private ComboBox processIdCB;
        private ComboBox filterCB;
        private ComboBox selectComboBox;
        private ComboBox finalSelectCB;
        private ComboBox finalWhereCB;
        private ComboBox finalAndCB;
        private ComboBox finalBetweenCB;

        // reusable components
        private StackPanel stackPanel;
        private StackPanel stackPanel2;
        private Label label;
        private ComboBox comboBox;
        private DockPanel dockPanel;
        private TextBox textBox;


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

            var screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            var screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;

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

            processNameCB = new ComboBox();
            processNameCB.ItemsSource = generator.getDistinctProcessNames();
            processNameCB.Name = "procName";
            processNameCB.Margin = new System.Windows.Thickness(86, 15, 0, 0);
            processNameCB.Width = 120;
            processNameCB.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            processNameCB.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            processNameCB.SelectionChanged += procName_SelectionChanged;
            queryGrid.Children.Add(processNameCB);

            processIdCB = new ComboBox();
            processIdCB.ItemsSource = generator.getDistinctProcessIDs(null);
            processIdCB.Name = "procID";
            processIdCB.Margin = new System.Windows.Thickness(268, 15, 0, 0);
            processIdCB.Width = 120;
            processIdCB.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            processIdCB.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            queryGrid.Children.Add(processIdCB);
            
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

            graph.Content = PopulateGraph("IKM_AT_ATServiceManager", "6700", "HC");
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

            TabItem query2 = generateQueryTabItem();

            this.ContextMenu = contextMenu;
            graph.ContextMenu = new ContextMenu();
            table.ContextMenu = new ContextMenu();
            query.ContextMenu = new ContextMenu();
            query2.ContextMenu = new ContextMenu();
            tc.Items.Insert(0, table);
            tc.Items.Insert(1, graph);
            tc.Items.Insert(2, query);
            tc.Items.Insert(3, query2);
            //tc.Items.Insert(2, FileSpecificGraphTabGenerator());
            this.Content = tc;
        }

        private void procName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            string procName = (string) combo.SelectedItem;
            processIdCB.ItemsSource = generator.getDistinctProcessIDs(procName);
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

        private Wpf.CartesianChart.ZoomingAndPanning.ZoomingAndPanning PopulateGraph(string processName, string processID, string variable)
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
                catch (IndexOutOfRangeException t) {
                    Console.WriteLine(t);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return new Wpf.CartesianChart.ZoomingAndPanning.ZoomingAndPanning(data);
        }

       
        ///**
        // * This creates a StackPanel with a label and combobox generated to be placed inside another StackPanel as a sub-element
        // * @stackPanel is a StackPanel object
        // */
        //private StackPanel  ( string labelText, 
        //                                                int comboBoxWidth,
        //                                                System.Windows.Thickness labelMargins,
        //                                                System.Windows.Thickness comboboxMargins)
        //{
        //    stackPanel = new StackPanel();
        //    stackPanel.Orientation = Orientation.Horizontal;

        //    label = new Label();
        //    label.Content = labelText;
        //    label.Margin = labelMargins;
        //    stackPanel.Children.Add(label);

        //    comboBox = new ComboBox();
        //    comboBox.Margin = comboboxMargins;
        //    comboBox.Width = comboBoxWidth;
            
        //    stackPanel.Children.Add(comboBox);

        //    return stackPanel;
        //}

       
        /**
         * Main
         */
        private TabItem generateQueryTabItem()
        {
            TabItem tab = new TabItem();
            tab.Header = "Query2";

            dockPanel = baseDockPanel();
            
            tab.Content = dockPanel;
            return tab;
        }

        private DockPanel baseDockPanel()
        {
            dockPanel = new DockPanel();

            filterStackPanel = new StackPanel();
            filterStackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            filterStackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            filterStackPanel.Orientation = Orientation.Horizontal;
            label = new Label();
            label.Margin = new System.Windows.Thickness(10, 0, 0, 0);
            label.Content = "Filter By:";
            label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            filterStackPanel.Children.Add(label);

            filterCB = new ComboBox();
            filterCB.Name = "filterComboBox";
            filterCB.Margin = new System.Windows.Thickness(10, 0, 10, 0);
            filterCB.Width = 100;
            filterCB.Height = 30;
            filterCB.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            filterCB.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            filterCB.ItemsSource = filterByArray;
            filterCB.SelectionChanged += filter_SelectionChanged;
            filterStackPanel.Children.Add(filterCB);

            DockPanel.SetDock(filterStackPanel, Dock.Top);
            dockPanel.Children.Add(filterStackPanel);

            mainStackPanel = new StackPanel();
            mainStackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            mainStackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            mainStackPanel.Orientation = Orientation.Horizontal;
            mainStackPanel.Margin = new System.Windows.Thickness(0, 10, 0, 0);

            dockPanel.Children.Add(mainStackPanel);

            return dockPanel;
        }

        private void filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox = (ComboBox)sender;

            // remove all previous existing children
            while(filterStackPanel.Children.Count > 2)
            {
                filterStackPanel.Children.RemoveAt(filterStackPanel.Children.Count - 1);
            }
            while (mainStackPanel.Children.Count > 0)
            {
                mainStackPanel.Children.RemoveAt(mainStackPanel.Children.Count - 1);
            }

            string filterValue = (string)comboBox.SelectedItem;
            
            if (filterValue == "Process")
            {
                stackPanel = processFilterStackPanelGenerator(
                    new System.Windows.Thickness(20, 0, 0, 0),
                    new System.Windows.Thickness(10, 0, 0, 0),
                    new System.Windows.Thickness(20, 0, 0, 0),
                    120,
                    new System.Windows.Thickness(20, 0, 0, 0),
                    new System.Windows.Thickness(10, 0, 0, 0),
                    100
                );
                filterStackPanel.Children.Add(stackPanel);

                StackPanel _stack = new StackPanel();
                _stack.Orientation = Orientation.Horizontal;

                _stack.Children.Add(selectDockPanelGenerator(new System.Windows.Thickness(0, 10, 0, 0),
                                                                    50,
                                                                    new System.Windows.Thickness(10, 0, 0, 0),
                                                                    100,
                                                                    new System.Windows.Thickness(15, 0, 0, 0),
                                                                    100,
                                                                    new System.Windows.Thickness(0, 0, 10, 0))
                );
                mainStackPanel.Children.Add(_stack);
                selectComboBox.ItemsSource = generator.selectProcessNames;
            }
        }


        private StackPanel processFilterStackPanelGenerator(System.Windows.Thickness mainStackMargins,
                                                            System.Windows.Thickness processNameLabelMargins,
                                                            System.Windows.Thickness processNameCBMargins,
                                                            int procNameCBWidth,
                                                            System.Windows.Thickness processIdLabelMargins,
                                                            System.Windows.Thickness processIdBMargins,
                                                            int procIdCBWidth)
        {
            stackPanel = new StackPanel();
            stackPanel.Margin = mainStackMargins;

            // First inner stack start
            stackPanel2 = new StackPanel();
            stackPanel2.Orientation = Orientation.Horizontal;

            label = new Label();
            label.Content = "Process name:";
            label.Margin = processNameLabelMargins;
            label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            stackPanel2.Children.Add(label);

            processNameCB = new ComboBox();
            processNameCB.Margin = processNameCBMargins;
            processNameCB.Width = procNameCBWidth;
            processNameCB.ItemsSource = generator.getDistinctProcessNames();
            processNameCB.SelectedIndex = 0;
            processNameCB.SelectionChanged += processNameCB_SelectionChanged;
            processNameCB.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            processNameCB.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            stackPanel2.Children.Add(processNameCB);
            // First inner stack end

            stackPanel.Children.Add(stackPanel2);


            // Second inner stack start
            stackPanel2 = new StackPanel();
            stackPanel2.Orientation = Orientation.Horizontal;

            label = new Label();
            label.Content = "Process ID:";
            label.Margin = processIdLabelMargins;
            label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            stackPanel2.Children.Add(label);

            processIdCB = new ComboBox();
            processIdCB.Margin = processIdBMargins;
            processIdCB.Width = procIdCBWidth;
            processIdCB.ItemsSource = generator.getDistinctProcessIDs((string)processNameCB.SelectedItem);
            processIdCB.SelectedIndex = 0;
            processIdCB.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            processIdCB .VerticalAlignment = System.Windows.VerticalAlignment.Top;
            stackPanel2.Children.Add(processIdCB);
            // Second inner stack start

            stackPanel.Children.Add(stackPanel2);

            return stackPanel;
        }

        private void processNameCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            string procName = (string)combo.SelectedItem;
            processIdCB.ItemsSource = generator.getDistinctProcessIDs(procName);
            processIdCB.SelectedIndex = 0;
        }

        private DockPanel selectDockPanelGenerator(System.Windows.Thickness dockPanelMargins,
                                                   int labelWidth,
                                                   System.Windows.Thickness labelMargins,
                                                   int comboWidth,
                                                   System.Windows.Thickness comboMargins,
                                                   int outsideComboWidth,
                                                   System.Windows.Thickness outsideComboMargins)
        {
            dockPanel = new DockPanel();
            dockPanel.Margin = dockPanelMargins;
            dockPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            dockPanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            label = new Label();
            label.Width = labelWidth;
            label.Margin = labelMargins;
            label.Content = "SELECT";
            label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            stackPanel.Children.Add(label);

            // OnChange of filter we change it's data
            selectComboBox = new ComboBox();
            selectComboBox.Width = comboWidth;
            selectComboBox.Margin = comboMargins;
            parameterNamesComboBox.Add(selectComboBox);
            selectComboBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            selectComboBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            stackPanel.Children.Add(selectComboBox);

            DockPanel.SetDock(stackPanel, Dock.Left);
            dockPanel.Children.Add(stackPanel);

            finalSelectCB = new ComboBox();
            finalSelectCB.Width = outsideComboWidth;
            finalSelectCB.Margin = outsideComboMargins;
            finalSelectCB.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            finalSelectCB.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            // UPGRADE TO BELOW
            //string[] list = { ";", "WHERE", "BETWEEN" };
            string[] list = { ";", "WHERE" };
            finalSelectCB.SelectionChanged += selectFinalCB_SelectionChanged;
            finalSelectCB.SelectedIndex = 0;
            finalSelectCB.ItemsSource = list;

            DockPanel.SetDock(finalSelectCB, Dock.Right);
            dockPanel.Children.Add(finalSelectCB);
            dockPanel.Children.Add(new Label());

            return dockPanel;
        }


        private void selectFinalCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox = (ComboBox)sender;
            // remove all previous existing children
            Console.WriteLine("Count " + mainStackPanel.Children.Count);
            Console.WriteLine("Position " + (mainStackPanel.Children.Count - 1));
            while (mainStackPanel.Children.Count > 1)
            {
                mainStackPanel.Children.RemoveAt(1);
            }
            
            if ((string)comboBox.SelectedItem == "WHERE")
            {
                string filterValue = (string)filterCB.SelectedItem;
                if ( filterValue == "Process") { 
                    StackPanel stak = new StackPanel();
                    stak.Orientation = Orientation.Horizontal;
                    stak.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    stak.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    stak.Children.Add(whereDockPanelGenerator(  new System.Windows.Thickness(0, 10, 0, 0),
                                                                //new System.Windows.Thickness(10, 0, 0, 0),
                                                                //50,
                                                                new System.Windows.Thickness(15, 0, 0, 0),
                                                                100,
                                                                new System.Windows.Thickness(40, 0, 0, 0),
                                                                60,
                                                                new System.Windows.Thickness(20, 0, 0, 0),
                                                                new System.Windows.Thickness(10, 0, 0, 0),
                                                                100,
                                                                new System.Windows.Thickness(0, 0, 10, 0),
                                                                50,
                                                                generator.selectProcessNames));
                    mainStackPanel.Children.Add(stak);
                }
            }
            
        }

        
        private DockPanel whereDockPanelGenerator(  System.Windows.Thickness dockPanelMargins,
                                                    //System.Windows.Thickness whereLabelMargins,
                                                    //int whereLabelWidth,
                                                    System.Windows.Thickness variableComboMargins,
                                                    int variableComboWidth,
                                                    System.Windows.Thickness operatorsComboMargins,
                                                    int operatorsComboWidth,
                                                    System.Windows.Thickness valueLabelMargins,
                                                    System.Windows.Thickness txtBoxMargins,
                                                    int txtBoxWidth,
                                                    System.Windows.Thickness finalComboboxMargins,
                                                    int finalComboBoxWidth,
                                                    string[] cbNames)
        {
            dockPanel = new DockPanel();
            dockPanel.Margin = dockPanelMargins;

            stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            //label = new Label();
            //label.Margin = whereLabelMargins;
            //label.Width = whereLabelWidth;
            //label.Content = "WHERE";
            //stackPanel.Children.Add(label);

            comboBox = new ComboBox();
            comboBox.Margin = variableComboMargins;
            comboBox.Width = variableComboWidth;
            comboBox.ItemsSource = cbNames;
            stackPanel.Children.Add(comboBox);

            comboBox = new ComboBox();
            comboBox.Margin = operatorsComboMargins;
            comboBox.Width = operatorsComboWidth;
            string[] list = { "==", ">", ">=", "<", "<=", "!=" };
            comboBox.ItemsSource = list;
            comboBox.SelectedIndex = 0;
            stackPanel.Children.Add(comboBox);

            label = new Label();
            label.Margin = valueLabelMargins;
            label.Content = "Value:";
            stackPanel.Children.Add(label);

            textBox = new TextBox();
            textBox.Margin = txtBoxMargins;
            textBox.Width = txtBoxWidth;
            stackPanel.Children.Add(textBox);

            dockPanel.Children.Add(stackPanel);

            finalWhereCB = new ComboBox();
            finalWhereCB.Margin = finalComboboxMargins;
            finalWhereCB.Width = finalComboBoxWidth;
            string[] finalList = { ";", "AND" };
            finalWhereCB.ItemsSource = finalList;
            finalWhereCB.SelectedIndex = 0;

            DockPanel.SetDock(finalWhereCB, Dock.Right);
            dockPanel.Children.Add(finalWhereCB);
            dockPanel.Children.Add(new Label());

            return dockPanel;
        }


        private DockPanel andDockPanelGenerator(System.Windows.Thickness dockPanelMargins,
                                                System.Windows.Thickness andLabelMargins,
                                                int andLabelWidth,
                                                System.Windows.Thickness variableComboMargins,
                                                int variableComboWidth,
                                                System.Windows.Thickness operatorsComboMargins,
                                                int operatorsComboWidth,
                                                System.Windows.Thickness valueLabelMargins,
                                                int valueLabelWidth,
                                                System.Windows.Thickness txtBoxMargins,
                                                int txtBoxWidth,
                                                System.Windows.Thickness finalComboboxMargins,
                                                int finalComboBoxWidth)
        {
            dockPanel = new DockPanel();
            dockPanel.Margin = dockPanelMargins;

            stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            label = new Label();
            label.Margin = andLabelMargins;
            label.Width = andLabelWidth;
            label.Content = "AND";
            stackPanel.Children.Add(label);

            comboBox = new ComboBox();
            comboBox.Margin = variableComboMargins;
            comboBox.Width = variableComboWidth;
            parameterNamesComboBox.Add(comboBox);
            stackPanel.Children.Add(comboBox);

            comboBox = new ComboBox();
            comboBox.Margin = operatorsComboMargins;
            comboBox.Width = operatorsComboWidth;
            string[] list = { "=", ">", ">=", "<", "<=", "!=" };
            comboBox.ItemsSource = list;
            stackPanel.Children.Add(comboBox);

            label = new Label();
            label.Margin = valueLabelMargins;
            label.Width = valueLabelWidth;
            label.Content = "Value:";
            stackPanel.Children.Add(label);

            textBox = new TextBox();
            textBox.Margin = txtBoxMargins;
            textBox.Width = txtBoxWidth;
            stackPanel.Children.Add(textBox);

            dockPanel.Children.Add(stackPanel);

            finalAndCB = new ComboBox();
            finalAndCB.Margin = finalComboboxMargins;
            finalAndCB.Width = finalComboBoxWidth;
            string[] finalList = { ";" };
            finalAndCB.ItemsSource = finalList;

            DockPanel.SetDock(finalAndCB, Dock.Right);
            dockPanel.Children.Add(finalAndCB);
            dockPanel.Children.Add(new Label());

            return dockPanel;
        }
        

        /**
         * TO BE IMPLEMENTED
         */

        //private DockPanel betweenDockPanelGenerator(System.Windows.Thickness dockPanelMargins,
        //                                           System.Windows.Thickness whereLabelMargins,
        //                                           int whereLabelWidth,
        //                                           System.Windows.Thickness variableComboMargins,
        //                                           int variableComboWidth,
        //                                           System.Windows.Thickness betweenLabelMargins,
        //                                           int betweenLabelWidth,
        //                                           System.Windows.Thickness startTextMargins,
        //                                           int startTextWidth,
        //                                           System.Windows.Thickness andLabelMargins,
        //                                           int andLabelWidth,
        //                                           System.Windows.Thickness endTextMargins,
        //                                           int endTextWidth,
        //                                           System.Windows.Thickness finalComboboxMargins,
        //                                           int finalComboBoxWidth)
        //{
        //    dockPanel = new DockPanel();
        //    dockPanel.Margin = dockPanelMargins;

        //    stackPanel = new StackPanel();
        //    stackPanel.Orientation = Orientation.Horizontal;

        //    label = new Label();
        //    label.Margin = whereLabelMargins;
        //    label.Width = whereLabelWidth;
        //    label.Content = "WHERE";
        //    stackPanel.Children.Add(label);

        //    comboBox = new ComboBox();
        //    comboBox.Margin = variableComboMargins;
        //    comboBox.Width = variableComboWidth;
        //    parameterNamesComboBox.Add(comboBox);
        //    stackPanel.Children.Add(comboBox);

        //    label = new Label();
        //    label.Margin = betweenLabelMargins;
        //    label.Width = betweenLabelWidth;
        //    label.Content = "BETWEEN";
        //    stackPanel.Children.Add(label);

        //    textBox = new TextBox();
        //    textBox.Margin = startTextMargins;
        //    textBox.Width = startTextWidth;
        //    stackPanel.Children.Add(textBox);

        //    label = new Label();
        //    label.Margin = andLabelMargins;
        //    label.Width = andLabelWidth;
        //    label.Content = "AND";
        //    stackPanel.Children.Add(label);

        //    textBox = new TextBox();
        //    textBox.Margin = endTextMargins;
        //    textBox.Width = endTextWidth;
        //    stackPanel.Children.Add(textBox);
            
        //    dockPanel.Children.Add(stackPanel);

        //    comboBox = new ComboBox();
        //    comboBox.Margin = finalComboboxMargins;
        //    comboBox.Width = finalComboBoxWidth;

        //    DockPanel.SetDock(comboBox, Dock.Right);
        //    dockPanel.Children.Add(comboBox);
        //    dockPanel.Children.Add(new Label());

        //    return dockPanel;
        //}
         
    }
}
