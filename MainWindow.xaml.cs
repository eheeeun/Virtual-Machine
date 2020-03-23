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
using System.Windows.Navigation;
using System.Windows.Shapes;

/* default 에서 추가 using */

using System.ComponentModel;
using System.Drawing;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace client
{  
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void myUICallBack(string myStr, TextBox ctl);  // UIcallback method declaration
        static MqttClient client;                                       // MQTT Client type client 
        
        /* user control .dll 추가하고 변수 추가 03/32 */
        Thermometer.UserControl1 thermometer;

        public MainWindow()
        {
            InitializeComponent();
            GetThermometer();                                           // usercontrol call 
            
        }
        private void GetThermometer()
        {
            thermometer = new Thermometer.UserControl1();
            Temperature.Content = thermometer;
        }
        private void Publish() { // publish function
            client.Publish("state/temperature", Encoding.UTF8.GetBytes("24℃"), (byte)2, false); // temperature
            client.Publish("state/humidity", Encoding.UTF8.GetBytes("50%"), (byte)2, false);     // humidity
        
        }

        private void ConnectButton_Click(object sender, EventArgs e) {
            if (ConnectButton.Content.Equals("Connect") == true) {
                ConnectButton.Content = "Disconnect";
                client = new MqttClient("192.168.0.20", 1883, false, null);
                client.Connect(Guid.NewGuid().ToString());
                Publish();
            }
            else if (ConnectButton.Content.Equals("Disconnect") == true){
                ConnectButton.Content = "Connect";
                client.Disconnect();
            }
        }

        private void Sleep(int v)
        {
            throw new NotImplementedException();
        }
    }
}
