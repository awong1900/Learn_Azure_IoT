using System;
using Windows.UI.Xaml.Controls;
using System.Text;

using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

using GrovePi;
using GrovePi.Sensors;
using GrovePi.I2CDevices;
using System.Threading;
using System.Threading.Tasks;



namespace HelloAzureRaspi
{
    
    public sealed partial class MainPage : Page
    {
        IDHTTemperatureAndHumiditySensor Temp_Humi = DeviceFactory.Build.DHTTemperatureAndHumiditySensor(Pin.DigitalPin6, DHTModel.Dht11);
        ISoundSensor sound = DeviceFactory.Build.SoundSensor(Pin.AnalogPin0);
        IThreeAxisAccelerometerADXL345 axis = DeviceFactory.Build.ThreeAxisAccelerometerADXL345();

        private Timer periodicTimer;

        static DeviceClient deviceClient;
        static string iotHubUri = "GroveSensorIoTHub.azure-devices.cn";
        static string deviceKey = "sATWumzUT0VQiIpYyKwjr1cRRQpyGJFz4IkRQJRu8FU=";
        static string deviceId = "GroveSensor";

        double SensorTemp;
        double SensorHumi;
        double SensorSound;
        double[] SensorAxis;

        public MainPage()
        {
            this.InitializeComponent();

            deviceClient = DeviceClient.Create(iotHubUri, AuthenticationMethodFactory.CreateAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey), TransportType.Http1);

            periodicTimer = new Timer(this.TimerCallBack, null, 0, 1000);
        }

        private void TimerCallBack(object state)
        {
            try
            {                
                Temp_Humi.Measure();
                SensorTemp = Temp_Humi.TemperatureInCelsius;
                SensorHumi = Temp_Humi.Humidity;
                SensorSound = sound.SensorValue();
                SensorAxis = axis.GetAcclXYZ();

                // Debug Dialog
                System.Diagnostics.Debug.WriteLine("Temp: " + SensorTemp);
                System.Diagnostics.Debug.WriteLine("Humi: " + SensorHumi);
                System.Diagnostics.Debug.WriteLine("Sound: " + SensorSound);
                System.Diagnostics.Debug.WriteLine("AxisX: " + SensorAxis[0]);
                System.Diagnostics.Debug.WriteLine("AxisY: " + SensorAxis[1]);
                System.Diagnostics.Debug.WriteLine("AxisZ: " + SensorAxis[2]);

                //var telemetryDataPoint = new
                //{
                //    deviceId = "myFirstDevice",
                //    temp = SensorTemp,
                //    humidity = SensorHumi,
                //    sound = SensorSound,
                //    axisx = SensorAxis[0],
                //    axisy = SensorAxis[1],
                //    axisz = SensorAxis[2]
                //};
                //var messageSerialized = JsonConvert.SerializeObject(telemetryDataPoint);
                //var encodedMessage = new Message(Encoding.ASCII.GetBytes(messageSerialized));
                //await deviceClient.SendEventAsync(encodedMessage);
                //Console.WriteLine("{0} > Sent message: Device ID={1}, WindSpeed={2}", DateTime.Now, telemetryDataPoint.deviceId, telemetryDataPoint.windSpeed);

                //await Task.Delay(1000);

                /* UI updates must be invoked on the UI thread */
                var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    Temperature.Text = "Temperature: " + SensorTemp + "C";
                    Humidity.Text = "Humidity: " + SensorHumi + "%";
                    SoundSensor.Text = "Sound: " + SensorSound;
                    Accelemeter.Text = "Axis: " + SensorAxis[0].ToString() + " " + SensorAxis[1].ToString() + " " + SensorAxis[2].ToString();
                });

                // Send sensor data to Azure IoT Hub
                SendDeviceToCloudMessagesAsync();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        // Send data to Azure
        private async void SendDeviceToCloudMessagesAsync()
        {
            double currentHumidity = SensorTemp;
            double currentTemperature = SensorHumi;
            double currentAxisX = SensorAxis[0];
            double currentAxisY = SensorAxis[1];
            double currentAxisZ = SensorAxis[2];
            double currentSound = SensorSound;

            var telemetryDataPoint = new
            {
                time = DateTime.Now.ToString(),
                deviceId = deviceId,
                currentHumidity = currentHumidity,
                currentTemperature = currentTemperature,
                currentSound = currentSound,
                currentAxisX = currentAxisX,
                currentAxisY = currentAxisY,
                currentAxisZ = currentAxisZ
            };
            var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));

            await deviceClient.SendEventAsync(message);
            System.Diagnostics.Debug.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
        }
    }
}
