﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockExchangeData.Models.Mongo
{
    public class Entity
    {
        public ObjectId _id { get; set; }
        public string Symbol { get; set; }

        public decimal? Price { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
