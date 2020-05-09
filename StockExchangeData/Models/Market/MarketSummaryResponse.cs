using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockExchangeData
{
    public class MarketSummaryResponse
    {
        [JsonProperty("result")]
        public Result[] Result { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }
    }
}
