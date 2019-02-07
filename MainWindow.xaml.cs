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
           
        }
    }
}
