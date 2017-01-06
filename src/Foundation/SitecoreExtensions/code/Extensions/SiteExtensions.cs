using Sitecore.Web;

namespace Sitecore.Foundation.SitecoreExtensions.Extensions
{
	using System;
	using Sitecore;
	using Sitecore.Data;
	using Sitecore.Data.Items;
	using Sitecore.ExperienceEditor.Utils;
	using Sitecore.ExperienceExplorer.Business.Managers;
	using Sitecore.Sites;

	public static class SiteExtensions
	{
		public static Item GetContextItem(this SiteContext site, ID derivedFromTemplateID)
		{
			if (site == null)
				throw new ArgumentNullException(nameof(site));

			var startItem = site.GetStartItem();
			return startItem?.GetAncestorOrSelfOfTemplate(derivedFromTemplateID);
		}

		public static Item GetRootItem(this SiteContext site)
		{
			if (site == null)
				throw new ArgumentNullException(nameof(site));

			return site.Database.GetItem(Context.Site.RootPath);
		}

		public static Item GetStartItem(this SiteContext site)
		{
			if (site == null)
				throw new ArgumentNullException(nameof(site));

			return site.Database.GetItem(Context.Site.StartPath);
		}

		public static bool IsContentOriginated(this SiteContext site)
		{
			if (site == null || site.RootPath == null)
				return false;
			return site.RootPath.StartsWith(Constants.ContentPath.ToLower(), StringComparison.InvariantCultureIgnoreCase) &&
					site.RootPath.Length > Constants.ContentPath.Length;
		}


	}
}