using Sitecore.Data.Items;
using System;

namespace Sitecore.Foundation.Multisite.Services
{
	public interface ICountryService
	{
		string GetCountryUrl(Item siteItem, Guid languageGuid);
	}
}