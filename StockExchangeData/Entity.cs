using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockExchangeData
{
    public class Entity
    {
        public ObjectId _id { get; set; }
        public string Symbol { get; set; }
    }
}
