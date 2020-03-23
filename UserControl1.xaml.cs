using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Thermometer /* mid온도 값 위한 메서드 및 변수 추가 */
{
    /// <summary>
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserControl1 : UserControl, INotifyPropertyChanged
    {
        /* 속성 변경알림 구현 https://docs.microsoft.com/ko-kr/dotnet/framework/wpf/data/how-to-implement-property-change-notification */
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName]string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private bool isCelsius = true;
        public bool IsCelsius
        {
            get { return isCelsius; }
            set
            {
                isCelsius = value; // value는 새로 들어오는 값을 의미 public bool isCelsius와 같은 의미라는데 잘 모르겠다..
                NotifyPropertyChanged(nameof(MinTemperatureStr));
                NotifyPropertyChanged(nameof(MaxTemperatureStr));
                NotifyPropertyChanged(nameof(TemperatureText));
            }

        }
        private double minTemp = -10.0;
        public double MinTemperature
        {
            get { return  minTemp; }
            set
            {
                minTemp = value;
                temperatureStep = (temperatureTube.ActualHeight - (bulb.ActualHeight / 2)) / (maxTemp - minTemp);
                NotifyPropertyChanged(nameof(TemperatureHeight));
                NotifyPropertyChanged(nameof(MinTemperatureStr));
            }
        }
        // midtemp 추가
        private double midTemp = 15.0;
        public double MidTemperature
        {
            get { return midTemp; }
            set
            {
                midTemp = value;
                temperatureStep = (temperatureTube.ActualHeight - (bulb.ActualHeight / 2)) / (maxTemp - minTemp);
                NotifyPropertyChanged(nameof(TemperatureHeight));
                NotifyPropertyChanged(nameof(MidTemperatureStr));

            }
        }
        private double maxTemp = 40.0;
        public double MaxTemperature
        {
            get { return maxTemp; }
            set
            {
                maxTemp = value;
                temperatureStep = (temperatureTube.ActualHeight - (bulb.ActualHeight / 2)) / (maxTemp - minTemp);
                NotifyPropertyChanged(nameof(TemperatureHeight));
                NotifyPropertyChanged(nameof(MaxTemperatureStr));
            }
        }

        public static readonly DependencyProperty TemperatureProperty =
            DependencyProperty.Register(
                "Temperature", 
                typeof(double), 
                typeof(UserControl1), 
                new PropertyMetadata(37.0)
            );

        public string MinTemperatureStr
        {
            get { return $"{(int)minTemp}°" + (isCelsius ? "C" : "F"); }
        }
        public string MidTemperatureStr
        {
            get { return $"{(int)midTemp}°" + (isCelsius ? "C" : "F"); }
        }
        public string MaxTemperatureStr
        {
            get { return $"{(int)maxTemp}°" + (isCelsius ? "C" : "F"); }
        }

        private double temperatureStep = 1;
        public double TemperatureHeight
        {
            get {
                return bulb != null ? 
                    ((Temperature - minTemp) * temperatureStep) + (bulb.ActualHeight / 2) : ((Temperature - minTemp) * temperatureStep); }
        }
        public string TemperatureText
        {
            get { return $"{(int)Temperature}°" + (isCelsius ? "C" : "F"); }
        }

        public double Temperature
        {
            get { return (double)GetValue(TemperatureProperty); }
            set { SetValue(TemperatureProperty, value); }
        }

        public UserControl1()
        {
            this.DataContext = this;
            InitializeComponent();
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.Property.Name)
            {
                case "ActualHeight":
                    {
                        temperatureStep = (temperatureTube.ActualHeight - (bulb.ActualHeight / 2)) / (maxTemp - minTemp);
                        NotifyPropertyChanged(nameof(TemperatureHeight));
                     }
                    break;
                case "Temperature":
                    NotifyPropertyChanged(nameof(TemperatureHeight));
                    NotifyPropertyChanged(nameof(TemperatureText));
                    break;
            }
        }

    }
}
