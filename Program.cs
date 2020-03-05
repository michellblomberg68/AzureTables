using AzureTableStorage.Model;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureTableStorage
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("Azure TableStorage Test");

            string connstr = "DefaultEndpointsProtocol=https;AccountName=mbittest;AccountKey=o/1YPBuJGpNUb2HmdK0h5ccnzKsvd/UZfXaHku9Nm2NKuixpaPF5H/78Lxo646DWFpP+72n2+IH1X9KyqCkDrQ==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount;
            CloudTableClient cloudTableClient;

            storageAccount = CloudStorageAccount.Parse(connstr);
            cloudTableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());













            Console.WriteLine("Select P: prepHistoryData, Q: testQuery");
            var k = Console.ReadKey();


            if (k.KeyChar == 'p')
            {
                Console.Clear();
                Console.WriteLine("prepHistoryData");

                new InsertHistory(cloudTableClient);
            }
            else if (k.KeyChar == 'q')
            {
                Console.Clear();
                Console.WriteLine("testQuery");
            }
        }


    }


}
