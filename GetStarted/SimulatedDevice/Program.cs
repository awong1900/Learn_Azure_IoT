using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace SimulatedDevice
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "ten-test.azure-devices.cn";
        static string deviceKey = "oBQikRHUhE5s14TPS8mxNrog6K3pHLUH8hVxm85SIIY=";

        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("myFirstDevice", deviceKey), TransportType.Mqtt);

            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }

        private static async void SendDeviceToCloudMessagesAsync()
        {
            double avgWindSpeed = 10; // m/s
            Random rand = new Random();

            while (true)
            {
                double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;
                //string levelVal, messageString = string.Empty;

                //if (rand.NextDouble() > 0.7)
                //{
                //    messageString = "This is a critical message";
                //    levelVal = "critical";
                //}
                //else
                //{
                //    levelVal = "normal"; 
                //}

                //var telemetryDataPoint = new
                //{
                //    deviceId = "myFirstDevice",
                //    windSpeed = currentWindSpeed,
                //    levelValue = levelVal,
                //    messageStr = messageString
                //};

                var telemetryDataPoint = new
                {
                    deviceId = "myFirstDevice",
                    windSpeed = currentWindSpeed
                };
                var messageSerialized = JsonConvert.SerializeObject(telemetryDataPoint);
                var encodedMessage = new Message(Encoding.ASCII.GetBytes(messageSerialized));
                await deviceClient.SendEventAsync(encodedMessage);
                Console.WriteLine("{0} > Sent message: Device ID={1}, WindSpeed={2}", DateTime.Now, telemetryDataPoint.deviceId, telemetryDataPoint.windSpeed);

                await Task.Delay(1000);
            }
        }
    }
}
