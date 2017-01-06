namespace Valtech.Foundation.CommerceAbstractions.Services
{
	public interface IShopContextService
	{
		string GetCurrentUserId();
		string GetCurrentCartId();
		string GetCurrentShopName();
		void EnsureCartCookie(string resultExternalId);
		void DeleteCartCookie();
	}
}