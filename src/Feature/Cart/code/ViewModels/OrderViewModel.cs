using Sitecore.Commerce.Entities.Payments;
using Sitecore.Commerce.Entities.Shipping;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Sitecore.Feature.Cart.ViewModels
{
    public class OrderViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string ZipCode { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string DeliveryAddress { get; set; }
        public string DeliveryZipCode { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryCountry { get; set; }

        public SelectList CountryOptions { get; set; }
        public List<PaymentOption> PaymentOptions { get; set; }
    }

    public class OrderPostViewModel
    {
        [Required]
        public string PaymentOption { get; set; }
    }

}