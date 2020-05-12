using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockExchangeData.Models.Mongo
{
    public class Purchase
    {
        public int Quantity { get; set; }

        public decimal? PurchasePrice { get; set; }
    }
}
