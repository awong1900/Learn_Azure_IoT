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
            CloudTable table = tableClient.GetTableReference("mytable02");
            //TableQuery<DeviceEntity> query = new TableQuery<DeviceEntity>();
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<DeviceEntity> query = new TableQuery<DeviceEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "myFirstDevice"));

            // Print the fields for each customer.
            foreach (DeviceEntity entity in table.ExecuteQuery(query))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                    entity.DeviceId, entity.WindSpeed);
            }
            Console.Read();
        }
    }

    public class DeviceEntity : TableEntity
    {
        public DeviceEntity(string deviceId, string windSpeed)
        {
            this.PartitionKey = deviceId;
            this.RowKey = windSpeed;
        }

        public DeviceEntity() { }

        public string DeviceId { get; set; }

        public string WindSpeed { get; set; }
    }
}
