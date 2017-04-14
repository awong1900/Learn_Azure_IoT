using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Diagnostics;
using System.Threading;

namespace ReadEventHub
{
    class Program
    {
        static void Main(string[] args)
        {
            string eventHubName = "eventhub2";
            string eventHubConnectionString = "Endpoint=sb://teneventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=kZpFUBYn0Hz+JRkLw+Trp20qfbkveP8Zek7L1nZb6l0=";
            
            string storageAccountName = "tenstorage";
            string storageAccountKey = "2tE11mpduhRGJGLc0T52OZLQW17EsPWwroluy0GsQYwdfQYdX6X+Zp1zLmSO8IHpVhTyt5zdd+wMPgm0jOI6qA==";
            // string EndpointSuffix = "core.chinacloudapi.cn"; //for china
            string EndpointSuffix = "core.windows.net";
            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix={2}", 
                storageAccountName, storageAccountKey, EndpointSuffix);

            ReadEventHub(eventHubName, eventHubConnectionString, storageConnectionString);
            // SendingRandomMessages(eventHubConnectionString, eventHubName);
        }

        static void SendingRandomMessages(String connectionString, String eventHubName)
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, eventHubName);
            while (true)
            {
                try
                {
                    var message = Guid.NewGuid().ToString();
                    Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, message);
                    eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(message)));
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                    Console.ResetColor();
                }

                Thread.Sleep(1000);
            }
        }


        static void ReadEventHub(String eventHubName, String eventHubConnectionString, String storageConnectionString)
        {
            string eventProcessorHostName = Guid.NewGuid().ToString();
            EventProcessorHost eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName, EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
            Console.WriteLine("Registering EventProcessor...");
            var options = new EventProcessorOptions();
            options.ExceptionReceived += (sender, e) => { Console.WriteLine(e.Exception); };
            eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>(options).Wait();

            Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }



    }
}
