using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTableStorage.Model
{
    class AzurePointTable : TableEntity
    {
        public byte[] AzurePointProtobufStr { get; set; }

        public AzurePointTable() { }
        public AzurePointTable(long id, long tick)
        {
            PartitionKey = id.ToString();
            RowKey = tick.ToString();
        }
    }
}
