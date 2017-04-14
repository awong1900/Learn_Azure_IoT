using Microsoft.ServiceBus.Messaging;
using System;
using System.IO;
using System.Text;

namespace ReadCriticalQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Receive critical messages. Ctrl-C to exit.\n");
            var connectionString = "Endpoint=sb://tenservicebus.servicebus.windows.net/;SharedAccessKeyName=iothubroutes_teniothub3;SharedAccessKey=lE0TnHzy/6vSfHdxms2gYOhr1INREYpjSyu63/IQJak=;EntityPath=tenqueue";
            var queueName = "tenqueue";

            var client = QueueClient.CreateFromConnectionString(connectionString);

            client.OnMessage(message =>
            {
                Stream stream = message.GetBody<Stream>();
                StreamReader reader = new StreamReader(stream, Encoding.ASCII);
                string s = reader.ReadToEnd();
                Console.WriteLine(String.Format("Message body: {0}", s));
            });

            Console.ReadLine();
        }
    }
}
