using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Foundation.SitecoreExtensions.Providers;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Sites;
using System;
using System.Linq;

namespace Sitecore.Foundation.Multisite.Services
{
	public class CountryService : ICountryService
	{
		private readonly ISitecoreContextProvider _sitecoreContextProvider;

		public CountryService(ISitecoreContextProvider sitecoreContextProvider)
		{
			this._sitecoreContextProvider = sitecoreContextProvider;
		}

		public string GetCountryUrl(Item siteItem, Guid languageGuid)
		{
			var siteInfo = Sitecore.Configuration.Factory.GetSiteInfoList().FirstOrDefault(s => s.RootPath.Contains(siteItem.Paths.FullPath));

			if (siteInfo != null)
				using (new SiteContextSwitcher(Sitecore.Sites.SiteContext.GetSite(siteInfo.Name)))
				{
					using (new LanguageSwitcher(Language.Invariant))
					{
						var languageItem = _sitecoreContextProvider.GetCurrentContextDatabase().GetItem(new Data.ID(languageGuid));
						if (languageItem == null)
							return string.Empty;

						var language = LanguageManager.GetLanguage(languageItem.Name);
						if (language == null)
							return string.Empty;

						var urlOptions = new UrlOptions() { AlwaysIncludeServerUrl = true, Language = language, LanguageEmbedding = LanguageEmbedding.Always };
						var startItemPath = String.Format("{0}/{1}", siteInfo.RootPath, siteInfo.StartItem);
						return Sitecore.Links.LinkManager.GetItemUrl(Context.Database.GetItem(startItemPath), urlOptions).Replace(":443", "");
					}
				}

			return string.Empty;
		}
	}
}