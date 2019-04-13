using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using SiemensPerformance;

/*
 * This class is the behind code for the Graph. It displays whatever data is set to it in the form of  ChartValues<DataModel>
 */
namespace Wpf.CartesianChart.ZoomingAndPanning
{
    public partial class ZoomingAndPanning : INotifyPropertyChanged
    {
        private ZoomingOptions _zoomingMode;
        private LinearGradientBrush gradientBrush;
        private ChartValues<DateModel> data;

        /*
         * Constructor
         * Fills graph with initial given data
         */
        public ZoomingAndPanning(ChartValues<DateModel> dataInput)
        {
            InitializeComponent();

            data = dataInput;

            gradientBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(33, 148, 241), 0));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));

            populateGraph(data);

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

        /*
         * Changes zoom option (only used to set to default setting)
         */ 
        public ZoomingOptions ZoomingMode
        {
            get { return _zoomingMode; }
            set
            {
                _zoomingMode = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        /*
         * Resets zoom to show the whole graph with the y axis starting at zero
         */
        private void ResetZoomOnClick(object sender, RoutedEventArgs e)
        {
            populateGraph(data);
        }

        /*
         * Resets zoom to show the whole graph with the y axis starting at zero
         */
        private void Update(object sender, RoutedEventArgs e)
        {
            string startString = startTime.Text;
            string endString = endTime.Text;
            try
            {
                DateTime start = DateTime.ParseExact(startString, "yyyy-MM-ddTHH:mm:ss", null);
                DateTime end = DateTime.ParseExact(endString, "yyyy-MM-ddTHH:mm:ss", null);
                setTimeSpan(start, end);
            }catch (Exception)
            {
                Console.WriteLine("Timespan error");
            }
        }

        /*
         * Populate Graph between two DateTime constraints
         */
        private void setTimeSpan(DateTime start, DateTime end)
        {
            if (start < end)
            {
                ChartValues<DateModel> data2 = new ChartValues<DateModel>();

                foreach (DateModel dm in data)
                {
                    if (dm.DateTime > start & dm.DateTime < end)
                    {
                        data2.Add(dm);
                    }
                }

                populateGraph(data2);
            }
            else
            {
                MessageBox.Show("Start time must be less than end time");
            }
        }

        private void populateGraph(ChartValues<DateModel> insertData)
        {
            var dayConfig = Mappers.Xy<DateModel>()
               .X(dayModel => (double)dayModel.DateTime.Ticks / TimeSpan.FromHours(1).Ticks)
               .Y(dayModel => dayModel.Value);

            SeriesCollection = new SeriesCollection(dayConfig)
            {
                new LineSeries
                {
                    Values = insertData,
                    Fill = gradientBrush,
                    StrokeThickness = 1,
                    PointGeometrySize = 0
                }
            };

            ZoomingMode = ZoomingOptions.X;

            X.MinValue = double.NaN;
            X.MaxValue = double.NaN;
            Y.MinValue = 0;
            Y.MaxValue = double.NaN;

            XFormatter = val => new System.DateTime((long)(val * TimeSpan.FromHours(1).Ticks)).ToString("T");
            YFormatter = val => val.ToString();

            DataContext = this;
            setTimeSpanDefault(insertData);
        }

        private void setTimeSpanDefault(ChartValues<DateModel> insertData)
        {
            startTime.Text = insertData[0].DateTime.ToString("yyyy-MM-ddTHH:mm:ss");
            endTime.Text = insertData[insertData.Count-1].DateTime.ToString("yyyy-MM-ddTHH:mm:ss");
        }
    }
}
