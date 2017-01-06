using Sitecore.Data.Items;

namespace Sitecore.Foundation.Multisite
{
	public interface ISiteContext
	{
		SiteDefinition GetSiteDefinition([NotNull]Item item);
	}
}