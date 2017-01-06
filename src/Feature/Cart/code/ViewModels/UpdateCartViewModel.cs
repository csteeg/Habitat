namespace Sitecore.Feature.Cart.ViewModels
{
    /// <summary>
    /// View model for a Basket LineItem.
    /// </summary>
    public class UpdateCartViewModel
    {
        /// <summary>
        /// Gets or sets the Product Id of the current LineItem.
        /// </summary>
        public string CartLineId { get; set; }

        /// <summary>
        /// Gets or sets the Quantity of the current LineItem.
        /// </summary>
        public uint Quantity { get; set; }
    }
}
