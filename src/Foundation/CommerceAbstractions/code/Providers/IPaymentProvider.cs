namespace Valtech.Foundation.CommerceAbstractions.Providers
{
	public interface IPaymentProvider
	{
		void CreatePayment(string cartId, int paymentMethodId, bool requestPayment = false);
	}
}