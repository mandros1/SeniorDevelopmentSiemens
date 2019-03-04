using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SiemensPerformance
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SelectionBox : Window
    {
        private DataGenerator generator;
        private ComboBox pCmbo;
        private ComboBox pdCmbo;
        private ComboBox vCmbo;

        public SelectionBox(DataGenerator generator)
        {
            this.generator = generator;
            
            InitializeComponent();
            pCmbo = this.FindName("Process") as ComboBox;
            pdCmbo = this.FindName("ProcessID") as ComboBox;
            vCmbo = this.FindName("Variable") as ComboBox;
            populateProcesses();
            populateVariables();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Process_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<int> ids = generator.getProcessIDs(pCmbo.Text);
            pdCmbo.Items.Clear();
            foreach (var id in ids)
            {
                pdCmbo.Items.Add(id);
            }
        }

        private void ProcessID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void populateProcesses()
        {
            var processes = generator.processNames;

            pCmbo.Items.Clear();
            foreach (var process in processes)
            {
                pCmbo.Items.Add(process);
            } 
        }

        private void populateVariables()
        {
            string[] variables = generator.numericProcessVariables;
            
            foreach (var variable in variables)
            {
                vCmbo.Items.Add(variable);
            }
        }

        public string ShowSelectionDialog()
        {
            this.ShowDialog();
            return pCmbo.Text + "," + pdCmbo.Text + "," + vCmbo.Text;
        }
    }
}
