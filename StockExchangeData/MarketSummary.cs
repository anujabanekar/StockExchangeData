using Newtonsoft.Json;

namespace StockExchangeData
{
    public class MarketSummary
    {
        [JsonProperty("marketSummaryResponse")]
        public MarketSummaryResponse MarketSummaryResponse { get; set; }
    }
}
