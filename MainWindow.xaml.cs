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
using System.Collections.Generic;

namespace SiemensPerformance
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public Func<double, string> Formatter { get; set; }
        private TabControl tabs;

        public MainWindow()
        {

            
            InitializeComponent();

            // Tabs setup
            tabs = (TabControl)this.FindName("logNav");
        }

        // Called when a new log tab item is chosen
        private void LogNav_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl control = sender as TabControl;

            if (control.SelectedIndex == control.Items.Count - 1)
            {
                TabItem tab = GenerateTabItem();    
                if(tab != null) { 
                    control.Items.Insert(control.Items.Count - 1, tab);
                    control.SelectedIndex = control.Items.Count - 2;
                }
            }

            //Repopulate the Select file query when going to the query tab
            if (control.SelectedIndex == 0)
            {
                SelectionPopulate();
            }
        }

        //Generates and returns a new TabItem object
        private TabItem GenerateTabItem()
        {
            DataDisplayTab tab = new DataDisplayTab();
            if(tab.displayable) return tab;
            return null;
        }

        //Populates the Select combo box for Queries
        private void SelectionPopulate()
        {
            TabControl control = this.FindName("logNav") as TabControl;
            ComboBox cmbo = this.FindName("SelectFile") as ComboBox;
            cmbo.Items.Clear();
            for(int i = 1; i < control.Items.Count-1; i++)
            {
                TabItem tab = control.Items.GetItemAt(i) as TabItem;
                cmbo.Items.Add(tab.Header);
            }
        }
    }
}
