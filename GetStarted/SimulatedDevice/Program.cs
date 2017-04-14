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
        static string iotHubUri = "teniothub3.azure-devices.net";
        static string deviceKey = "97IY3uREkjPZ1jASopAqgUupMNTtGvUEMepP6fLefkE=";
        static string deviceId = "123";

        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey), TransportType.Mqtt);

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

                var telemetryDataPoint = new
                {
                    deviceId = "myFirstDevice",
                    windSpeed = currentWindSpeed
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                string levelValue;

                if (rand.NextDouble() > 0.7)
                {
                    messageString = "This is a critical message";
                    levelValue = "critical";
                }
                else
                {
                    levelValue = "normal";
                }

                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties.Add("level", levelValue);

                //await deviceClient.SendEventAsync(message);
                //deviceClient.SendEventAsync.
                Console.WriteLine("{0} > Sent message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000);
            }

        }


        private static async void ReceiveC2dAsync()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                Console.ResetColor();

                await deviceClient.CompleteAsync(receivedMessage);
            }
        }
    }
}
