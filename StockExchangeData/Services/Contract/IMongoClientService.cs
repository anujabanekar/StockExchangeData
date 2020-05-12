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
    }
}
