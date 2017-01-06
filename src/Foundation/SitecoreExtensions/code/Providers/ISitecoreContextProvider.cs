using System.Globalization;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Security.Accounts;
using Sitecore.Sites;

namespace Sitecore.Foundation.SitecoreExtensions.Providers
{
	using Sitecore.Analytics.Tracking;
	using Sitecore.Mvc.Presentation;

	public interface ISitecoreContextProvider
	{
		bool PageModeIsNormal();

		bool PageModeIsPageEditor();

		DisplayMode GetDisplayMode();

		SiteContext GetCurrentContextSite();

		void SetCurrentContextSite(SiteContext site);

		Language GetCurrentContextLanguage();

		void SetCurrentContextLanguage(Language language);

		Database GetCurrentContextDatabase();

		void SetCurrentContextDatabase(Database database);

		Database GetCurrentContentDatabase();

		Item GetCurrentContextItem();

		void SetCurrentContextItem(Item item);

		User GetCurrentContextUser();

		Layouts.PageContext GetCurrentContextPage();

		RenderingContext GetCurrentRenderingContext();

		DeviceItem GetCurrentContextDevice();

		CultureInfo GetCurrentCulture();

		Contact GetCurrentContact();
	}
}
