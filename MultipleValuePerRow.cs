using AzureTableStorage.Model;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AzureTableStorage
{
    class MultipleValuePerRow
    {
        string connstr = "DefaultEndpointsProtocol=https;AccountName=mbittest;AccountKey=o/1YPBuJGpNUb2HmdK0h5ccnzKsvd/UZfXaHku9Nm2NKuixpaPF5H/78Lxo646DWFpP+72n2+IH1X9KyqCkDrQ==;EndpointSuffix=core.windows.net";
        CloudStorageAccount storageAccount;
        CloudTableClient cloudTableClient;


        int nrOfSampSec = 100;
        int scanrateSec = 1;

        List<List<Point>> genBatches = new List<List<Point>>();
        
        public MultipleValuePerRow()
        {
            // Open storage account
            storageAccount = CloudStorageAccount.Parse(connstr);
            cloudTableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());


            generateTestData();





        }

        private void generateTestData()
        {
            // Check if the test tabels exist:

            // Tabel point_0_201912, point_1_201912 etc and point_0_202001, point_1_202001 etc
            var res = simNanoPackagesFromIOTAPI(1);

        }

        private async Task<List<List<Point>>> simNanoPackagesFromIOTAPI(int days)
        {

            // Generate a list of pushed blocks from nano
            // 1 block ever 1000ms holding total set of points 
            // over a month period.
            var dateUtc = new DateTime(2019, 12, 29, 0, 0, 0, DateTimeKind.Utc);
            var unixDateTimeMs = new DateTimeOffset(dateUtc).ToUnixTimeMilliseconds();

            int packagesPerDay = ((60*scanrateSec) * 60 * 24);
            int packagesToGenerate = packagesPerDay * days;

            List<List<Point>> result = new List<List<Point>>();



            List<Point> package;
            Point p;

            var stream = new MemoryStream();
            var binaryWriter = new BinaryWriter(stream);
            Stopwatch stw = new Stopwatch();
            stw.Start();
            for (int packagesI = 0; packagesI < packagesToGenerate; packagesI++)
            {
                package = new List<Point>();
                long value = unixDateTimeMs;


                for (int ponitI = 0; ponitI < nrOfSampSec; ponitI++)
                {
                    binaryWriter.BaseStream.Position = 0;
                    binaryWriter.Write((long)value);

                    byte[] buffer = new byte[(int)binaryWriter.BaseStream.Position];

                    Buffer.BlockCopy((binaryWriter.BaseStream as MemoryStream).ToArray(), 0, buffer, 0, (int)binaryWriter.BaseStream.Position);

                    p = new Point() { Id = ponitI, State = 0, Type = 0, TimeStampUnixUTC = value , Value = buffer};

                    package.Add(p);
                    value += 1000;
                }

                result.Add(package);
                
            }
            Console.WriteLine(stw.ElapsedMilliseconds + " ms Samps " + (packagesToGenerate * nrOfSampSec) + "/samp " + (stw.ElapsedMilliseconds / (packagesToGenerate * nrOfSampSec)) + " ms/samp");
            return result;
        }
        private List<AzurePoint> generateNanoPackage()
        {
            // Generate blocks received from IOTApi
             


            List<AzurePoint> pointArray = new List<AzurePoint>();
            //AzurePoint p;
            //var dateUtc = new DateTime(2019, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            //var unixDateTimems = new DateTimeOffset(dateUtc).ToUnixTimeMilliseconds();

            //for (int customerIndex = 0; customerIndex < nrOfCustomers; customerIndex++)
            //{
            //    var tempStartTime = unixDateTimems;
            //    for(int pointIDIndex = 0; pointIDIndex > nrOfSampPerBatch; pointIDIndex++)
            //    {
            //        p = new AzurePoint() { State = 1, Type = 2, TimeStampUnixUTC = tempStartTime, ValueStr = "12344.3544545" };
            //        pointArray.Add(p);

            //        tempStartTime += 100;
            //    }
            //}

            return pointArray;
        }

        private void runStoreTabel()
        {
            List<AzurePoint> pointArray = generateNanoPackage();


        }
    }
}
