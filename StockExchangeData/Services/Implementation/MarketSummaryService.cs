using StockExchangeData.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockExchangeData.Services.Implematation
{
    public class MarketSummaryService : IMarketSummaryService
    {

        public string Key = Environment.GetEnvironmentVariable("YahooApiKey", EnvironmentVariableTarget.Machine);
        public string Host = Environment.GetEnvironmentVariable("YahooApiHost", EnvironmentVariableTarget.Machine);

        private readonly HttpClient client = new HttpClient();

        public async Task<HttpResponseMessage> GetMarkeySummaryAsync()
        {
           BuildClient();
           return await client.GetAsync("https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/get-summary?region=US&lang=en");
        }

        private void BuildClient()
        {
            client.DefaultRequestHeaders.Add("x-rapidapi-host", Host);
            client.DefaultRequestHeaders.Add("x-rapidapi-key", Key);
        }
    }
}
