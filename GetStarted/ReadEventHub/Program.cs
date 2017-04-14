using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Diagnostics;

namespace ReadEventHub
{
    class Program
    {
        static void Main(string[] args)
        {
            string eventHubName = "eventhub";
            string eventHubConnectionString = "Endpoint=sb://teneventhub.servicebus.chinacloudapi.cn/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=P9UmPL4ZK/gDt404XinSG/86qWS3huK7XhKyu71C5+8=";

            string storageAccountName = "tentable";
            string storageAccountKey = "V+U6KF2vntqpuVhfnbwTxeVwQFTHMxSloSVYbrSYrMkasoO1l8TGOAMTEnj4cbwqCai83UlxjoNvpFVfZIJc7g==";
            string EndpointSuffix = "core.chinacloudapi.cn";
            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix={2}", 
                storageAccountName, storageAccountKey, EndpointSuffix);

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
