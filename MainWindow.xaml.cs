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

namespace SiemensPerformance
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public Func<double, string> Formatter { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            SeriesCollection CPU = new SeriesCollection
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
            RankGraph2.Series = Memory;
        }

        private void Info_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Make the DataTable object.
            DataTable dt = new DataTable("People");

            // Add columns to the DataTable.
            dt.Columns.Add("First Name",
                System.Type.GetType("System.String"));
            dt.Columns.Add("Last Name",
                System.Type.GetType("System.String"));
            dt.Columns.Add("Occupation",
                System.Type.GetType("System.String"));
            dt.Columns.Add("Salary",
                System.Type.GetType("System.Int32"));

            // Make all columns required.
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt.Columns[i].AllowDBNull = false;
            }

            // Make First Name + Last Name require uniqueness.
            DataColumn[] unique_cols =
            {
        dt.Columns["First Name"],
        dt.Columns["Last Name"]
    };
            dt.Constraints.Add(new UniqueConstraint(unique_cols));

            // Add items to the table.
            dt.Rows.Add(new object[]
                {"Rod", "Stephens", "Nerd", 10000});
            dt.Rows.Add(new object[]
                {"Sergio", "Aragones", "Cartoonist", 20000});
            dt.Rows.Add(new object[]
                {"Eoin", "Colfer", "Author", 30000});
            dt.Rows.Add(new object[]
                {"Terry", "Pratchett", "Author", 40000});

            // Make the DataGridView use the DataTable as its data source.

            Info.DataContext = dt.DefaultView;
        }
    }
}
