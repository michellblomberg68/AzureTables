using Microsoft.Azure.Cosmos.Table;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTableStorage.Model
{
    public class Point
    {
        public long Id { get; set; }
        public byte Type { get; set; }
        public byte State { get; set; }
        public long TimeStampUnixUTC { get; set; }
        public byte[] Value { get; set; }
    }
}
