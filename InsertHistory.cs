using AzureTableStorage.Model;
using Microsoft.Azure.Cosmos.Table;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureTableStorage
{
    class InsertHistory
    {
        CloudTableClient _cloudTableClient;

        List<List<Point>> genBatches = new List<List<Point>>();

        DateTime _startHistoryDataDateUtc;
        int _customerID;
        int _scanRateMs;
        int _nrOfPoints;
        int _nrOfSampSec;
        int _nrOfMinRowKey;
        int _nrOfMinPartionKey;


        public InsertHistory(CloudTableClient cloudTableClient)
        {
            _cloudTableClient = cloudTableClient;

            // Setup test 
            //_startHistoryDataDateUtc = new DateTime(2020, 02, 27, 0, 0, 0, DateTimeKind.Utc);
            _startHistoryDataDateUtc = DateTime.UtcNow.AddDays(-2);

            _customerID = 12345;
            _scanRateMs = 1000;
            _nrOfPoints = 2;
            _nrOfSampSec = (_scanRateMs / 1000) * _nrOfPoints;

            _nrOfMinPartionKey = 60;
            _nrOfMinRowKey = 1;

            
            prepareInParallell();
 

        }

        void prepareInParallell()
        {
            // Create data for all points from _startHistoryDataDateUtc too now
            Parallel.For(0, _nrOfPoints, async index => { await prepPointHistory(index); });
        }

        private async Task prepPointHistory(long pointid)
        {
            CloudTable cloudTable = null;
            Dictionary<int, DateTime?> res = new Dictionary<int, DateTime?>();
            
            var StartHistory = _startHistoryDataDateUtc;//.ToString("yyyyMM");
            var Now = DateTime.UtcNow;//.ToString("yyyyMM");

            long sampCount = 0;
            long sampCountTimer = DateTime.UtcNow.Ticks;
            int expCount = 3;

            string presentTableName = "";
            while(((Now.Ticks - StartHistory.Ticks) / TimeSpan.TicksPerSecond) > 0)
            {

                // Check that table exists for month
                var tableName = "P" + support.padZeroOnItem(6, pointid) + StartHistory.ToString("yyyyMM");
                if (presentTableName != tableName)
                {
                    Exception tableExp = null;
                    while (expCount > 0)
                    {
                        try
                        {
                            // Check if table exists
                            cloudTable = _cloudTableClient.GetTableReference(tableName);
                            if (!cloudTable.Exists())
                                cloudTable.CreateAsync().Wait();
                            tableExp = null;
                            break;
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine((DateTime.Now.ToString("HH:mm:ss.fff") + ": Table Error " + ex.Message));
                            await Task.Delay(1000 * (3-expCount));

                            tableExp = ex;
                            expCount--;
                        }
                    }

                    if (tableExp != null)
                        throw tableExp;

                    presentTableName = tableName;
                }

                // Generate new partion, thats 1 hour of 60 rows
                var tuple = generatePartionDataSetForPoint(pointid, StartHistory);
                StartHistory = tuple.Item2;
                var rows = tuple.Item1;

                expCount = 3;
                Exception insertExp = null;
                while(expCount > 0)
                {
                    try
                    {
                        // Insert into Table source with batch
                        var t = InsertBatchEntiy(cloudTable, rows);
                        t.Wait();
                        var insertRes = t.Result;
                        insertExp = null;
                        Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + ": Inserted  Blockstart:" + rows[0].RowKey + " " + rows.Count + " rows");
                        sampCount += (rows.Count * (60 * 1000 / _scanRateMs));
                        break;
                    }
                    catch(Exception ex)
                    {
                        insertExp = ex;
                        expCount--;
                        Console.WriteLine((DateTime.Now.ToString("HH:mm:ss.fff") + ": Error " + ex.Message));
                        await Task.Delay(2000);
                    }
                }
                if (insertExp != null)
                {
                    // Failed insert
                    Console.WriteLine((DateTime.Now.ToString("HH:mm:ss.fff") + ": FAILED insert " + insertExp.Message));
                }
            }

            Console.WriteLine("Point " + pointid + " insert " + sampCount + " samps in " + ((DateTime.UtcNow.Ticks - sampCountTimer) / TimeSpan.TicksPerSecond));
        }

        CloudTable createNewTable(long customerID, long pointID, DateTime startTime)
        {
            // Check if table exists
            var tableName = "P" + support.padZeroOnItem(6, pointID) + startTime.ToString("yyyyMM");
            var cloudTable = _cloudTableClient.GetTableReference(tableName);
            if (!cloudTable.Exists())
            {
                AzurePointTableInfo azurePointTableInfo = new AzurePointTableInfo() { customerID = customerID, pointID = pointID, tableName = tableName };

                cloudTable.CreateAsync().Wait();
            }

            return cloudTable;
        }

        async Task<List<TableBatchResult>> InsertBatchEntiy(CloudTable table, List<AzurePointTableRow> batchItems)
        {
            TableBatchOperation batch = new TableBatchOperation();
            List<TableBatchResult> results = new List<TableBatchResult>();

            if (batchItems == null || batchItems.Count == 0)
                throw new ApplicationException("Missing data");

            // Seperate out partionkey
            string partionKey = batchItems[0].PartitionKey;
            for (int c = 0; c < batchItems.Count; c++)
            {
                if (batchItems[c].PartitionKey != partionKey)
                {
                    // New partion, send batch in order to isolate partionKey in each batch
                    results.Add(await table.ExecuteBatchAsync(batch));
                    partionKey = batchItems[c].PartitionKey;
                    batch.Clear();
                }

                batch.Insert(batchItems[c]);
            }
            results.Add(await table.ExecuteBatchAsync(batch));

            return results;
        }

        class ValueSum
        {
            public double min;
            public double max;
            public double med;
            public long nrOfSamps;
            public ValueSum()
            {
                min = double.MaxValue;
                max = double.MinValue;
                med = 0;
            }
        }

        private Tuple<List<AzurePointTableRow>, DateTime> generatePartionDataSetForPoint(long pointID, DateTime startTime)
        {
            var rows = new List<AzurePointTableRow>();

            var end = new DateTime(startTime.Ticks, DateTimeKind.Utc).AddMinutes(_nrOfMinPartionKey);

            List<AzurePoint> points = new List<AzurePoint>();
            while(startTime.Subtract(end).TotalMinutes < 0)
            {
                // Generate block 
                points.Clear();
                ValueSum vs = new ValueSum();
                var nrOfSampsInBlock = ((_nrOfMinRowKey*60)/(_scanRateMs/1000));
                var startTimeOfBlock = new DateTime(startTime.Ticks, DateTimeKind.Utc);
                for (int c = 0; c < nrOfSampsInBlock; c++)
                {
                    AzurePoint p = new AzurePoint() { State = 0, ValueStr = startTime.Ticks.ToString(), TimeStampUnixUTC = startTime.Ticks };
                    points.Add(p);

                    if (startTime.Ticks > vs.max)
                        vs.max = startTime.Ticks;
                    if (startTime.Ticks < vs.min)
                        vs.min = startTime.Ticks;
                    vs.med += startTime.Ticks;
                    vs.nrOfSamps++;

                    startTime = startTime.AddSeconds(1);
                }

                // Generate a row
                AzurePointTableRow azurePointTableRow = new AzurePointTableRow(startTimeOfBlock);
                azurePointTableRow.Type = 3;
                azurePointTableRow.MaxValue = vs.max;
                azurePointTableRow.MinValue = vs.min;
                azurePointTableRow.MedValue = (vs.med / vs.nrOfSamps);
                azurePointTableRow.anyStateFail = false;
                azurePointTableRow.anyStateOk = true;

                MemoryStream stream = new MemoryStream();
                Serializer.Serialize(stream, points);
                azurePointTableRow.AzurePointProtobufStr = stream.ToArray();

                rows.Add(azurePointTableRow);
            }

            return new Tuple<List<AzurePointTableRow>, DateTime>(rows, startTime);
        }


 
    }
}
