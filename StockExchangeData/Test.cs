using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockExchangeData
{
    public class Test
    {
        [JsonProperty("marketSummaryResponse")]
        public MarketSummaryResponse MarketSummaryResponse { get; set; }
    }
}
