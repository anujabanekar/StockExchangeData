using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockExchangeData.Models.Mongo
{
    public class Entity
    {
        public ObjectId Id { get; set; }
        public string Symbol { get; set; }

        public decimal? Price { get; set; }

        public int TotalQuantity { get; set; }

        public List<Purchase> AddPurchase { get; set; }

        public List<Purchase> SoldStock { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
