using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tenapp.Models;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using Newtonsoft.Json;
using Microsoft.ServiceBus.Messaging;
using System.Text;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;

namespace tenapp.Utilities
{
    public class AzureDataStorage
    {
        private String storageConnectString = "DefaultEndpointsProtocol=https;AccountName=tenstorage;AccountKey=2tE11mpduhRGJGLc0T52OZLQW17EsPWwroluy0GsQYwdfQYdX6X+Zp1zLmSO8IHpVhTyt5zdd+wMPgm0jOI6qA==;EndpointSuffix=core.windows.net";

        private string eventHubName = "eventhub";
        private string eventHubConnectionString = "Endpoint=sb://teneventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=kZpFUBYn0Hz+JRkLw+Trp20qfbkveP8Zek7L1nZb6l0=";

        private string queueConnectionString = "Endpoint=sb://tenservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=v2edb+Qu+Cbd+fzbX4bGdcHtNUMkkXP5mfDZgIG8jaM=";
        private string queueName = "tenqueue";

        private string iotHubUri = "iothubfun.azure-devices.net";
        private string deviceKey = "WuxTxrUdvJ65y9TSFbdke6xRDFEaaAK0lXJQmes1lo0=";
        private string deviceName = "firstDevice";

        public void SendDataToBlob(LabDataModels labData)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("tencontainer");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            // Retrieve reference to a blob named "myblob".
            CloudAppendBlob cloudAppendBlob = container.GetAppendBlobReference("sensordatablob");

            // Create or overwrite the "myblob" blob with contents
            var messageSerialized = JsonConvert.SerializeObject(labData);
            cloudAppendBlob.UploadTextAsync(messageSerialized);
        }

        public void SendDataToEventHub(LabDataModels labData)
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString, eventHubName);

            try
            {
                var messageSerialized = JsonConvert.SerializeObject(labData);

                // Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, message);
                eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(messageSerialized)));
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                Console.ResetColor();
            }
        }

        public void SendDataToServiceBusQueue(LabDataModels labData)
        {
            var client = QueueClient.CreateFromConnectionString(queueConnectionString, queueName);
            var messageSerialized = JsonConvert.SerializeObject(labData);
            var message = new BrokeredMessage(messageSerialized);
            client.Send(message);
        }

        public async Task SendDataToIotHubAsync(LabDataModels labData)
        {
            DeviceClient deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceName, deviceKey),
                Microsoft.Azure.Devices.Client.TransportType.Mqtt);
            var messageSerialized = JsonConvert.SerializeObject(labData);
            var encodedMessage = new Message(Encoding.ASCII.GetBytes(messageSerialized));
            await deviceClient.SendEventAsync(encodedMessage);
        }
    }
}