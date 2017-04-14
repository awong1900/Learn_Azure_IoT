using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Threading;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common;

namespace ReadDeviceToCloudMessages
{
    class Program
    {
        //static string connectionString = "HostName=teniothub2.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=t9KauPx1KsMwsjfvb/kpwIDTCD9xKXXtuvo4GXwT40k=";
        static string connectionString = "HostName=teniothub3.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=FrrswuBpK5ENJOeMxUtWYc3ZLTbF/4jL02coaaLN4AU=";
        static string iotHubD2cEndpoint = "messages/events";
        static EventHubClient eventHubClient;

        static void Main(string[] args)
        {
            Console.WriteLine("Receive messages. Ctrl-C to exit.\n");
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            CancellationTokenSource cts = new CancellationTokenSource();

            System.Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var tasks = new List<Task>();
            foreach (string partition in d2cPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
            }
            Task.WaitAll(tasks.ToArray());


            //var eventHubPartitionsCount = eventHubClient.GetRuntimeInformation().PartitionCount;
            //string partition = EventHubPartitionKeyResolver.ResolveToPartition("123", eventHubPartitionsCount);
            ////eventHubReceiver = eventHubClient.GetConsumerGroup(consumerGroupName).CreateReceiver(partition, startTime);

            //ReceiveMessagesFromDeviceAsync(partition, cts.Token).Wait();

            //ReceiveFromIothubAsync().Wait();
        }

        private static async Task ReceiveFromIothubAsync()
        {
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);
            var eventHubPartitionsCount = eventHubClient.GetRuntimeInformation().PartitionCount;
            string partition = EventHubPartitionKeyResolver.ResolveToPartition("123", eventHubPartitionsCount);
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);

            EventData eventData = await eventHubReceiver.ReceiveAsync();
            string data = Encoding.UTF8.GetString(eventData.GetBytes());
            Console.WriteLine("Message received. Partition: {0} Data: '{1}'", partition, data);
            Console.ReadLine();
        }

        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
           
            while (true)
            {
                if (ct.IsCancellationRequested)
                    break;
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null)
                    continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received. Partition: {0} Data: '{1}'", partition, data);
            }
        }
    }
}
