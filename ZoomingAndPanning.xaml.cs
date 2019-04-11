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

namespace Wpf.CartesianChart.ZoomingAndPanning
{
    public partial class ZoomingAndPanning : INotifyPropertyChanged
    {
        private ZoomingOptions _zoomingMode;

        public ZoomingAndPanning(ChartValues<DateModel> data)
        {
            InitializeComponent();

            var gradientBrush = new LinearGradientBrush {StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)};
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(33, 148, 241), 0));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));

            var dayConfig = Mappers.Xy<DateModel>()
                .X(dayModel => (double)dayModel.DateTime.Ticks / TimeSpan.FromHours(1).Ticks)
                .Y(dayModel => dayModel.Value);

            SeriesCollection = new SeriesCollection(dayConfig)
            {
                new LineSeries
                {
                    Values = data,
                    Fill = gradientBrush,
                    StrokeThickness = 1,
                    PointGeometrySize = 0
                }
            };

            Console.WriteLine(data);

            ZoomingMode = ZoomingOptions.X;

            X.MinValue = double.NaN;
            X.MaxValue = double.NaN;
            Y.MinValue = double.NaN;
            Y.MaxValue = double.NaN;

            XFormatter = val => new System.DateTime((long)(val * TimeSpan.FromHours(1).Ticks)).ToString("T");
            YFormatter = val => val.ToString();

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

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
        
        private void ResetZoomOnClick(object sender, RoutedEventArgs e)
        {
            X.MinValue = double.NaN;
            X.MaxValue = double.NaN;
            Y.MinValue = double.NaN;
            Y.MaxValue = double.NaN;
        }
    }
}
