using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ReadQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Endpoint=sb://tenservicebus.servicebus.chinacloudapi.cn/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gn517yH+6y9/QM5D8XWVOAXcrJTSztSDOjyn5JZcSUQ=";
            var queueName = "tenservicebusqueue";

            //var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            //var message = new BrokeredMessage("This is a test message!");
            //client.Send(message);


            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            client.OnMessage(message =>
            {
                Console.WriteLine(String.Format("Message body: {0}", message.GetBody<String>()));
                Console.WriteLine(String.Format("Message id: {0}", message.MessageId));
            });

            Console.ReadLine();
        }
    }
}
