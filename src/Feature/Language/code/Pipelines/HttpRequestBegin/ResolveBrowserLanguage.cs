using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Feature.Language.Repositories;
using Sitecore.Pipelines.HttpRequest;

namespace Sitecore.Feature.Language.Pipelines.HttpRequestBegin
{
	public class ResolveBrowserLanguage : HttpRequestProcessor
	{
		public override void Process(Sitecore.Pipelines.HttpRequest.HttpRequestArgs args)
		{
			Assert.ArgumentNotNull(args, "args");

			var lang = GetLanguageFromBrowser(args);
			if (lang != null)
			{
				Log.Info("Language resolved from browser: " + lang.Name, this);
				Sitecore.Context.Language = lang;
			}
		}
		/// <summary>
		/// Gets language from browser but only when language cookie doesn't exist yet
		/// Logic taken from https://markstiles.net/Blog/2013/04/01/browser-language-detection-in-sitecore.aspx
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public Globalization.Language GetLanguageFromBrowser(Sitecore.Pipelines.HttpRequest.HttpRequestArgs args)
		{
			if (IsBrowserLangDetectionAllowed(args))
			{
				// gets browser languages
				string[] userLangs = args.Context?.Request?.UserLanguages;
				if (userLangs == null || !userLangs.Any())
					return null;
				var supportedLanguages = LanguageRepository.GetSupportedLanguages();
				if (supportedLanguages == null || !supportedLanguages.Any())
					return null;

				foreach (string userLang in userLangs)
				{
					// gets first part where information about language is stored
					string langs = userLang.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
					foreach (var lang in langs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
					{
						if (string.IsNullOrEmpty(lang))
							continue;

						var foundLang = supportedLanguages.FirstOrDefault(l => lang.Equals(l.CultureInfo.Name, StringComparison.InvariantCultureIgnoreCase));

						if (foundLang != null || !lang.Contains("-"))
							return foundLang;

					    var langparts = lang.Split('-');
						return
							supportedLanguages.FirstOrDefault(l => (langparts[0] + "-" + langparts[0]).Equals(l.CultureInfo.Name, StringComparison.InvariantCultureIgnoreCase))
							??
							supportedLanguages.FirstOrDefault(l => (langparts[1] + "-" + langparts[1]).Equals(l.CultureInfo.Name, StringComparison.InvariantCultureIgnoreCase))
							??
							supportedLanguages.FirstOrDefault(l => langparts[0].Equals(l.CultureInfo.Name, StringComparison.InvariantCultureIgnoreCase));

					}
				}
			}
			return null;
		}

		/// <summary>
		/// Determinates if detection is allowed
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		protected bool IsBrowserLangDetectionAllowed(Sitecore.Pipelines.HttpRequest.HttpRequestArgs args)
		{
			// site must be defined
			return Sitecore.Context.Site != null
                &&!string.IsNullOrEmpty(args.Context.Request.UserAgent)
				// lang cookie is not already set
				&& !args.Context.Request.Cookies.AllKeys.Contains(Sitecore.Context.Site.GetCookieKey("lang"))
				// user agent is not robot
				&& !Sitecore.Analytics.Configuration.AnalyticsSettings.Robots.ExcludeList.ContainsUserAgent(args.Context.Request.UserAgent);
		}
	}
}
