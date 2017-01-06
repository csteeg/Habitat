using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Valtech.Foundation.CommerceConnect.Repositories
{
    using BoC.Logging;
    using BoC.Persistence.SitecoreGlass;
    using Valtech.Foundation.CommerceAbstractions.Models;
    using Valtech.Foundation.CommerceAbstractions.Repositories;

    [Obsolete("Please use a specific implementation of ICatalogRepository, this version is not implemented yet")]
    public class CatalogRepository : SitecoreRepository<ProductBase>, ICatalogRepository
    {
        public CatalogRepository(IDatabaseProvider dbProvider, ISitecoreServiceProvider sitecoreServiceProvider, IProviderSearchContextProvider searchContextProvider, ILogger logger) : base(dbProvider, sitecoreServiceProvider, searchContextProvider, logger)
        {
        }

        public ProductBase GetBySku(string sku)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductBase> SearchProducts(string luceneQuery, params CategoryBase[] categories)
        {
            throw new NotImplementedException();
        }
    }
}