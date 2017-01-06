using System.IO;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Fields;
using Sitecore.Foundation.Multisite;
using Sitecore.Foundation.SitecoreExtensions.Extensions;
using BoC.InversionOfControl;

namespace Sitecore.Feature.Language.Repositories
{
	//TODO: needs loads of refactoring, getting rid of statics at least
	public static class LanguageRepository
	{
		private static IEnumerable<Globalization.Language> GetAll()
		{
			if (Context.Database == null)
				return Enumerable.Empty<Globalization.Language>();
			return Context.Database.GetLanguages();
		}

		public static Globalization.Language GetActive()
		{
			return Context.Language;
		}

		public static IEnumerable<Globalization.Language> GetSupportedLanguages()
		{
			if (Context.Database == null)
				return Enumerable.Empty<Globalization.Language>();

			var languages = GetAll();
			var siteContext = new SiteContext();
		    var item = Context.Item ?? Context.Database.GetItem(Context.Site.RootPath + "/" + Context.Site.StartItem);
		    if (item == null)
		        return languages;

			var siteDefinition = siteContext.GetSiteDefinition(item);

			if (siteDefinition?.Item == null)
			{
				return languages;
			}

		    if (!siteDefinition.Item.IsDerived(Templates.LanguageSettings.ID))
		        return siteDefinition.Item.Languages;

			var supportedLanguagesField = new MultilistField(siteDefinition.Item.Fields[Templates.LanguageSettings.Fields.SupportedLanguages]);
			if (supportedLanguagesField.Count == 0)
			{
				return Enumerable.Empty<Globalization.Language>();
			}

			var supportedLanguages = supportedLanguagesField.GetItems();
			languages = languages.Where(language => supportedLanguages.Any(sl => sl.Name.Equals(language.Name)));
			return languages;
		}
	}
}