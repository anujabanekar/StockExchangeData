﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using RestSharp;
using StockExchangeData.Models.Mongo;
using StockExchangeData.Models.Profile;
using StockExchangeData.Services.Contract;

namespace StockExchangeData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class StockDataController : ControllerBase
    {
        private IMemoryCache _cache;
        private readonly HttpClient client = new HttpClient();
        private readonly ILogger<StockDataController> _logger;
        private readonly IMarketSummaryService _marketSummaryService;
        private readonly IProfileService _profileService;
        private readonly IMongoClientService _mongoClientService;

        public StockDataController(ILogger<StockDataController> logger,
            IMemoryCache memoryCache,
             IMarketSummaryService marketSummaryService,
             IProfileService profileService,
             IMongoClientService mongoClientService)
        {
            _logger = logger;
            _cache = memoryCache;
            _marketSummaryService = marketSummaryService;
            _profileService = profileService;
            _mongoClientService = mongoClientService;
        }

        [HttpGet]
        public async Task<IEnumerable<Result>> GetAsync()
        {
            HttpResponseMessage marketSummary;
            if (!_cache.TryGetValue("MarketSummary", out marketSummary))
            {

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(3000));

          
                marketSummary = await _marketSummaryService.GetMarkeySummaryAsync();
                
                _cache.Set("MarketSummary", marketSummary.Content, cacheEntryOptions);
            }
            //var response = await client.GetAsync("https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/get-summary?region=US&lang=en");
            if (marketSummary.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = marketSummary.Content.ReadAsAsync<MarketSummary>();
                return content.Result?.MarketSummaryResponse?.Result;
            }
            return null;
        }

        [HttpGet]
        [Route("AddToDashBoard/{value}")]
        public async Task<bool> AddToDashBoard(string value)
        {
            string Key = Environment.GetEnvironmentVariable("YahooApiKey", EnvironmentVariableTarget.Machine);
            string Host = Environment.GetEnvironmentVariable("YahooApiHost", EnvironmentVariableTarget.Machine);


            HttpResponseMessage profileData;
            if (!_cache.TryGetValue(value, out profileData))
            {

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(3000));

                client.DefaultRequestHeaders.Add("x-rapidapi-host", Host);
                client.DefaultRequestHeaders.Add("x-rapidapi-key", Key);

                const string connectionString = "mongodb://localhost:27017";
                profileData = await _profileService.GetProfileData(value);

                _cache.Set(value, profileData.Content, cacheEntryOptions);

                if (profileData.IsSuccessStatusCode)
                {
                    var content = profileData.Content.ReadAsAsync<ProfileData>();

                    if (content != null)
                    {
                        var isPresent = await _mongoClientService.GetStockInformationAsync(value);
                        if (isPresent.Count == 0)
                        {
                            var entity = new Entity
                            {
                                Symbol = content.Result.Symbol,
                                Price = content.Result.Price?.RegularMarketDayHigh?.Raw,
                                AddPurchase = new List<Purchase>(),
                                SoldStock = new List<Purchase>()
                            };
                            
                            return await _mongoClientService.InsertToUserProfileAsync(entity);
                        }
                        else
                        {
                            await _mongoClientService.UpsertToUserProfileAsync(value, content.Result.Price?.RegularMarketDayHigh?.Raw);
                        }
                        
                    }
                }
                        
            }
            
            return false;
        }

        /*TODO Get Live Call*/
        [HttpGet]
        [Route("GetProfileData")]
        public async Task<List<Entity>> GetProfileData()
        {
            const string connectionString = "mongodb://localhost:27017";
            // Create a MongoClient object by using the connection string
            var client = new MongoClient(connectionString);

            //Use the MongoClient to access the server
            var database = client.GetDatabase("stockexchangedata");

            //get mongodb collection
            var collection = database.GetCollection<Entity>("userprofile");
            return await collection.Find(new BsonDocument()).ToListAsync();
        }

        [HttpGet]
        [Route("GetPortfolioValue/{symbol}")]
        public async Task<List<Entity>> GetStockInformationAsync(string symbol)
        {
            var result = await _mongoClientService.GetStockInformationAsync(symbol);
            return result;
        }

        [HttpGet]
        [Route("DeleteStockPurchase/{symbol}/{id}")]
        public async Task<bool> DeleteStockPurchase(string symbol, string id)
        {
            //return false;
            return await _mongoClientService.DeleteStockPurchaseAsync(symbol, new ObjectId(id));
        }


        [HttpGet]
        [Route("SellPurchaseQuantity/{symbol}/{quantity}/{purchasePrice}")]
        public async Task<bool> SellPurchaseQuantity(string symbol, string quantity, string purchasePrice)
        {
            try
            {
                return await _mongoClientService.UpdateUserProfilePurchaseData(symbol, quantity, purchasePrice, "sellToPurchase");
            }
            catch(Exception e)
            {
                _logger.LogError($"Cannot update quantity for given symbol {symbol}");
                
            }
            

            return false;
        }

        [HttpGet]
        [Route("AddPurchaseQuantity/{symbol}/{quantity}/{purchasePrice}")]
        public async Task<bool> AddPurchaseQuantity(string symbol, string quantity, string purchasePrice)
        {
            try
            {
                return await _mongoClientService.UpdateUserProfilePurchaseData(symbol, quantity, purchasePrice, "addToPurchase");
            }
            catch (Exception e)
            {
                _logger.LogError($"Cannot update quantity for given symbol {symbol}");

            }


            return false;
        }
    }
}