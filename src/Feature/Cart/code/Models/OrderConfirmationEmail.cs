using BoC.Persistence.SitecoreGlass.Models;

namespace Sitecore.Feature.Cart.Models
{
	public class OrderConfirmationEmail : SitecoreItem
	{
		public virtual string Header { get; set; }
		public virtual string Salutation { get; set; }
		public virtual string Introduction { get; set; }
		public virtual string OrderReceived { get; set; }
		public virtual string OrderWillBeSent { get; set; }
		public virtual string OrderOverview { get; set; }
		public virtual string OrderContains { get; set; }
		public virtual string Pieces { get; set; }
		public virtual string AccountingDetails { get; set; }
		public virtual string ShippingDetails { get; set; }
		public virtual string SenderDetails { get; set; }
		public virtual string AdditionalContent { get; set; }
		public virtual string Copyright { get; set; }
	}
}