using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Preliminary test. We later test in Azure.
            var message = "{\"deviceId\":\"jhjhhjh6b1c\",\"windSpeed\":\"177\"" +
                          //"\"messageType\": \"impression\"," +
                          //"\"displayedAdId\":\"3149351f-3c9e-4d0a-bfa5-d8caacfd77f2\",\"timestamp\":\"2016-08-22T16:32:45.892Z\"," +
                          //"\"faces\":{\"age\":41,\"gender\":\"male\"," +
                          //"\"scores\":{\"anger\":0,\"contempt\":0.1,\"disgust\":0.2,\"fear\":0.3,\"happiness\":0.4,\"neutral\":0.5,\"sadness\":0.6,\"surprise\":0.7
                          "}";
            Run(message, new TraceWriter());
        }

    public static void Run(string myEventHubMessage, TraceWriter log)
    {
        // Retrieve storage account from connection string.
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));

        // Create the blob client.
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        // Retrieve reference to a previously created container.
        CloudBlobContainer container = blobClient.GetContainerReference("tenblob01");

        // Retrieve reference to a blob named "myblob".
        CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");

        // Create or overwrite the "myblob" blob with contents from a local file.
        using (var fileStream = System.IO.File.OpenRead(@"path\myfile"))
        {
            blockBlob.UploadFromStream(fileStream);
        }

        // This will be removed after the end-to-end test.
        log.Info($"Event Hub trigger function processed message {myEventHubMessage}.");
    }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static void Run_SQLDB(string myEventHubMessage, TraceWriter log)
        {
            using (var cmd = new SqlCommand())
            {
                try
                {
                    cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString);
                    cmd.Connection.Open();

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "PersistABC_SP";
                    cmd.Parameters.Add(new SqlParameter("@json", myEventHubMessage));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // This can fail only as a result of a cmd problem.
                    log.Info($"Event Hub trigger function failed with {ex.Message}.");
                }
            }

            // This will be removed after the end-to-end test.
            log.Info($"Event Hub trigger function processed message {myEventHubMessage}.");
        }
    }

    class TraceWriter
    {
        public void Info(string message)
        {
            // do nothing - it is just a simulation
        }
    }
}
