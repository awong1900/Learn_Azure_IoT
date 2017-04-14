using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager 
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types
namespace ListTable
{
    class Program
    {
        static void Main(string[] args)
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("sensortable");
            //TableQuery<DeviceEntity> query = new TableQuery<DeviceEntity>();
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<DeviceEntity> query = new TableQuery<DeviceEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "GroveSensor"));

            // Print the fields for each customer.
            foreach (DeviceEntity entity in table.ExecuteQuery(query))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{8}", entity.PartitionKey, entity.RowKey,
                    entity.deviceId, entity.humidity, entity.temperature, entity.sound, entity.axisx,
                    entity.axisy, entity.axisz);
            }
            Console.Read();
        }
    }

    public class DeviceEntity : TableEntity
    {
        public DeviceEntity(string deviceId, string humidity)
        {
            this.PartitionKey = deviceId;
            this.RowKey = humidity;
        }

        public DeviceEntity() { }

        public string deviceId { get; set; }

        public string humidity { get; set; }

        public string temperature { get; set; }

        public string sound { get; set; }

        public string axisx { get; set; }

        public string axisy { get; set; }
        public string axisz { get; set; }
    }
}
