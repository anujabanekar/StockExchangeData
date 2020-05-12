using MongoDB.Bson;
using MongoDB.Driver;
using StockExchangeData.Models.Mongo;
using StockExchangeData.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockExchangeData.Services.Implementation
{
    public class MongoClientService : IMongoClientService
    {

        const string connectionString = "mongodb://localhost:27017";

        public MongoClient mongoClient;

        public IMongoDatabase database;
        public MongoClientService()
        {
            mongoClient = new MongoClient(connectionString);
            database = mongoClient.GetDatabase("stockexchangedata");
        }

        public async Task<bool> UpdateUserProfilePurchaseData(string symbol, string quantity, string purchasePrice)
        {
            try
            {
                var collection = database.GetCollection<Entity>("userprofile");
                var filter = Builders<Entity>.Filter.Eq("Symbol", symbol);

                var result = await collection.Find(filter).ToListAsync();

                var totalQuantity = 0;
                if (result.Count > 0)
                {
                    foreach (var document in result)
                    {
                        totalQuantity = document.TotalQuantity + Int32.Parse(quantity);


                        var purchase =
                            new Purchase
                            {
                                PurchasePrice = Convert.ToDecimal(purchasePrice),
                                Quantity = Int32.Parse(quantity)
                            };
                        var update = Builders<Entity>.Update
                            .AddToSet<Purchase>(e => e.AddPurchase, purchase)
                             .Set(x => x.TotalQuantity, totalQuantity);
                        await collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;

            }


        }
    }
}
