using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTableStorage.Model
{
    class AzurePointTableRow : TableEntity
    {
        public byte[] AzurePointProtobufStr { get; set; }

        public int Type { get; set; }

        public long Tick { get; set; }
        public bool anyStateFail { get; set; }
        public bool anyStateOk { get; set; }

        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public double MedValue { get; set; }

        public AzurePointTableRow() { }
        public AzurePointTableRow(DateTime firstSamp)
        {
            PartitionKey = firstSamp.ToString("yyyyMMddHH");
            RowKey = firstSamp.ToString("yyyyMMddHHmm");
        }
    }
}
