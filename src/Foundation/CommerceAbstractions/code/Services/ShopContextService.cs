using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Foundation.SitecoreExtensions.Providers;
using Sitecore.Foundation.SitecoreExtensions.Services;

namespace Valtech.Foundation.CommerceAbstractions.Services
{
	public class ShopContextService : IShopContextService
	{
		public const string CookieName = "cartid";

		private readonly IHttpContextBaseProvider _contextProvider;
		private readonly ISitecoreContextProvider _sitecoreContextProvider;
	    private readonly ITrackerService _trackerService;

	    public ShopContextService(IHttpContextBaseProvider contextProvider, ISitecoreContextProvider sitecoreContextProvider, ITrackerService trackerService)
		{
			this._contextProvider = contextProvider;
			this._sitecoreContextProvider = sitecoreContextProvider;
		    this._trackerService = trackerService;
		}

		public string GetCurrentUserId()
		{
			var identity = this._contextProvider.GetCurrentHttpContext()?.User?.Identity;
		    if (identity == null || !identity.IsAuthenticated)
		    {
		        var contact = this._trackerService.CurrentContact;
		        if (contact != null)
		        {
		            return contact.Identifiers?.Identifier ?? contact.ContactId.ToString();
		        }
		    }
		    return identity?.Name;
		}

		public string GetCurrentCartId()
		{
			return this._contextProvider.GetCurrentHttpContext()?.Request?.Cookies?[CookieName]?.Value;
		}

		public string GetCurrentShopName()
		{
			return this._sitecoreContextProvider.GetCurrentContextSite()?.Properties["shopName"] ?? this._sitecoreContextProvider.GetCurrentContextSite()?.Name;
		}

		public void EnsureCartCookie(string cartId)
		{
			var context = this._contextProvider.GetCurrentHttpContext();
			if (context == null)
				return;
			if (string.IsNullOrEmpty(cartId))
			{
				var cookie = context.Response.Cookies[CookieName];
				if (cookie != null)
					cookie.Expires = DateTime.Now.AddYears(-1);
				return;
			}
			context.Response.Cookies.Remove(CookieName);
			context.Response.Cookies.Add(new HttpCookie(CookieName, cartId) { Expires = DateTime.Now.AddDays(1) });
		}

		public void DeleteCartCookie()
		{
			var context = this._contextProvider.GetCurrentHttpContext();
			if (context == null)
				return;

			var cookie = context.Response.Cookies[CookieName];
			if (cookie != null)
				cookie.Expires = DateTime.Now.AddYears(-1);
		}
	}
}