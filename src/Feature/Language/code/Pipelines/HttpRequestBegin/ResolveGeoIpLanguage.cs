using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using Sitecore.Analytics;
using Sitecore.Analytics.Configuration;
using Sitecore.Analytics.Lookups;
using Sitecore.Analytics.Model;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Feature.Language.Repositories;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Web;

namespace Sitecore.Feature.Language.Pipelines.HttpRequestBegin
{
	public class ResolveGeoIpLanguage: HttpRequestProcessor
	{
	    public override void Process(HttpRequestArgs args)
	    {
	        var site = Context.Site;
	        if (site == null || !IsLangDetectionAllowed(args))
	        {
	            return;
	        }
	        var lang = GetLanguageFromIp(args);
			if (lang != null)
			{
				Log.Info("Language resolved from GeoIP: " + lang.Name, this);
				Sitecore.Context.Language = lang;
			}

		}

		public static Globalization.Language GetLanguageFromIp(HttpRequestArgs args)
		{
		    var hostAddress = GetHostAddress(args?.Context?.Request);
			var ip = GetIp(hostAddress);
	        if (ip == null)
	        {
	            return null;
	        }

	        var geoIpData = GeoIpManager.GetGeoIpData(new GeoIpOptions()
	        {
	            Ip = GeoIpManager.IpHashProvider.ResolveIpAddress(ip),
	            Id = GeoIpManager.IpHashProvider.ComputeGuid(ip),
	            MillisecondsTimeout = -1
	        });
	        if (geoIpData?.ResolveState != GeoIpResolveState.Resolved || geoIpData?.GeoIpData == null)
	            return null;
	        if (string.IsNullOrEmpty(geoIpData.GeoIpData.Country))
	            return null;

			Globalization.Language lang = null;
		    var supportedLanguages = LanguageRepository.GetSupportedLanguages().ToArray();
		    if (!string.IsNullOrEmpty(geoIpData.GeoIpData.Region))
	        {
				var regionized = geoIpData.GeoIpData.Country + "-"+ geoIpData.GeoIpData.Region;
	            lang = supportedLanguages.FirstOrDefault(l => regionized.Equals(l.CultureInfo.Name, StringComparison.InvariantCultureIgnoreCase));
	        }
		    return lang
				?? supportedLanguages.FirstOrDefault(l => (geoIpData.GeoIpData.Country + "-" + geoIpData.GeoIpData.Country).Equals(l.CultureInfo.Name, StringComparison.InvariantCultureIgnoreCase))
				?? supportedLanguages.FirstOrDefault(l => geoIpData.GeoIpData.Country.Equals(l.CultureInfo.Name, StringComparison.InvariantCultureIgnoreCase));
		}

	    private static string GetHostAddress(HttpRequest contextRequest)
	    {
	        if (contextRequest == null)
	            return null;
			var requestHttpHeader = AnalyticsSettings.ForwardedRequestHttpHeader;
	        if (string.IsNullOrEmpty(requestHttpHeader))
	            requestHttpHeader = "X-Forwarded-For";

			var header = contextRequest.Headers[requestHttpHeader];
			if (string.IsNullOrEmpty(header))
				return contextRequest.UserHostAddress;
			var ipFromHeader = GetIpFromHeader(header);

			return ipFromHeader ?? contextRequest.UserHostAddress;
	    }

		protected static string GetIpFromHeader(string header)
		{
			Assert.ArgumentNotNull((object)header, "header");
			string[] strArray = header.Split(',');
		    string str = strArray.FirstOrDefault();
			if (string.IsNullOrEmpty(str))
				return (string)null;
			return str.Trim();
		}

		private static byte[] GetIp(string userHostAddress)
		{
		    if (string.IsNullOrEmpty(userHostAddress))
		        return null;
			IPAddress address;
			if (IPAddress.TryParse(userHostAddress, out address))
				return address.GetAddressBytes();
			Log.Warn("Failed to parse ip address: " + userHostAddress, typeof(ResolveGeoIpLanguage));
		    return null;
		}

		protected static bool IsLangDetectionAllowed(Sitecore.Pipelines.HttpRequest.HttpRequestArgs args)
		{
			// site must be defined
			return Sitecore.Context.Site != null
				// lang cookie is not already set
				&& !args.Context.Request.Cookies.AllKeys.Contains(Sitecore.Context.Site.GetCookieKey("lang"))
				&& !string.IsNullOrEmpty(args.Context.Request.UserAgent)
				// user agent is not robot
				&& !Sitecore.Analytics.Configuration.AnalyticsSettings.Robots.ExcludeList.ContainsUserAgent(args.Context.Request.UserAgent);
		}

	}
}