using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.IO;

namespace TestBlob
{
    class Program
    {
        static void Main(string[] args)
        {
            var ConnectString = "DefaultEndpointsProtocol=https;AccountName=tenstorage01;AccountKey=TSC4ui7bmOczfjAYlvHx3cfm4/ME03nDQvfoeZXGal5Jl7OODM5tWixtRa+G3n2NR2Mzm6Y06j7qAYq/Z9JwPQ==;EndpointSuffix=core.windows.net";

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously crejated container.
            CloudBlobContainer container = blobClient.GetContainerReference("tenblob01");

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob02");

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = GenerateStreamFromString("1234"))
            {
                blockBlob.UploadFromStream(fileStream);
            }
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
    }
}
