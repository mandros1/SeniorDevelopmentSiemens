﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Controls;
using Newtonsoft.Json;
using LiveCharts.Wpf;
using LiveCharts;
using System.Linq;
using System.Windows;

namespace SiemensPerformance
{
    class DataDisplayTab : TabItem
    {
        private DataGenerator generator = new DataGenerator();
        private DataGrid processGrid;
        private DataTable processTable;

        // Global(0) data grid and data table
        private DataGrid globalZeroGrid;
        private DataTable globalZeroTable;

        // Global(_Total) grid and data table
        private DataGrid globalTotalGrid;
        private DataTable globalTotalTable;

        private string[] filterByArray = { "Process", "Global(0)", "Global(_Total)" };

        public Boolean displayable { get; set; }

        private CartesianChart ch;

        // Main stack to which all of the below are appended to
        private StackPanel mainStackPanel;

        // Filter stack elements
        private StackPanel filterStackPanel;
        private ComboBox processNameCB;
        private ComboBox processIdCB;
        private ComboBox filterCB;

        // Select stack elements
        private ComboBox selectComboBox;
        private ComboBox finalSelectCB;

        // Where stack elements
        private ComboBox whereSelectName;
        private TextBox whereValue;
        private ComboBox whereOperatorsComboBox;
        private ComboBox finalWhereCB;

        // And stack elements    
        private ComboBox andSelectNameCB;
        private TextBox andValue;
        private ComboBox andOperatorsCB;
        private ComboBox finalAndCB;

        private Button runButton, saveButton; 
        private TabItem graphTabItem;

        private ComboBox finalBetweenCB;

        // reusable components
        private StackPanel stackPanel;
        private StackPanel stackPanel2;
        private Label label;
        private ComboBox comboBox;
        private DockPanel dockPanel;
        private TextBox textBox;
        private TabItem tabItem;
        private DataGrid dataGrid;
        private DataTable dataTable;
        private List<string[]> processData;
        private string[] columnNames;
        private Wpf.CartesianChart.ZoomingAndPanning.ZoomingAndPanning cartesianChart;
        private ChartValues<DateModel> data;
        private List<DateModel> dateModelData;
        private List<string> queryList;
        public static string utfFileName;
        public static int dbConnection;

        public DataDisplayTab(int dbInt)
        {
            dbConnection = dbInt;
            //Open File
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Log/Text Files (*.utr; *.json)|*.utr; *.json";

            //Get Data
            if (ofd.ShowDialog() == true)
            {
                if (ofd.FileName.EndsWith(".utr"))
                {
                    generator.getJsonString(ofd);
                    displayable = true;
                }

                /*Not Working */
                //else if (ofd.FileName.EndsWith(".json"))
                //{
                //    Console.WriteLine("Working");
                //    displayable = true;
                //}
                else
                {
                    return;
                }
            }
            else
            {
                displayable = false;
                return;
            }

            TabControl tc = new TabControl();

            //Tab dropdown menu
            //Rename Tab
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem1 = new MenuItem();
            contextMenu.Items.Add(menuItem1);
            menuItem1.Header = "Rename";
            menuItem1.Click += delegate { Rename(); };

            //Save data to Json
            MenuItem menuItem2 = new MenuItem();
            contextMenu.Items.Add(menuItem2);
            menuItem2.Header = "Save";
            menuItem2.Click += delegate { Save(); };
            

            //Close Tab
            MenuItem menuItem3 = new MenuItem();
            contextMenu.Items.Add(menuItem3);
            menuItem3.Header = "Close";
            menuItem3.Click += delegate { Close(); };

            this.ContextMenu = contextMenu;

            // Create process table tab
            tabItem = new TabItem();
            this.Header = generator.fileName;
            tabItem.Header = "Processes";
            // Pumping content into the table
            processGrid = dataGridData(generator.processes2DList, generator.processVariables, processTable);
            tabItem.Content = processGrid;

            tabItem.ContextMenu = new ContextMenu();
            tc.Items.Insert(0, tabItem);

            // Create global zero table tab
            tabItem = new TabItem();
            this.Header = generator.fileName;
            tabItem.Header = "Global(0)";
            // Pumping content into the table
            globalZeroGrid = dataGridData(generator.globalZero2DList, generator.globalZeroVariables, globalZeroTable);

            tabItem.Content = globalZeroGrid;

            tabItem.ContextMenu = new ContextMenu();
            tc.Items.Insert(1, tabItem);


            // Create global total table tab
            tabItem = new TabItem();
            this.Header = generator.fileName;
            tabItem.Header = "Global(_Total)";
            // Pumping content into the table
            globalTotalGrid = dataGridData(generator.globalTotal2DList, generator.globalTotalVariables, globalTotalTable);
            tabItem.Content = globalTotalGrid;

            tabItem.ContextMenu = new ContextMenu();
            tc.Items.Insert(2, tabItem);


            // Create Graph tab
            graphTabItem = new TabItem();
            graphTabItem.Header = "Graph";

            graphTabItem.ContextMenu = new ContextMenu();
            tc.Items.Insert(3, graphTabItem);

            // Create Query tab
            tabItem = generateQueryTabItem();
            tabItem.ContextMenu = new ContextMenu();
            tc.Items.Insert(4, tabItem);

            this.Content = tc;
        }

        public DataDisplayTab()
        {
            //Open File
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".utr";
            ofd.Filter = "Text files (*.utr)|*.utr";
            //Get Data
            if (ofd.ShowDialog() == true)
            {
                utfFileName = System.IO.Path.GetFileName(ofd.FileName);
                generator.getJsonString(ofd);
                displayable = true;
            }
            else
            {
                displayable = false;
                return;
            }

            TabControl tc = new TabControl();

            //Tab dropdown menu
            //Rename Tab
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem1 = new MenuItem();
            contextMenu.Items.Add(menuItem1);
            menuItem1.Header = "Rename";
            menuItem1.Click += delegate { Rename(); };
            
            //Close Tab
            MenuItem menuItem2 = new MenuItem();
            contextMenu.Items.Add(menuItem2);
            menuItem2.Header = "Close";
            menuItem2.Click += delegate { Close(); };

            //Save data to Json
            MenuItem menuItem3 = new MenuItem();
            contextMenu.Items.Add(menuItem3);
            menuItem3.Header = "Save";
            menuItem3.Click += delegate { Save(); };

            this.ContextMenu = contextMenu;
            
            // Create process table tab
            tabItem = new TabItem();
            this.Header = generator.fileName;
            tabItem.Header = "Processes";
            // Pumping content into the table
            processGrid = dataGridData(generator.processes2DList, generator.processVariables, processTable);
            tabItem.Content = processGrid;

            tabItem.ContextMenu = new ContextMenu();
            tc.Items.Insert(0, tabItem);

            // Create global zero table tab
            tabItem = new TabItem();
            this.Header = generator.fileName;
            tabItem.Header = "Global(0)";
            // Pumping content into the table
            globalZeroGrid = dataGridData(generator.globalZero2DList, generator.globalZeroVariables, globalZeroTable);
            tabItem.Content = globalZeroGrid;

            tabItem.ContextMenu = new ContextMenu();
            tc.Items.Insert(1, tabItem);
            

            // Create global total table tab
            tabItem = new TabItem();
            this.Header = generator.fileName;
            tabItem.Header = "Global(_Total)";
            // Pumping content into the table
            globalTotalGrid = dataGridData(generator.globalTotal2DList, generator.globalTotalVariables, globalTotalTable);
            tabItem.Content = globalTotalGrid;

            tabItem.ContextMenu = new ContextMenu();
            tc.Items.Insert(2, tabItem);

            
            // Create Graph tab
            graphTabItem = new TabItem();
            graphTabItem.Header = "Graph";

            graphTabItem.ContextMenu = new ContextMenu();
            tc.Items.Insert(3, graphTabItem);

            // Create Query tab
            tabItem = generateQueryTabItem();
            tabItem.ContextMenu = new ContextMenu();
            tc.Items.Insert(4, tabItem);
            
            this.Content = tc;
        } 

        private void procName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            string procName = (string) combo.SelectedItem;
            processIdCB.ItemsSource = generator.getDistinctProcessIDs(procName);
        } 


        /**
         * Main method for generating query tab item
         */
        private TabItem generateQueryTabItem()
        {
            TabItem tab = new TabItem();
            if (dbConnection == 1) { tab.Header = "Query file"; }
            else
            {
                tab.Header = "Query database";
            }

            stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            
            dockPanel = baseDockPanel();

            DockPanel.SetDock(stackPanel, Dock.Top);
            dockPanel.Children.Add(stackPanel);
            

            tab.Content = dockPanel;
            return tab;
        }

        private void RunButtonClick(object sender, EventArgs e)
        {
            if (String.Equals((string)filterCB.SelectedItem, "Process"))
            {
                processData = generator.getProcessData((string)processNameCB.SelectedItem, (string)processIdCB.SelectedItem);
                columnNames = generator.processVariables;
            }
            if (String.Equals((string)filterCB.SelectedItem, "Global(0)"))
            {
                processData = generator.globalZero2DList;
                columnNames = generator.globalZeroVariables;
            }
            if (String.Equals((string)filterCB.SelectedItem, "Global(_Total)"))
            {
                processData = generator.globalTotal2DList;
                columnNames = generator.globalTotalVariables;
            }


            if (String.Equals((string)finalSelectCB.SelectedItem, ";"))
            {
                dateModelData = generator.getDateModelList(processData, (string)selectComboBox.SelectedItem, columnNames);
                graphTabItem.Content = PopulateGraph(dateModelData);
                processTable = ConvertListToDataTable(processData, generator.processVariables);
                processGrid.ItemsSource = processTable.DefaultView;
            }
            else if (String.Equals((string)finalSelectCB.SelectedItem, "WHERE"))
            {

                string whereColumn = (string)whereSelectName.SelectedItem;
                string whereOperator = (string)whereOperatorsComboBox.SelectedItem;
                string whereVal = whereValue.Text;
                processData = generator.getWhereProcessData(processData, whereColumn, whereOperator, whereVal, columnNames);

                if (String.Equals((string)finalWhereCB.SelectedItem, "AND"))
                {
                    whereColumn = (string)andSelectNameCB.SelectedItem;
                    whereOperator = (string)andOperatorsCB.SelectedItem;
                    whereVal = andValue.Text;
                    processData = generator.getWhereProcessData(processData, whereColumn, whereOperator, whereVal, columnNames);
                }

                dateModelData = generator.getDateModelList(processData, whereColumn, columnNames);
                graphTabItem.Content = PopulateGraph(dateModelData);

                if (String.Equals((string)filterCB.SelectedItem, "Process"))
                {
                    processTable = ConvertListToDataTable(processData, generator.processVariables);
                    processGrid.ItemsSource = processTable.DefaultView;
                }
                else if (String.Equals((string)filterCB.SelectedItem, "Global(0)"))
                {
                    globalZeroTable = ConvertListToDataTable(processData, generator.globalZeroVariables);
                    globalZeroGrid.ItemsSource = globalZeroTable.DefaultView;
                }
                else if (String.Equals((string)filterCB.SelectedItem, "Global(_Total)"))
                {
                    globalTotalTable = ConvertListToDataTable(processData, generator.globalTotalVariables);
                    globalTotalGrid.ItemsSource = globalTotalTable.DefaultView;
                }
            }
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            if (dbConnection == 1)
            {
                List<string[]> youGoodMate = generator.getProcessData((string)processNameCB.SelectedItem, (string)processIdCB.SelectedItem);
                processData = youGoodMate;
            }
            else
            {
                //Fix
                //Method not found
                //List<string[]> test = generator.getDataFromDb((string)processNameCB.SelectedItem, (string)processIdCB.SelectedItem);
                //processData = test;
            }

            if (String.Equals((string)finalSelectCB.SelectedItem, ";"))
            {
                dateModelData = generator.getDateModelList(processData, (string)selectComboBox.SelectedItem, columnNames);
                graphTabItem.Content = PopulateGraph(dateModelData);
                processTable = ConvertListToDataTable(processData, generator.processVariables);
                processGrid.ItemsSource = processTable.DefaultView;
            }
            else if (   String.Equals((string)finalSelectCB.SelectedItem, "WHERE") 
                        && 
                        String.Equals((string)filterCB.SelectedItem, "Process"))
            {
                Console.WriteLine("Where Select");
                string whereColumn = (string)whereSelectName.SelectedItem;
                string whereOperator = (string)whereOperatorsComboBox.SelectedItem;
                string whereVal = whereValue.Text;
                processData = generator.getWhereProcessData(processData, whereColumn, whereOperator, whereVal, columnNames);
            }

            processTable = ConvertListToDataTable(processData, generator.processVariables);
            processGrid.ItemsSource = processTable.DefaultView;

            queryList = new List<string>();
            queryList.Add("processName:" + (string)processNameCB.SelectedItem);
            queryList.Add("processId:" + (string)processIdCB.SelectedItem);
            queryList.Add("selectValue:" + (string)selectComboBox.SelectedItem);
            queryList.Add("where:" + (string)finalSelectCB.SelectedItem);

            if ((string)finalSelectCB.SelectedItem == "WHERE")
            {
                queryList.Add("whereColumn: " + (string)whereSelectName.SelectedItem);
                queryList.Add("whereOperator: " + (string)whereOperatorsComboBox.SelectedItem);
                queryList.Add("whereOperatorValue: " + whereValue.Text);
            }
        }

        //Save Query into Database
        private void SaveButton_Click(object sender, EventArgs e)
        {
            queryList = new List<string>();
            string name = new InputBox("Name").ShowDialog();
            if (name == "" || name ==null)
            {
                name = new InputBox("Name").ShowDialog();
            }

            queryList.Add(name);
            queryList.Add("processName:" + (string)processNameCB.SelectedItem);
            queryList.Add("processId:" + (string)processIdCB.SelectedItem);
            queryList.Add("selectValue:" + (string)selectComboBox.SelectedItem);
            queryList.Add("where:" + (string)finalSelectCB.SelectedItem);

            if ((string)finalSelectCB.SelectedItem == "WHERE")
            {
                queryList.Add("whereColumn: " + (string)whereSelectName.SelectedItem);
                queryList.Add("whereOperator: " + (string)whereOperatorsComboBox.SelectedItem);
                queryList.Add("whereOperatorValue: " + whereValue.Text);
            }
            DataInsert dataInsert = new DataInsert();
            dataInsert.insertQuery(queryList);
        }

        private Button runButtonGenerator()
        {
            runButton = new Button();
            runButton.Content = "Query Data";
            runButton.Width = 100;
            runButton.Height = 50;
            runButton.Margin = new System.Windows.Thickness(320, 10, 0, 0);
            runButton.Click += new System.Windows.RoutedEventHandler(NewButton_Click);

            return runButton;
        }
        private Button saveButtonGenerator()
        {
            saveButton = new Button();
            saveButton.Content = "Save Query";
            saveButton.Width = 100;
            saveButton.Height = 50;
            saveButton.Margin = new System.Windows.Thickness(320, 10, 0, 0);
            saveButton.Click += new System.Windows.RoutedEventHandler(SaveButton_Click);

            if (dbConnection == 1)
            {

            }
            else
            {
                return saveButton;
            }
            return saveButton;
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
            filterCB.Width = 150;
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
                selectComboBox.SelectedIndex = 0;

                stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Children.Add(runButtonGenerator());
                if (dbConnection == 1) {}
                else
                {
                    stackPanel.Children.Add(saveButtonGenerator());
                }
                mainStackPanel.Children.Add(stackPanel);
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
            stackPanel.Orientation = Orientation.Horizontal;
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
            finalSelectCB.SelectedIndex = 0;
            finalSelectCB.ItemsSource = list; 
            finalSelectCB.SelectionChanged += selectFinalCB_SelectionChanged;

            DockPanel.SetDock(finalSelectCB, Dock.Right);
            dockPanel.Children.Add(finalSelectCB);
            dockPanel.Children.Add(new Label());

            return dockPanel;
        }


        private void selectFinalCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StackPanel stak;
            // remove all previous existing children
            while (mainStackPanel.Children.Count > 1)
            {
                mainStackPanel.Children.RemoveAt(1);
            }

            if ((string)finalSelectCB.SelectedItem == ";")
            {
                stak = new StackPanel();
                stak.Orientation = Orientation.Horizontal;
                stak.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                stak.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                stak.Children.Add(runButtonGenerator());
                stak.Children.Add(saveButtonGenerator());
                mainStackPanel.Children.Add(stak);
            }

            if ((string)finalSelectCB.SelectedItem == "WHERE")
            {
                string filterValue = (string)filterCB.SelectedItem;
                if ( filterValue == "Process") { 
                    stak = new StackPanel();
                    stak.Orientation = Orientation.Horizontal;
                    stak.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    stak.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    stak.Children.Add(whereDockPanelGenerator(  new System.Windows.Thickness(0, 10, 0, 0),
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

                    stak = new StackPanel();
                    stak.Orientation = Orientation.Horizontal;
                    stak.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    stak.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    stak.Children.Add(runButtonGenerator());
                    stak.Children.Add(saveButtonGenerator());
                    mainStackPanel.Children.Add(stak);
                }
            }
        }

        
        private DockPanel whereDockPanelGenerator(  System.Windows.Thickness dockPanelMargins,
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

            whereSelectName = new ComboBox();
            whereSelectName.Margin = variableComboMargins;
            whereSelectName.Width = variableComboWidth;
            whereSelectName.ItemsSource = cbNames;
            stackPanel.Children.Add(whereSelectName);

            whereOperatorsComboBox = new ComboBox();
            whereOperatorsComboBox.Margin = operatorsComboMargins;
            whereOperatorsComboBox.Width = operatorsComboWidth;
            string[] list = { "==", ">", ">=", "<", "<=", "!=" };
            whereOperatorsComboBox.ItemsSource = list;
            whereOperatorsComboBox.SelectedIndex = 0;
            stackPanel.Children.Add(whereOperatorsComboBox);

            label = new Label();
            label.Margin = valueLabelMargins;
            label.Content = "Value:";
            stackPanel.Children.Add(label);

            whereValue = new TextBox();
            whereValue.Margin = txtBoxMargins;
            whereValue.Width = txtBoxWidth;
            stackPanel.Children.Add(whereValue);

            dockPanel.Children.Add(stackPanel);

            finalWhereCB = new ComboBox();
            finalWhereCB.Margin = finalComboboxMargins;
            finalWhereCB.Width = finalComboBoxWidth;
            string[] finalList = { ";" };
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



        /*
         * Generate DataGrid from data
         */
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

        /*
         * Converts list to data table
         */
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


        private DataGrid dataGridData(List<string[]> data2dList, string[] columns, DataTable whichTable)
        {
            whichTable = ConvertListToDataTable(data2dList, columns);
            dataGrid = new DataGrid();
            dataGrid.ItemsSource = whichTable.DefaultView;
            dataGrid.IsReadOnly = true;
            return dataGrid;
        }

        /*
         * Opens a popup window to ask for a new name and renames the tab
         */
        private void Rename()
        {
            string name = new InputBox("Name").ShowDialog();
            if (name != "")
            {
                this.Header = name;
            }
        }

        /*
         * Saves data from a graph/query to json file
         */
        private void Save()
        {
            //set default file name to tab header
            String defaultName = this.Header.ToString();
            //Remove .utr extention if present
            if (defaultName.EndsWith(".utr"))
            {
                defaultName = defaultName.Substring(0, defaultName.Length - 4);
            }

            try
            {
                if (processTable == null && processTable.Rows.Count < 0)
                {
                    Console.WriteLine("Error. Table is null");
                }

                //Create save dialog
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = defaultName;
                dlg.DefaultExt = ".json";
                dlg.Filter = "JSON (.json)|*.json";

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                var query = JsonConvert.SerializeObject(queryList);
                string json = JsonConvert.SerializeObject(processTable, Formatting.Indented);
                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    string filename = dlg.FileName;
                    File.WriteAllText(filename, query);
                    File.AppendAllText(filename, json);
                }
            }

            catch (NullReferenceException nre)
            {
                MessageBox.Show("Cannot create a result file. Create a query first!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /*
         * Close a tab
         */
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

        /*
         * Converts data from a list of String arrays [timstamp, data] to a Chartvalues<DateModel> and puts it in the graph
         */
        private Wpf.CartesianChart.ZoomingAndPanning.ZoomingAndPanning PopulateGraph(List<DateModel> dateModel2dList)
        {
            data = new ChartValues<DateModel>();
            data.AddRange(dateModel2dList);
            cartesianChart = new Wpf.CartesianChart.ZoomingAndPanning.ZoomingAndPanning(data);
            return cartesianChart;
        }
    }
}