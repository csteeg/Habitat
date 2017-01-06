namespace Valtech.Foundation.CommerceAbstractions.Services
{
    public interface IPricingService
    {
        string GetBySku(string sku);
		string GetBySku(string sku, string type);
	}
}
