using System.Collections.Generic;
using BoC.Persistence;
using Valtech.Foundation.CommerceAbstractions.Models;

namespace Valtech.Foundation.CommerceAbstractions.Repositories
{
	public interface ICatalogRepository: IRepository<ProductBase>
	{
		IEnumerable<ProductBase> SearchProducts(string luceneQuery, params CategoryBase[] categories);
		ProductBase GetBySku(string sku);
	}
}