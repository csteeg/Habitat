using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BoC.Persistence.SitecoreGlass.Models;
using Glass.Mapper.Sc.Fields;

namespace Sitecore.Feature.Cart.Models
{
    public class CartOverview: SitecoreItem
    {
        public virtual string CartIsEmptyText { get; set; }
        public virtual Link ContinueShoppingLink { get; set; }
        public virtual Link CheckoutLink { get; set; }
        public virtual string AllArticlesInStockText { get; set; }
        public virtual string NotAllArticlesInStockText { get; set; }
    }
}