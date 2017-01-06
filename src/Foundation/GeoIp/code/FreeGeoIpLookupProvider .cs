using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Sitecore.Analytics.Lookups;
using Sitecore.Analytics.Model;
using Sitecore.Diagnostics;

namespace Sitecore.Foundation.FreeGeoIp
{
    public class FreeGeoIpLookupProvider : LookupProviderBase
    {

        [UsedImplicitly]
        public override WhoIsInformation GetInformationByIp(string ipAddress)
        {
            var whois = new WhoIsInformation();
            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                try
                {
                    if (!isPrivate(ipAddress))
                    {
                        var geoLocationServiceUrl = new Uri("https://freegeoip.net/json/" + ipAddress);
                        var client = new WebClient();
                        var serviceResponse = client.DownloadString(geoLocationServiceUrl);
                        dynamic serviceData = JsonConvert.DeserializeObject(serviceResponse);
                        if (Log.IsDebugEnabled) { Log.Debug(string.Format("Ip geo lookup for {0}: {1}", ipAddress, serviceResponse)); }
                        whois.City = serviceData.city ?? string.Empty;
                        whois.MetroCode = serviceData.metro_code.ToString() ?? string.Empty;
                        whois.Region = serviceData.region_name ?? string.Empty;
                        whois.PostalCode = serviceData.zip_code ?? string.Empty;
                        whois.Country = serviceData.country_code ?? serviceData.country_name ?? string.Empty;
                        whois.Latitude = serviceData.latitude ?? 0;
                        whois.Longitude = serviceData.longitude ?? 0;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Sitecore.Foundation.FreeGeoIp.FreeGeoIpLookupProvider failed", ex, this);
                }
            }
            return whois;
        }

        private bool isPrivate(string ipAddress)
        {
            int[] ipParts = ipAddress.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(s => int.Parse(s)).ToArray();
            // in private ip range
            if (ipParts[0] == 10 ||
                (ipParts[0] == 192 && ipParts[1] == 168) ||
                (ipParts[0] == 172 && (ipParts[1] >= 16 && ipParts[1] <= 31)))
            {
                return true;
            }

            // IP Address is probably public.
            // This doesn't catch some VPN ranges like OpenVPN and Hamachi.
            return false;
        }
    }
}
