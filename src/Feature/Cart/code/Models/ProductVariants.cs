using BoC.Persistence.SitecoreGlass.Models;
using System;
using Valtech.Foundation.CommerceAbstractions.Models;

namespace Sitecore.Feature.Cart.Models
{
    public class ProductVariants : SitecoreItem, IProductDetailSource
    {
        public virtual ProductBase Product { get; set; }
        public virtual Guid TextField { get; set; }
    }
}