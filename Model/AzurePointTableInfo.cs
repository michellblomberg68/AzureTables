using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTableStorage.Model
{
    class AzurePointTableInfo : TableEntity
    {
        public long customerID { get; set; }

        public long pointID { get; set; }

        public string tableName { get; set; }
                
        public AzurePointTableInfo() { }
        public AzurePointTableInfo(long customerID,long pointID, string tableName)
        {
            PartitionKey = support.padZeroOnItem(8, customerID) + "-" + support.padZeroOnItem(9, pointID);
            RowKey = tableName;
        }
    }
}
