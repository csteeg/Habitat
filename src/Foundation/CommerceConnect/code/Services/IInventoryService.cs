using System.Collections.Generic;
using Sitecore.Commerce.Entities.Inventory;

namespace Valtech.Foundation.CommerceConnect.Services
{
    public interface IInventoryService
    {
        double GetStockBySku(string sku);

        IEnumerable<StockInformation> GetStockBySkus(IEnumerable<string> skus);
    }
}