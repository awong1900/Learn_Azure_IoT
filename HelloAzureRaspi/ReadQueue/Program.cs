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
            var queueConnectionString = "Endpoint=sb://tenservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=v2edb+Qu+Cbd+fzbX4bGdcHtNUMkkXP5mfDZgIG8jaM=";
            var queueName = "tenqueue";

            // SendQueue(queueConnectionString, queueName);
            ReadQueue(queueConnectionString, queueName);
        }


        static void ReadQueue(String connectionString, String queueName)
        {
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

        static void SendQueue(String connectionString, String queueName)
        {
            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            var message = new BrokeredMessage("This is a test message!");
            client.Send(message);
        }
    }
}
