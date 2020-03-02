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

            // TEST: Run 
            MultipleValuePerRow multipleValuePerRow = new MultipleValuePerRow();


            Console.WriteLine("Select i: insert, q: query");
            var k = Console.ReadKey();



            if (k.KeyChar == 'i')
            {
                //MultipleValuePerRow multipleValuePerRow = new MultipleValuePerRow();


                //Console.WriteLine("Insert as fast as possible 1 samp / row Partion = pointID");
                //var tableName = "PointData";

                //CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);
                //if (!cloudTable.Exists())
                //    await cloudTable.CreateAsync();

                //while (true)
                //{
                //    // Prepare 100000
                //    List<AzurePoint> entities = new List<AzurePoint>();
                //    long startIme = DateTime.UtcNow.Ticks;
                //    for (int c = 0; c < 100000; c++)
                //    {
                //        entities.Add(new AzurePoint(12345, startIme) { Type = 1, State = 2, ValueStr = startIme.ToString() });
                //        startIme++;
                //    }

                //    // R
                //    List<List<AzurePoint>> workItems = new List<List<AzurePoint>>();

                //    List<Task> runners = new List<Task>();
                //    long timer = DateTime.UtcNow.Ticks;
                //    int nrOfBatch = 100;
                //    int nextPos = 0;
                //    int nrOfItemsInBatch = 100;

                //    // Prepare one list per batch 
                //    for (int x = 0; x < nrOfBatch; x++)
                //    {
                //        // Get next 100 samps from entities
                //        List<AzurePoint> slice = new List<AzurePoint>();

                //        if ((nextPos + 100) > entities.Count)
                //            nrOfItemsInBatch = entities.Count - ((nextPos + 100));

                //        slice.AddRange(entities.GetRange(nextPos, nrOfItemsInBatch));
                //        workItems.Add(slice);
                //        nextPos += 100;
                //    }

                //    for (int x = 0; x < nrOfBatch; x++)
                //    {
                //        // Get next 100 samps from entities

                //        runners.Add(InsertBatchEntiy(cloudTable, workItems[x]));
                //        //Console.WriteLine("Batch inserted");
                //        nextPos += 100;
                //    }

                //    await Task.WhenAll(runners);


                //    Console.WriteLine((nrOfBatch * 100) + " Inserts in " + ((DateTime.UtcNow.Ticks - timer) / TimeSpan.TicksPerMillisecond + "ms" + "Avg " + ((DateTime.UtcNow.Ticks - timer) / TimeSpan.TicksPerMillisecond / (nrOfBatch * 100)) + "ms / Insert"));

                //    // Run create table

                //    // Run insert simulate x customers with x samps/sec of double values
                //}
            }
            else
            {
                //Console.WriteLine("Read 1000 ");

                //CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);
                //if (!cloudTable.Exists())
                //    await cloudTable.CreateAsync();

                //while (true)
                //{
                //    // Prepare 100000
                //    List<AzurePoint> entities = new List<AzurePoint>();
                //    long startIme = DateTime.UtcNow.Ticks;
                //    for (int c = 0; c < 100000; c++)
                //    {
                //        entities.Add(new AzurePoint(12345, startIme) { Type = 1, State = 2, ValueStr = startIme.ToString() });
                //        startIme++;
                //    }

                //    // R
                //    List<List<AzurePoint>> workItems = new List<List<AzurePoint>>();

                //    List<Task> runners = new List<Task>();
                //    long timer = DateTime.UtcNow.Ticks;
                //    int nrOfBatch = 100;
                //    int nextPos = 0;
                //    int nrOfItemsInBatch = 100;

                //    // Prepare one list per batch 
                //    for (int x = 0; x < nrOfBatch; x++)
                //    {
                //        // Get next 100 samps from entities
                //        List<AzurePoint> slice = new List<AzurePoint>();

                //        if ((nextPos + 100) > entities.Count)
                //            nrOfItemsInBatch = entities.Count - ((nextPos + 100));

                //        slice.AddRange(entities.GetRange(nextPos, nrOfItemsInBatch));
                //        workItems.Add(slice);
                //        nextPos += 100;
                //    }

                //    for (int x = 0; x < nrOfBatch; x++)
                //    {
                //        // Get next 100 samps from entities

                //        runners.Add(InsertBatchEntiy(cloudTable, workItems[x]));
                //        //Console.WriteLine("Batch inserted");
                //        nextPos += 100;
                //    }

                //    await Task.WhenAll(runners);


                //    Console.WriteLine((nrOfBatch * 100) + " Inserts in " + ((DateTime.UtcNow.Ticks - timer) / TimeSpan.TicksPerMillisecond + "ms" + "Avg " + ((DateTime.UtcNow.Ticks - timer) / TimeSpan.TicksPerMillisecond / (nrOfBatch * 100)) + "ms / Insert"));



                }
            Console.WriteLine("Exit with enter");
            Console.ReadKey();

        }

        static async Task<TableBatchResult> InsertBatchEntiy(CloudTable table, List<TableEntity> batchItems)
        {
            TableBatchOperation batch = new TableBatchOperation();

            for(int c = 0; c < batchItems.Count;c++ )
            {
                batch.Insert(batchItems[c]); 
            }
            return await table.ExecuteBatchAsync(batch);
        }

        static async Task InsertEntity(CloudTable table, TableEntity entity)
        {
            TableOperation tableOperation = TableOperation.Insert(entity);

            TableResult res = await table.ExecuteAsync(tableOperation);
            AzurePoint insertedPoiint = res.Result as AzurePoint; 
            
        }
    }


}
