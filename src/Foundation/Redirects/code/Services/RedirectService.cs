using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using BoC.DataContext;
using BoC.EventAggregator;
using BoC.Logging;
using BoC.Persistence;
using BoC.Persistence.SitecoreGlass.Models;
using BoC.Services;
using BoC.Validation;
using Sitecore.Collections;
using Sitecore.Common;
using Sitecore.Configuration;
using Sitecore.Events;
using Sitecore.Foundation.Redirects.Enums;
using Sitecore.Foundation.Redirects.Models;
using Sitecore.Foundation.SitecoreExtensions.Providers;

#if SC70
using Sitecore.ContentSearch.Utilities;
#else

#endif

namespace Sitecore.Foundation.Redirects.Services
{
    public class RedirectService : BaseModelService<BaseRedirect>, IRedirectService
    {
        private readonly ISitecoreContextProvider _sitecoreContextProvider;
        private readonly IHttpContextBaseProvider _httpContextBaseProvider;
        private readonly ILogger _logger;
        public static object _allRedirectsLock = new object();

        private static IDictionary<string, IEnumerable<BaseRedirect>> _allRedirects;

        /// <summary>
        /// Redirects can be truned off in Sitecore setting Redirects.Enabled (Redirects.config)
        /// </summary>
        public bool Enabled
        {
            get
            {
                var enabled = Settings.GetBoolSetting("Redirects.Enabled", false);

                if (!enabled)
                    this._logger.Debug("Redirect are disabled");

                return enabled;
            }
        }

        public IEnumerable<BaseRedirect> AllRedirects
        {
            get
            {
                var currentSite = this._sitecoreContextProvider.GetCurrentContextSite().RootPath;

                return _allRedirects != null && _allRedirects.ContainsKey(currentSite)
                    ? _allRedirects[currentSite]
                    : this.GetCurrentSiteRedirects();
            }
        }

        public RedirectService(IModelValidator validator, IEventAggregator eventAggregator,
                               IRepository<BaseRedirect> repository, ISitecoreContextProvider sitecoreContextProvider,
                               IHttpContextBaseProvider httpContextBaseProvider, ILogger logger)
            : base(validator, eventAggregator, repository)
        {
            this._logger = logger;
            this._httpContextBaseProvider = httpContextBaseProvider;
            this._sitecoreContextProvider = sitecoreContextProvider;

            Event.Subscribe("publish:end", this.Clear);
            Event.Subscribe("publish:end:remote", this.Clear);
        }

        private IEnumerable<BaseRedirect> GetCurrentSiteRedirects()
        {
            if (!this.Enabled) return Enumerable.Empty<BaseRedirect>();

            try
            {
                var currentsite = this._sitecoreContextProvider.GetCurrentContextSite()?.RootPath;
                if (currentsite == null)
                    return Enumerable.Empty<BaseRedirect>();

                if (_allRedirects != null && _allRedirects.ContainsKey(currentsite))
                    return _allRedirects[currentsite];

                using (DataContext.BeginDataContext())
                {
                    lock (_allRedirectsLock)
                    {
                        if (_allRedirects != null && _allRedirects.ContainsKey(currentsite))
                            return _allRedirects[currentsite];

                        var currentSiteItem = this._sitecoreContextProvider.GetCurrentContextDatabase().GetItem(currentsite);
                        var redirects = currentSiteItem != null
                            ? this.Find(redirect => ((ISearchable)redirect).ParentIds.Contains(currentSiteItem.ID.Guid)).ToList()
                            : new List<BaseRedirect>();

                        if (_allRedirects == null)
                            _allRedirects = new Dictionary<string, IEnumerable<BaseRedirect>>();

                        if (!_allRedirects.ContainsKey((currentsite)))
                            _allRedirects.Add(currentsite, redirects);
                        else
                            _allRedirects[currentsite] = redirects;

                        return redirects;
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.ErrorFormat("Error in GetCurrentRedirects: {0} - {1}", ex.Message, ex.StackTrace);
                return new List<BaseRedirect>();
            }

        }

        public void Clear(object sender, EventArgs eventArgs)
        {
            if (_allRedirects != null)
            {
                this._logger.Info("Clearing redirects");
                _allRedirects = null;
            }
        }

        public BaseRedirect GetFor(Guid itemId)
        {
            if (this.AllRedirects == null || !this.AllRedirects.Any())
                return null;

            try
            {
                return this.AllRedirects.OfType<ItemToLinkRedirect>().FirstOrDefault(redirect => redirect.Id.Equals(itemId));
            }
            catch (Exception ex)
            {
                this._logger.ErrorFormat("Error in GetFor(Guid) redirect: {0} - {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        public BaseRedirect GetFor(Uri uri)
        {
            try
            {
                if (uri.AbsolutePath.Contains("/api/") || uri.AbsolutePath.Contains("/layouts/system/"))
                    return null;

                if (this.AllRedirects == null || !this.AllRedirects.Any())
                    return null;

                var linkRedirects = this.AllRedirects.OfType<LinkToLinkRedirect>().ToList();

                if (linkRedirects.Any())
                {
                    foreach (var redirect in linkRedirects.Where(redirect => redirect != null && redirect.SourceLink != null))
                    {
                        this._logger.Debug("Trying redirect: " + redirect.SourceLink.Url);

                        var sourceUri = new Uri(redirect.SourceLink.BuildUrl(new SafeDictionary<string>()), UriKind.RelativeOrAbsolute);

                        if (sourceUri.IsAbsoluteUri)
                        {
                            if (uri.Host.Equals(sourceUri.Host) && uri.AbsolutePath.Equals(sourceUri.AbsolutePath))
                            {
                                this._logger.Info("Found redirect: " + redirect.SourceLink);
                                return redirect;
                            }
                        }
                        else if (HttpUtility.UrlDecode(uri.PathAndQuery).Equals(sourceUri.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            this._logger.Info("Found redirect: " + redirect.SourceLink);
                            return redirect;
                        }
                    }
                }

                var regexRedirects = this.AllRedirects.OfType<RegexToLinkRedirect>().ToList();

                if (regexRedirects.Any())
                {
                    return this.AllRedirects.OfType<RegexToLinkRedirect>()
                        .FirstOrDefault(redirect => Regex.IsMatch(uri.ToString(), redirect.SourceRegex));
                }

                return null;
            }
            catch (Exception ex)
            {
                this._logger.ErrorFormat("Error in GetFor(Uri) redirect: {0} - {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        public void ProcessRedirect(BaseRedirect redirect, Uri originalUri)
        {
            if (redirect == null)
            {
                this._logger.InfoFormat("Got null redirect to process!");
                return;
            }
            this._logger.InfoFormat("Processing redirect ({0}) for: {1}", redirect.Id, originalUri.ToString());
            var response = this._httpContextBaseProvider.GetCurrentHttpContext().Response;

            if (redirect.TargetLink == null) return;

            switch (redirect.RedirectType)
            {
                case RedirectType.Permanent:
                case RedirectType.Found:
                {
                    if (string.IsNullOrEmpty(redirect.TargetLink.Url))
                    {
                        this._logger.WarnFormat("No targetUrl found for redirect: {0}", redirect.Id);
                        return;
                    }
                    response.Status = GetStatusName(redirect.RedirectType);
                    response.StatusCode = (int)redirect.RedirectType;
                    response.AddHeader("Location", redirect.TargetLink.Url + HttpUtility.UrlDecode(redirect.TargetLink.Query));
                    response.End();
                    break;
                }
                case RedirectType.Alias:
                {
                    var target = this._sitecoreContextProvider.GetCurrentContentDatabase().GetItem(redirect.TargetLink.TargetId.ToID());
                    if (target == null)
                    {
                        this._logger.WarnFormat("No targetId set or item not found for Alias redirect: {0}", redirect.SitecorePath);
                        return;
                    }
                    this._sitecoreContextProvider.SetCurrentContextItem(target);
                    break;
                }
            }
        }

        private string GetStatusName(RedirectType type)
        {
            var attribute = type.GetType().GetField(type.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
            return attribute != null ? attribute.Description : type.ToString();
        }
    }
}
