﻿using MongoDB.Bson;
using StockExchangeData.Models.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockExchangeData.Services.Contract
{
    public interface IMongoClientService
    {
        Task<bool> UpdateUserProfilePurchaseData(string symbol, string quantity, string purchasePrice);

        Task<List<Entity>> GetStockInformationAsync(string symbol);
        Task<bool> DeleteStockPurchaseAsync(string symbol, ObjectId id);
    }
}
