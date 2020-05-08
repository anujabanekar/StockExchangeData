using Newtonsoft.Json;

namespace StockExchangeData
{
    public class Result
    {
        [JsonProperty("fullExchangeName")]
        public string FullExchangeName { get; set; }


        [JsonProperty("symbol")]
        public string Symbol { get; set; }

    }
}