﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace StockExchangeData.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class StockDataController : ControllerBase
    {
        private IMemoryCache _cache;
        private readonly HttpClient client = new HttpClient();
        private readonly ILogger<StockDataController> _logger;

        public StockDataController(ILogger<StockDataController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _cache = memoryCache;
        }

        [HttpGet]
        public IEnumerable<Result> GetAsync()
        {
            string Key = Environment.GetEnvironmentVariable("YahooApiKey", EnvironmentVariableTarget.Machine);
            string Host = Environment.GetEnvironmentVariable("YahooApiHost", EnvironmentVariableTarget.Machine);


            IRestResponse marketSummary;
            if (!_cache.TryGetValue("MarketSummary", out marketSummary))
            {

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(3000));

                var client = new RestClient("https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/get-summary?region=US&lang=en");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("x-rapidapi-host", Host);
                request.AddHeader("x-rapidapi-key", Key);
                marketSummary = client.Execute(request);
                Console.WriteLine(marketSummary.Content);
                // Save data in cache.
                _cache.Set("MarketSummary", marketSummary.Content, cacheEntryOptions);
            }
            //var response = await client.GetAsync("https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/get-summary?region=US&lang=en");
            if (marketSummary.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = JsonConvert.DeserializeObject<Test>(marketSummary.Content);
                return content.MarketSummaryResponse.Result;
            }

            /* // var json = "{"marketSummaryResponse":{"result":[{"fullExchangeName":"CME","exchangeTimezoneName":"America / New_York","symbol":"ES = F","regularMarketChange":{"raw":26.75,"fmt":"26.75"},"gmtOffSetMilliseconds":-14400000,"headSymbolAsString":"ES = F","exchangeDataDelayedBy":10,"language":"en","regularMarketTime":{"raw":1588900170,"fmt":"9:09PM EDT"},"regularMarketChangePercent":{"raw":0.9288194,"fmt":"0.93 % "},"exchangeTimezoneShortName":"EDT","quoteType":"FUTURE","regularMarketPrice":{"raw":2906.75,"fmt":"2,906.75"},"marketState":"REGULAR","market":"us24_market","tradeable":false,"exchange":"CME","sourceInterval":10,"shortName":"S & P Futures","region":"US","regularMarketPreviousClose":{"raw":2880.0,"fmt":"2,880.00"},"triggerable":false},{"fullExchangeName":"CBOT","exchangeTimezoneName":"America / New_York","symbol":"YM = F","regularMarketChange":{"raw":238.0,"fmt":"238.00"},"gmtOffSetMilliseconds":-14400000,"headSymbolAsString":"YM = F","exchangeDataDelayedBy":10,"language":"en","regularMarketTime":{"raw":1588900168,"fmt":"9:09PM EDT"},"regularMarketChangePercent":{"raw":0.99819654,"fmt":"1.00 % "},"exchangeTimezoneShortName":"EDT","quoteType":"FUTURE","regularMarketPrice":{"raw":24081.0,"fmt":"24,081.00"},"marketState":"REGULAR","market":"us24_market","tradeable":false,"exchange":"CBT","sourceInterval":10,"shortName":"Dow Futures","region":"US","regularMarketPreviousClose":{"raw":23843.0,"fmt":"23,843.00"},"triggerable":false},{"fullExchangeName":"CME","exchangeTimezoneName":"America / New_York","symbol":"NQ = F","regularMarketChange":{"raw":76.75,"fmt":"76.75"},"gmtOffSetMilliseconds":-14400000,"headSymbolAsString":"NQ = F","exchangeDataDelayedBy":10,"language":"en","regularMarketTime":{"raw":1588900032,"fmt":"9:07PM EDT"},"regularMarketChangePercent":{"raw":0.8426889,"fmt":"0.84 % "},"exchangeTimezoneShortName":"EDT","quoteType":"FUTURE","regularMarketPrice":{"raw":9184.5,"fmt":"9,184.50"},"marketState":"REGULAR","market":"us24_market","tradeable":false,"exchange":"CME","sourceInterval":10,"shortName":"Nasdaq Futures","region":"US","regularMarketPreviousClose":{"raw":9107.75,"fmt":"9,107.75"},"triggerable":false},{"fullExchangeName":"CME","exchangeTimezoneName":"America / New_York","symbol":"RTY = F","regularMarketChange":{"raw":14.599976,"fmt":"14.60"},"gmtOffSetMilliseconds":-14400000,"headSymbolAsString":"RTY = F","exchangeDataDelayedBy":10,"language":"en","regularMarketTime":{"raw":1588900168,"fmt":"9:09PM EDT"},"regularMarketChangePercent":{"raw":1.1361848,"fmt":"1.14 % "},"exchangeTimezoneShortName":"EDT","quoteType":"FUTURE","regularMarketPrice":{"raw":1299.6,"fmt":"1,299.60"},"marketState":"REGULAR","market":"us24_market","tradeable":false,"exchange":"CME","sourceInterval":10,"shortName":"Russell 2000 Futures","region":"US","regularMarketPreviousClose":{"raw":1285.0,"fmt":"1,285.00"},"triggerable":false},{"fullExchangeName":"NY Mercantile","exchangeTimezoneName":"America / New_York","symbol":"CL = F","regularMarketChange":{"raw":0.63000107,"fmt":"0.63"},"gmtOffSetMilliseconds":-14400000,"headSymbolAsString":"CL = F","exchangeDataDelayedBy":30,"language":"en","regularMarketTime":{"raw":1588900168,"fmt":"9:09PM EDT"},"regularMarketChangePercent":{"raw":2.6751637,"fmt":"2.68 % "},"exchangeTimezoneShortName":"EDT","quoteType":"FUTURE","regularMarketPrice":{"raw":24.18,"fmt":"24.18"},"marketState":"REGULAR","market":"us24_market","tradeable":false,"exchange":"NYM","sourceInterval":30,"shortName":"Crude Oil","region":"US","regularMarketPreviousClose":{"raw":23.55,"fmt":"23.55"},"triggerable":false},{"fullExchangeName":"COMEX","exchangeTimezoneName":"America / New_York","symbol":"GC = F","regularMarketChange":{"raw":3.3999023,"fmt":"3.40"},"gmtOffSetMilliseconds":-14400000,"headSymbolAsString":"GC = F","exchangeDataDelayedBy":30,"language":"en","regularMarketTime":{"raw":1588900169,"fmt":"9:09PM EDT"},"regularMarketChangePercent":{"raw":0.19700442,"fmt":"0.20 % "},"exchangeTimezoneShortName":"EDT","quoteType":"FUTURE","regularMarketPrice":{"raw":1729.2,"fmt":"1,729.20"},"marketState":"REGULAR","market":"us24_market","tradeable":false,"exchange":"CMX","sourceInterval":15,"shortName":"Gold","region":"US","regularMarketPreviousClose":{"raw":1725.8,"fmt":"1,725.80"},"triggerable":false},{"fullExchangeName":"COMEX","exchangeTimezoneName":"America / New_York","symbol":"SI = F","regularMarketChange":{"raw":0.11499977,"fmt":"0.11"},"gmtOffSetMilliseconds":-14400000,"headSymbolAsString":"SI = F","exchangeDataDelayedBy":30,"language":"en","regularMarketTime":{"raw":1588900167,"fmt":"9:09PM EDT"},"regularMarketChangePercent":{"raw":0.7376509,"fmt":"0.74 % "},"exchangeTimezoneShortName":"EDT","quoteType":"FUTURE","regularMarketPrice":{"raw":15.705,"fmt":"15.70"},"marketState":"REGULAR","market":"us24_market","tradeable":false,"exchange":"CMX","sourceInterval":15,"shortName":"Silver","region":"US","regularMarketPreviousClose":{"raw":15.59,"fmt":"15.59"},"triggerable":false},{"fullExchangeName":"CCY","exchangeTimezoneName":"Europe / London","symbol":"EURUSD = X","regularMarketChange":{"raw":0.0010578632,"fmt":"0.0011"},"gmtOffSetMilliseconds":3600000,"exchangeDataDelayedBy":0,"language":"en - US","regularMarketTime":{"raw":1588900689,"fmt":"2:18AM BST"},"regularMarketChangePercent":{"raw":0.097627744,"fmt":"0.0976 % "},"exchangeTimezoneShortName":"BST","quoteType":"CURRENCY","regularMarketPrice":{"raw":1.0847163,"fmt":"1.0847"},"marketState":"REGULAR","market":"ccy_market","priceHint":4,"tradeable":false,"exchange":"CCY","sourceInterval":15,"currency":"USD","shortName":"EUR / USD","region":"US","regularMarketPreviousClose":{"raw":1.0836585,"fmt":"1.0837"},"triggerable":true},{"fullExchangeName":"NYBOT","exchangeTimezoneName":"America / New_York","symbol":" ^ TNX","regularMarketChange":{"raw":-0.08000004,"fmt":" - 0.0800"},"gmtOffSetMilliseconds":-14400000,"exchangeDataDelayedBy":30,"language":"en","regularMarketTime":{"raw":1588877995,"fmt":"2:59PM EDT"},"regularMarketChangePercent":{"raw":-11.251763,"fmt":" - 11.25 % "},"exchangeTimezoneShortName":"EDT","quoteType":"INDEX","regularMarketPrice":{"raw":0.631,"fmt":"0.6310"},"marketState":"REGULAR","market":"us24_market","priceHint":4,"tradeable":false,"exchange":"NYB","sourceInterval":30,"shortName":"10 - Yr Bond","region":"US","regularMarketPreviousClose":{"raw":0.711,"fmt":"0.7110"},"triggerable":false,"longName":"Treasury Yield 10 Years"},{"fullExchangeName":"Chicago Options","exchangeTimezoneName":"America / New_York","symbol":" ^ VIX","regularMarketChange":{"raw":-2.6799984,"fmt":" - 2.68"},"gmtOffSetMilliseconds":-14400000,"exchangeDataDelayedBy":20,"language":"en","regularMarketTime":{"raw":1588882486,"fmt":"4:14PM EDT"},"regularMarketChangePercent":{"raw":-7.854626,"fmt":" - 7.85 % "},"exchangeTimezoneShortName":"EDT","quoteType":"INDEX","regularMarketPrice":{"raw":31.44,"fmt":"31.44"},"marketState":"POSTPOST","market":"us_market","priceHint":2,"tradeable":false,"exchange":"WCB","sourceInterval":15,"shortName":"Vix","region":"US","regularMarketPreviousClose":{"raw":34.12,"fmt":"34.12"},"triggerable":true},{"fullExchangeName":"CCY","exchangeTimezoneName":"Europe / London","symbol":"GBPUSD = X","regularMarketChange":{"raw":0.0022070408,"fmt":"0.0022"},"gmtOffSetMilliseconds":3600000,"exchangeDataDelayedBy":0,"language":"en - US","regularMarketTime":{"raw":1588900689,"fmt":"2:18AM BST"},"regularMarketChangePercent":{"raw":0.17843062,"fmt":"0.1784 % "},"exchangeTimezoneShortName":"BST","quoteType":"CURRENCY","regularMarketPrice":{"raw":1.2391113,"fmt":"1.2391"},"marketState":"REGULAR","market":"ccy_market","priceHint":4,"tradeable":false,"exchange":"CCY","sourceInterval":15,"currency":"USD","shortName":"GBP / USD","region":"US","regularMarketPreviousClose":{"raw":1.2369043,"fmt":"1.2369"},"triggerable":true},{"fullExchangeName":"CCY","exchangeTimezoneName":"Europe / London","symbol":"JPY = X","regularMarketChange":{"raw":-9.994507E-4,"fmt":" - 0.0010"},"gmtOffSetMilliseconds":3600000,"exchangeDataDelayedBy":0,"language":"en - US","regularMarketTime":{"raw":1588900770,"fmt":"2:19AM BST"},"regularMarketChangePercent":{"raw":-9.404913E-4,"fmt":" - 0.0009 % "},"exchangeTimezoneShortName":"BST","quoteType":"CURRENCY","regularMarketPrice":{"raw":106.268,"fmt":"106.2680"},"marketState":"REGULAR","market":"ccy_market","quoteSourceName":"Delayed Quote","priceHint":4,"tradeable":false,"exchange":"CCY","sourceInterval":15,"currency":"JPY","shortName":"USD / JPY","region":"US","regularMarketPreviousClose":{"raw":106.269,"fmt":"106.2690"},"triggerable":true},{"fullExchangeName":"CCC","exchangeTimezoneName":"Europe / London","symbol":"BTC - USD","regularMarketChange":{"raw":1.5859375,"fmt":"1.59"},"gmtOffSetMilliseconds":3600000,"exchangeDataDelayedBy":0,"language":"en","regularMarketTime":{"raw":1588900646,"fmt":"2:17AM BST"},"regularMarketChangePercent":{"raw":0.01594701,"fmt":"0.02 % "},"exchangeTimezoneShortName":"BST","quoteType":"CRYPTOCURRENCY","regularMarketPrice":{"raw":9946.632,"fmt":"9,946.63"},"marketState":"REGULAR","market":"ccc_market","quoteSourceName":"CryptoCurrency","tradeable":false,"exchange":"CCC","sourceInterval":15,"region":"US","regularMarketPreviousClose":{"raw":9945.046,"fmt":"9,945.05"},"triggerable":true},{"fullExchangeName":"Nasdaq GIDS","exchangeTimezoneName":"America / New_York","symbol":" ^ CMC200","regularMarketChange":{"raw":11.39299,"fmt":"11.39"},"gmtOffSetMilliseconds":-14400000,"exchangeDataDelayedBy":0,"language":"en","regularMarketTime":{"raw":1588900710,"fmt":"9:18PM EDT"},"regularMarketChangePercent":{"raw":6.0865417,"fmt":"6.09 % "},"exchangeTimezoneShortName":"EDT","quoteType":"INDEX","regularMarketPrice":{"raw":198.5763,"fmt":"198.58"},"marketState":"POSTPOST","market":"us_market","priceHint":2,"tradeable":false,"exchange":"NIM","sourceInterval":15,"shortName":"CMC Crypto 200","region":"US","regularMarketPreviousClose":{"raw":187.1833,"fmt":"187.18"},"triggerable":false},{"fullExchangeName":"FTSE Index","exchangeTimezoneName":"Europe / London","symbol":" ^ FTSE","regularMarketChange":{"raw":82.220215,"fmt":"82.22"},"gmtOffSetMilliseconds":3600000,"exchangeDataDelayedBy":15,"language":"en","regularMarketTime":{"raw":1588865730,"fmt":"4:35PM BST"},"regularMarketChangePercent":{"raw":1.404571,"fmt":"1.40 % "},"exchangeTimezoneShortName":"BST","quoteType":"INDEX","regularMarketPrice":{"raw":5935.98,"fmt":"5,935.98"},"marketState":"CLOSED","market":"gb_market","priceHint":2,"tradeable":false,"exchange":"FGI","sourceInterval":15,"shortName":"FTSE 100","region":"US","regularMarketPreviousClose":{"raw":5853.76,"fmt":"5,853.76"},"triggerable":false},{"fullExchangeName":"Osaka","exchangeTimezoneName":"Asia / Tokyo","symbol":" ^ N225","regularMarketChange":{"raw":408.70117,"fmt":"408.70"},"gmtOffSetMilliseconds":32400000,"exchangeDataDelayedBy":0,"language":"en","regularMarketTime":{"raw":1588899570,"fmt":"9:59AM JST"},"regularMarketChangePercent":{"raw":2.0772858,"fmt":"2.08 % "},"exchangeTimezoneShortName":"JST","quoteType":"INDEX","regularMarketPrice":{"raw":20083.47,"fmt":"20,083.47"},"marketState":"REGULAR","market":"jp_market","priceHint":2,"tradeable":false,"exchange":"OSA","sourceInterval":20,"shortName":"Nikkei 225","region":"US","regularMarketPreviousClose":{"raw":19674.77,"fmt":"19,674.77"},"triggerable":false}],"error":null}}"

             var json = "{\"marketSummaryResponse\":{\"result\":[{\"fullExchangeName\":\"test\"}], \"error\":\"test\"}}";

             var content = JsonConvert.DeserializeObject<Test>(json);*/
            return null;
        }
    }
}