using Microsoft.CodeAnalysis;
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
                decimal totalPrice = 0;
                if (result.Count > 0)
                {
                    foreach (var document in result)
                    {
                        totalQuantity = document.AddPurchase.Sum(x => x.Quantity);
                        totalPrice = totalQuantity * document.AddPurchase.Sum(x => x.PurchasePrice) + Int32.Parse(quantity)* Convert.ToDecimal(purchasePrice);


                        var purchase =
                            new Purchase
                            {
                                Id = ObjectId.GenerateNewId(),
                                PurchasePrice = Convert.ToDecimal(purchasePrice),
                                Quantity = Int32.Parse(quantity)
                            };
                        var update = Builders<Entity>.Update
                            .AddToSet<Purchase>(e => e.AddPurchase, purchase)
                             .Set(x => x.TotalQuantity, totalQuantity+ Int32.Parse(quantity))
                             .Set(x => x.TotalPrice, totalPrice);
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

        public async Task<List<Entity>> GetStockInformationAsync(string symbol)
        {
            try
            {
                var collection = database.GetCollection<Entity>("userprofile");
                var filter = Builders<Entity>.Filter.Eq("Symbol", symbol);

                return await collection.Find(filter).ToListAsync();

            }
            catch (Exception e)
            {
                return null;
            }


        }

        public async Task<bool> DeleteStockPurchaseAsync(string symbol, ObjectId id)
        {
            try
            {
                var collection = database.GetCollection<Entity>("userprofile");

                var filter = Builders<Entity>.Filter.Eq("Symbol", symbol);
                var update = Builders<Entity>.Update.PullFilter(p => p.AddPurchase,
                       Builders<Purchase>.Filter.Eq(per => per.Id, id));

                var result = collection
                    .FindOneAndUpdateAsync(filter, update).Result;

                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> InsertToUserProfileAsync(Entity entity)
        {
            try
            {
                var collection = database.GetCollection<Entity>("userprofile");
                await collection.InsertOneAsync(entity);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }            
        }

        public async Task<bool> UpsertToUserProfileAsync(string symbol, decimal? price )
        {
            try
            {
                var collection = database.GetCollection<Entity>("userprofile");

                var filter = Builders<Entity>.Filter.Eq("Symbol", symbol);
                var update = Builders<Entity>.Update.Set("Price", price);
                await collection.UpdateOneAsync(filter, update);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }  

}
