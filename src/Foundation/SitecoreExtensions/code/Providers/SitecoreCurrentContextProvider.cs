using System.Collections.Specialized;
using System.Globalization;
using BoC.InversionOfControl;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Mvc.Presentation;
using Sitecore.Security.Accounts;
using Sitecore.Sites;
using Sitecore.Web;
using Sitecore.Analytics.Tracking;

namespace Sitecore.Foundation.SitecoreExtensions.Providers
{

	public class SitecoreCurrentContextProvider : ISitecoreContextProvider
	{
		public bool PageModeIsNormal()
		{
			return this.GetDisplayMode() == DisplayMode.Normal;
		}

		public bool PageModeIsPageEditor()
		{
#pragma warning disable 618
		    return Context.PageMode.IsExperienceEditorEditing;
#pragma warning restore 618
		}

		public DisplayMode GetDisplayMode()
		{
			return Context.Site.DisplayMode;
		}

		public SiteContext GetCurrentContextSite()
		{
			return Context.Site;
		}

		public void SetCurrentContextSite(SiteContext site)
		{
			Context.Site = site;
		}

		public Language GetCurrentContextLanguage()
		{
			return Context.Language;
		}

		public void SetCurrentContextLanguage(Language language)
		{
			Context.SetLanguage(language, true);
		}

		public Database GetCurrentContextDatabase()
		{
			return Context.Database;
		}

		public void SetCurrentContextDatabase(Database database)
		{
			Context.Database = database;
		}

		public Database GetCurrentContentDatabase()
		{
			return Context.ContentDatabase ?? this.GetCurrentContextDatabase();
		}

		public Item GetCurrentContextItem()
		{
			return Context.Item;
		}

		public void SetCurrentContextItem(Item item)
		{
			Context.Item = item;
		}

		public User GetCurrentContextUser()
		{
			return Context.User;
		}

		public Layouts.PageContext GetCurrentContextPage()
		{
			return Context.Page;
		}

		public RenderingContext GetCurrentRenderingContext()
		{
			return RenderingContext.CurrentOrNull;
		}

		public DeviceItem GetCurrentContextDevice()
		{
			return Context.Device;
		}

		public CultureInfo GetCurrentCulture()
		{
			return Context.Culture;
		}

		public PageDefinition GetPageDefinition()
		{
			return global::Sitecore.Mvc.Presentation.PageContext.Current.PageDefinition;
		}

		public Rendering GetRendering()
		{
			return RenderingContext.Current.Rendering;
		}

		public NameValueCollection GetRenderingParameters()
		{
			var currentRendering = this.GetRendering();
			if (currentRendering == null || string.IsNullOrEmpty(currentRendering["Parameters"])) return new NameValueCollection();

			return WebUtil.ParseUrlParameters(currentRendering["Parameters"]);
		}

		public Contact GetCurrentContact()
		{
			return Analytics.Tracker.Current.Contact;
		}
	}
}