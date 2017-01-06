using System;
using BoC.Services;
using Sitecore.Foundation.Redirects.Models;

namespace Sitecore.Foundation.Redirects.Services
{
    public interface IRedirectService : IModelService<BaseRedirect>
    {
        BaseRedirect GetFor(Guid itemId);
        BaseRedirect GetFor(Uri uri);

        //void ProcessRedirect(String targetUrl, RedirectType redirectType = RedirectType.Found);
        void ProcessRedirect(BaseRedirect redirect, Uri originalUri);
    }
}
