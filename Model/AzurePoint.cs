using Microsoft.Azure.Cosmos.Table;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTableStorage.Model
{
    [ProtoContract]
    public class AzurePoint
    {
        [ProtoMember(1)] 
        public byte State { get; set; }
        [ProtoMember(2)] 
        public long TimeStampUnixUTC { get; set; }
        [ProtoMember(3)] 
        public string ValueStr { get; set; }
    }
}
