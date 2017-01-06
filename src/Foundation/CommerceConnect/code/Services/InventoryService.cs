using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Entities.Inventory;
using Sitecore.Commerce.Services.Inventory;

namespace Valtech.Foundation.CommerceConnect.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly InventoryServiceProvider _serviceProvider;

        public InventoryService(InventoryServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public double GetStockBySku(string sku)
        {
            var request = new GetStockInformationRequest(Sitecore.Context.GetSiteName(), new List<InventoryProduct>
            {
                new InventoryProduct
                {
                    ProductId = sku
                }
            },
            StockDetailsLevel.Count);

            var result = _serviceProvider.GetStockInformation(request);

            return result.StockInformation.FirstOrDefault(p => p.Product.ProductId.Equals(sku, StringComparison.InvariantCultureIgnoreCase)).Count;
        }

        public IEnumerable<StockInformation> GetStockBySkus(IEnumerable<string> skus)
        {
            var request = new GetStockInformationRequest(Sitecore.Context.GetSiteName(),
                skus.Select(s => new InventoryProduct {ProductId = s}),
                StockDetailsLevel.Count);

            var result = _serviceProvider.GetStockInformation(request);

            return result.StockInformation;
        }
    }
}