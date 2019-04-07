using System;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace SiemensPerformance
{
    
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
                //SelectionPopulate();
            }
        }

        //Generates and returns a new TabItem object
        private TabItem GenerateTabItem()
        {
            //var myProgressBar = (ProgressBar)this.FindName("pBar");
            DataDisplayTab tab = new DataDisplayTab();
            if(tab.displayable) return tab;
            return null;
        }
    }
}
