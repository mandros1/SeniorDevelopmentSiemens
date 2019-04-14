using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Controls;
using Newtonsoft.Json;
using LiveCharts.Wpf;
using LiveCharts;
using System.Linq;

namespace SiemensPerformance
{
    class DataDisplayTab : TabItem
    {
        private DataGenerator generator = new DataGenerator();
        
        // Processes data grid and data table
        private DataGrid processGrid;

        // Global(0) data grid and data table
        private DataGrid globalZeroGrid;

        // Global(_Total) grid and data table
        private DataGrid globalTotalGrid;

        private string[] filterByArray = { "Process", "Global(0)", "Global(_Total)" };

        public Boolean displayable {get; set;}

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


        private Button runButton;
        private TabItem graphTabItem;

        // reusable components
        private StackPanel stackPanel, stackPanel2;
        private Label label;
        private ComboBox comboBox;
        private DockPanel dockPanel;
        private TabItem tabItem;
        private DataGrid dataGrid;
        private List<string[]> processData;
        private string[] columnNames;
        private Wpf.CartesianChart.ZoomingAndPanning.ZoomingAndPanning cartesianChart;
        private ChartValues<DateModel> data;
        private List<DateModel> dateModelData;
        public static string utfFileName;
        private int dbConnection { get; set; }
        private string stringData;
        private int selection;
        private DataTable reusableDataTable { get; set; }
        private Button resetButton;
        private MenuItem reusableMenuItem;
        private Boolean import = false;

        public DataDisplayTab(int dbInt)
        {
            Console.WriteLine(dbInt);
            dbConnection = dbInt;
            //Open File
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".utr";
            ofd.Filter = "Text files (*.utr)|*.utr|Json files (*.json)|*.json";

            //Get Data
            if (ofd.ShowDialog() == true)
            {
                string ext = Path.GetExtension(ofd.FileName);
                utfFileName = System.IO.Path.GetFileName(ofd.FileName);
                if (ext == ".utr")
                {
                    generator.getJsonString(ofd);
                    if (dbConnection != 1)
                    {
                        //TODO: insert into DB
                    }
                }
                else if (ext == ".json")
                {
                    generator.importResultFile(ofd);
                    if (dbConnection != 1)
                    {
                        //TODO: insert into DB
                    }
                    import = true;
                }
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

            reusableMenuItem = new MenuItem();
            contextMenu.Items.Add(reusableMenuItem);
            reusableMenuItem.Header = "Rename";
            reusableMenuItem.Click += delegate { Rename(); };

            //Save data to Json
            reusableMenuItem = new MenuItem();
            contextMenu.Items.Add(reusableMenuItem);
            reusableMenuItem.Header = "Export";
            reusableMenuItem.Click += delegate { Export(); };

            //Close Tab
            reusableMenuItem = new MenuItem();
            contextMenu.Items.Add(reusableMenuItem);
            reusableMenuItem.Header = "Close";
            reusableMenuItem.Click += delegate { Close(); };
            
            this.ContextMenu = contextMenu;
            
            // Create process table tab
            tabItem = new TabItem();
            this.Header = generator.fileName;
            tabItem.Header = "Processes";

            processGrid = dataGridData(generator.processes2DList, generator.processVariables, reusableDataTable);
            processGrid.Height = 450;
           
            resetButton = resetButtonGenerator("Reset Process Table");

            dockPanel = new DockPanel();
            DockPanel.SetDock(processGrid, Dock.Top);
            DockPanel.SetDock(resetButton, Dock.Bottom);
            dockPanel.Children.Add(processGrid);
            dockPanel.Children.Add(resetButton);
            
            tabItem.Content = dockPanel;
            tabItem.ContextMenu = new ContextMenu();
            tc.Items.Insert(0, tabItem);

            // Create global zero table tab
            tabItem = new TabItem();
            this.Header = generator.fileName;
            tabItem.Header = "Global(0)";


            globalZeroGrid = dataGridData(generator.globalZero2DList, generator.globalZeroVariables, reusableDataTable);
            globalZeroGrid.Height = 450;
            resetButton = resetButtonGenerator("Reset Global_0 Table");

            dockPanel = new DockPanel();
            DockPanel.SetDock(globalZeroGrid, Dock.Top);
            DockPanel.SetDock(resetButton, Dock.Bottom);
            dockPanel.Children.Add(globalZeroGrid);
            dockPanel.Children.Add(resetButton);

            tabItem.Content = dockPanel;

            tabItem.ContextMenu = new ContextMenu();
            tc.Items.Insert(1, tabItem);
            

            // Create global total table tab
            tabItem = new TabItem();
            this.Header = generator.fileName;
            tabItem.Header = "Global(_Total)";

            globalTotalGrid = dataGridData(generator.globalTotal2DList, generator.globalTotalVariables, reusableDataTable);
            globalTotalGrid.Height = 450;
            resetButton = resetButtonGenerator("Reset Global_Total Table");

            dockPanel = new DockPanel();
            DockPanel.SetDock(globalTotalGrid, Dock.Top);
            DockPanel.SetDock(resetButton, Dock.Bottom);
            dockPanel.Children.Add(globalTotalGrid);
            dockPanel.Children.Add(resetButton);

            tabItem.Content = dockPanel;

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

            if (import)
            {
                dateModelData = generator.getDateModelList(generator.graphData, generator.graphColumn, generator.graphColumnNames);
                graphTabItem.Content = this.PopulateGraph(dateModelData);
            }

            this.Content = tc;
        } 

        /**
         * When the name of the process is changed generate the IDs in the IDComboBox that belong to that process name
         */
        private void procName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox = (ComboBox)sender;
            stringData = (string)comboBox.SelectedItem;
            processIdCB.ItemsSource = generator.getDistinctProcessIDs(stringData);
        } 


       
        /**
         * Main method for generating query tab item
         */
        private TabItem generateQueryTabItem()
        {
            TabItem tab = new TabItem();
            tab.Header = dbConnection == 1 ? "Query file" : "Query database";
           
            stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            
            dockPanel = baseDockPanel();

            DockPanel.SetDock(stackPanel, Dock.Top);
            dockPanel.Children.Add(stackPanel);


            tab.Content = dockPanel;
            return tab;
        }

        /**
        * Handles on button click event and runs the queries depending on multiple selecitons
        */
        private void RunButtonClick(object sender, EventArgs e)
        {
            string test1 = "";
			string whereColumn = "";
            string whereOperator = "";
            string whereVal = "";

            if (String.Equals((string)filterCB.SelectedItem, "Process"))
            {
                if (dbConnection == 1)
                {
                    processData = generator.getProcessData((string)processNameCB.SelectedItem, (string)processIdCB.SelectedItem);
                }
                else {
                    processData = generator.getProcessDataFromDB((string)processNameCB.SelectedItem, (string)processIdCB.SelectedItem);
                    test1 = "*, DATE_FORMAT(TimeStamp, '%Y/%m/%d-%H:%i:%s.%f') AS date FROM mri_data";
                }
                columnNames = generator.processVariables;
                selection = 1;
            }
            if (String.Equals((string)filterCB.SelectedItem, "Global(0)"))
            {
              
                    processData = generator.globalZero2DList;
                  
                    test1 = "*, DATE_FORMAT(TimeStamp, '%Y/%m/%d-%H:%i:%s.%f') AS date FROM global0";
                
                columnNames = generator.globalZeroVariables;
                selection = 2;
            }
            if (String.Equals((string)filterCB.SelectedItem, "Global(_Total)"))
            {
               
                    processData = generator.globalTotal2DList;
            
                    test1 = "*, DATE_FORMAT(TimeStamp, '%Y/%m/%d-%H:%i:%s.%f') AS date FROM globaltotal";
                
                columnNames = generator.globalTotalVariables;
				selection = 3;
            }
            
            if (String.Equals((string)finalSelectCB.SelectedItem, "WHERE"))
            {
                
                whereColumn = (string)whereSelectName.SelectedItem;
                whereOperator = (string)whereOperatorsComboBox.SelectedItem;    
                whereVal = whereValue.Text;

                if (dbConnection == 1)
                {
                    processData = generator.getWhereProcessData(processData, whereColumn, whereOperator, whereVal, columnNames);
                    if (String.Equals((string)finalWhereCB.SelectedItem, "AND"))
                    {
                        whereColumn = (string)andSelectNameCB.SelectedItem;
                        whereOperator = (string)andOperatorsCB.SelectedItem;
                        whereVal = andValue.Text;
                        processData = generator.getWhereProcessData(processData, whereColumn, whereOperator, whereVal, columnNames); 
                    }
                }
                else {
                    if (whereOperator.Equals("==")) test1 += " WHERE " + " " + whereColumn + " = " + whereVal + " ";
                    else test1 += " WHERE " + " " + whereColumn + " " + whereOperator + " " + whereVal + " ";

                    if (String.Equals((string)finalWhereCB.SelectedItem, "AND"))
                    {
                        whereColumn = (string)andSelectNameCB.SelectedItem;
                        whereOperator = (string)andOperatorsCB.SelectedItem;
                        whereVal = andValue.Text;
                        
                        // get processData using the database

                        if (whereOperator.Equals("==")) test1 += " AND " + " " + whereColumn + " = " + whereVal + " ";
                        else test1 += " AND " + " " + whereColumn + " " + whereOperator + " " + whereVal + " ";
                    }
                    processData = generator.getDataFromQueryDb(test1);
                }
            }
            
            dateModelData = generator.getDateModelList(processData, (string)selectComboBox.SelectedItem, columnNames);
            graphTabItem.Content = PopulateGraph(dateModelData);
            reusableDataTable = ConvertListToDataTable(reusableDataTable, processData, columnNames);

            if (selection == 1) processGrid.ItemsSource = reusableDataTable.DefaultView;
            if (selection == 2) globalZeroGrid.ItemsSource = reusableDataTable.DefaultView;
            if (selection == 3) globalTotalGrid.ItemsSource = reusableDataTable.DefaultView;
        }

        private Button runButtonGenerator()
        {
            runButton = new Button();
            runButton.Content = "Query Data";
            runButton.Width = 100;
            runButton.Height = 50;
            runButton.Margin = new System.Windows.Thickness(320, 10, 0, 0);
            runButton.Click += new System.Windows.RoutedEventHandler(RunButtonClick);

            return runButton;
        }

        private void ResetButtonClick(object sender, EventArgs e)
        {
            resetButton = (Button)sender;
            stringData = (string)resetButton.Content;
            if (stringData == "Reset Process Table")
            {
                // TODO:
                if(dbConnection == 1)
                {
                    reusableDataTable = ConvertListToDataTable(reusableDataTable, generator.processes2DList, generator.processVariables);
                    processGrid.ItemsSource = reusableDataTable.DefaultView;
                }
                else
                {
                    processData = generator.getProcessDataFromDB(null, null);
                    reusableDataTable = ConvertListToDataTable(reusableDataTable, processData, generator.processVariables);
                    processGrid.ItemsSource = reusableDataTable.DefaultView;
                }
                
            } else if (stringData == "Reset Global_0 Table")
            {
                if (dbConnection == 1)
                {
                    reusableDataTable = ConvertListToDataTable(reusableDataTable, generator.globalZero2DList, generator.globalZeroVariables);
                }
                else
                {
                    // TODO: get the globalZero2D list over the DB
                }
                globalZeroGrid.ItemsSource = reusableDataTable.DefaultView;
            } else if (stringData == "Reset Global_Total Table")
            {
                if (dbConnection == 1)
                {
                    reusableDataTable = ConvertListToDataTable(reusableDataTable, generator.globalTotal2DList, generator.globalTotalVariables);
                }
                else
                {
                    // TODO: get the globalTotal2D list over the DB
                }
                globalTotalGrid.ItemsSource = reusableDataTable.DefaultView;
            }
        }

        private Button resetButtonGenerator(string text)
        {
            resetButton = new Button();
            resetButton.Content = text;
            resetButton.Width = 200;
            resetButton.Height = 50;
            resetButton.Margin = new System.Windows.Thickness(100, 0, 0, 0);
            resetButton.Click += new System.Windows.RoutedEventHandler(ResetButtonClick);

            return resetButton;
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

            StackPanel _stack = new StackPanel();
            _stack.Orientation = Orientation.Horizontal;

            _stack = new StackPanel();
            _stack.Children.Add(selectDockPanelGenerator(new System.Windows.Thickness(0, 10, 0, 0),
                                                         50,
                                                         new System.Windows.Thickness(10, 0, 0, 0),
                                                         100,
                                                         new System.Windows.Thickness(15, 0, 0, 0),
                                                         100,
                                                         new System.Windows.Thickness(0, 0, 10, 0)));
            mainStackPanel.Children.Add(_stack);

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
                
                selectComboBox.ItemsSource = generator.selectProcessNames;

            } else if (filterValue == "Global(0)")
            {
                selectComboBox.ItemsSource = generator.selectGlobalZeroNames;
            }
            else if (filterValue == "Global(_Total)")
            {
                selectComboBox.ItemsSource = generator.selectGlobalTotalNames;
            }

            selectComboBox.SelectedIndex = 0;
            stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Children.Add(runButtonGenerator());

            mainStackPanel.Children.Add(stackPanel);
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

            if ((string)finalSelectCB.SelectedItem == "WHERE")
            {
                string filterValue = (string)filterCB.SelectedItem;
                stak = new StackPanel();
                stak.Orientation = Orientation.Horizontal;
                stak.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                stak.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                if ( filterValue == "Process") {
                    columnNames = generator.selectProcessNames;
                }
                else if (filterValue == "Global(0)")
                {
                    columnNames = generator.selectGlobalZeroNames;
                }
                else if (filterValue == "Global(_Total)")
                {
                    columnNames = generator.selectGlobalTotalNames;
                }

                stak.Children.Add(whereDockPanelGenerator(new System.Windows.Thickness(0, 10, 0, 0),
                                                            new System.Windows.Thickness(15, 0, 0, 0),
                                                            100,
                                                            new System.Windows.Thickness(40, 0, 0, 0),
                                                            60,
                                                            new System.Windows.Thickness(20, 0, 0, 0),
                                                            new System.Windows.Thickness(10, 0, 0, 0),
                                                            100,
                                                            new System.Windows.Thickness(0, 0, 10, 0),
                                                            80,
                                                            columnNames));
                mainStackPanel.Children.Add(stak);
            }
            stak = new StackPanel();
            stak.Orientation = Orientation.Horizontal;
            stak.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stak.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            stak.Children.Add(runButtonGenerator());
            mainStackPanel.Children.Add(stak);
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
            whereSelectName.SelectedIndex = 0;
            stackPanel.Children.Add(whereSelectName);

            whereOperatorsComboBox = new ComboBox();
            whereOperatorsComboBox.Margin = operatorsComboMargins;
            whereOperatorsComboBox.Width = operatorsComboWidth;
            string[] list = { "==", ">", ">=", "<", "<=", "!=" };
            whereOperatorsComboBox.ItemsSource = list;
            whereOperatorsComboBox.SelectedIndex = 1;
            stackPanel.Children.Add(whereOperatorsComboBox);

            label = new Label();
            label.Margin = valueLabelMargins;
            label.Content = "Value:";
            stackPanel.Children.Add(label);

            whereValue = new TextBox();
            whereValue.Margin = txtBoxMargins;
            whereValue.Width = txtBoxWidth;
            whereValue.Text = "0";
            stackPanel.Children.Add(whereValue);

            dockPanel.Children.Add(stackPanel);

            finalWhereCB = new ComboBox();
            finalWhereCB.Margin = finalComboboxMargins;
            finalWhereCB.Width = finalComboBoxWidth;
            string[] finalList = { ";", "AND" };
            finalWhereCB.ItemsSource = finalList;
            finalWhereCB.SelectedIndex = 0;
            finalWhereCB.SelectionChanged += whereFinalCB_SelectionChanged;

            DockPanel.SetDock(finalWhereCB, Dock.Right);
            dockPanel.Children.Add(finalWhereCB);
            dockPanel.Children.Add(new Label());

            return dockPanel;
        }

        private void whereFinalCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StackPanel stak;
            // remove all previous existing children
            while (mainStackPanel.Children.Count > 2)
            {
                mainStackPanel.Children.RemoveAt(mainStackPanel.Children.Count - 1);
            }
            if ((string)finalWhereCB.SelectedItem == "AND")
            {
                string filterValue = (string)filterCB.SelectedItem;
                stak = new StackPanel();
                stak.Orientation = Orientation.Horizontal;
                stak.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                stak.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                if (filterValue == "Process")
                {
                    columnNames = generator.selectProcessNames;
                }
                else if (filterValue == "Global(0)")
                {
                    columnNames = generator.selectGlobalZeroNames;
                }
                else if (filterValue == "Global(_Total)")
                {
                    columnNames = generator.selectGlobalTotalNames;
                }

                stak.Children.Add(andDockPanelGenerator(new System.Windows.Thickness(0, 10, 0, 0), //dock
                                                            new System.Windows.Thickness(15, 0, 0, 0), //variable cb
                                                            100,
                                                            new System.Windows.Thickness(40, 0, 0, 0), // operators cb
                                                            60,
                                                            new System.Windows.Thickness(20, 0, 0, 0), // valueLabel
                                                            new System.Windows.Thickness(10, 0, 0, 0), // textBox
                                                            100,
                                                            new System.Windows.Thickness(0, 0, 10, 0), // final
                                                            50,
                                                            columnNames));
                mainStackPanel.Children.Add(stak);
            }

            stak = new StackPanel();
            stak.Orientation = Orientation.Horizontal;
            stak.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stak.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            stak.Children.Add(runButtonGenerator());
            mainStackPanel.Children.Add(stak);
        }


        private DockPanel andDockPanelGenerator(System.Windows.Thickness dockPanelMargins,
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
            dockPanel = new DockPanel
            {
                Margin = dockPanelMargins
            };

            stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            andSelectNameCB = new ComboBox
            {
                Margin = variableComboMargins,
                Width = variableComboWidth,
                ItemsSource = cbNames,
                SelectedIndex = 0
            };
            stackPanel.Children.Add(andSelectNameCB);

            andOperatorsCB = new ComboBox
            {
                Margin = operatorsComboMargins,
                Width = operatorsComboWidth
            };
            string[] list = { "==", ">", ">=", "<", "<=", "!=" };
            andOperatorsCB.ItemsSource = list;
            andOperatorsCB.SelectedIndex = 1;
            stackPanel.Children.Add(andOperatorsCB);

            label = new Label
            {
                Margin = valueLabelMargins,
                Content = "Value:"
            };
            stackPanel.Children.Add(label);

            andValue = new TextBox
            {
                Margin = txtBoxMargins,
                Width = txtBoxWidth,
                Text = "0"
            };
            stackPanel.Children.Add(andValue);

            dockPanel.Children.Add(stackPanel);

            finalAndCB = new ComboBox
            {
                Margin = finalComboboxMargins,
                Width = finalComboBoxWidth
            };
            string[] finalList = { ";" };
            finalAndCB.ItemsSource = finalList;
            finalAndCB.SelectedIndex = 0;

            DockPanel.SetDock(finalAndCB, Dock.Right);
            dockPanel.Children.Add(finalAndCB);
            dockPanel.Children.Add(new Label());

            return dockPanel;
        }

        /*
        * Converts list to data table
        */
        private static DataTable ConvertListToDataTable(DataTable dt, List<string[]> list, string[] columns)
        {
             dt = new DataTable();

            // Get max columns.
            int columnsNum = columns.Length;
            for (int i = 0; i < columnsNum; i++)
            {
                dt.Columns.Add(columns[i]);
            }

            foreach (var array in list)
            {
                if (array.Length == columnsNum)
                {
                    dt.Rows.Add(array);
                }

            }
            return dt;
        }


        /*
         * Generate DataGrid from data
         */
        private DataGrid GenerateTable(String[] columns)
        {
            dataGrid = new DataGrid();

            for (int i = 0; i < columns.Length; i++)
            {
                DataGridTextColumn col = new DataGridTextColumn();

                // TODO: replace with the column name
                col.Header = columns[i];
                // should be able to bind data to the row
                dataGrid.Columns.Add(col);
            }
            return dataGrid;
        }

       
        private DataGrid dataGridData(List<string[]> data2dList, string[] columns, DataTable whichTable)
        {
            whichTable = ConvertListToDataTable(whichTable, data2dList, columns);
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

        private void Export()
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

            string json = generator.resultFileDataGenerator();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                File.WriteAllText(filename, json);
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
         
        private Wpf.CartesianChart.ZoomingAndPanning.ZoomingAndPanning PopulateGraph(List<string[]> data2DList, string variable)
        {
            ChartValues<DateModel> data = new ChartValues<DateModel>();

            int variableIndex = Array.IndexOf(generator.processVariables, variable);

            //Get data
            foreach (var array in data2DList)
            {
                try
                {
                    Double value = Double.Parse(array[variableIndex].Replace(".", ","));
                    DateTime timeStamp = DateTime.ParseExact(array[0], "yyyy/MM/dd-HH:mm:ss.ffffff", null);
                    data.Add(new DateModel
                    {
                        DateTime = timeStamp,
                        Value = value
                    });
                }
                catch (IndexOutOfRangeException t)
                {
                    Console.WriteLine(t);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return new Wpf.CartesianChart.ZoomingAndPanning.ZoomingAndPanning(data);
        } 
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
