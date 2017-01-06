using BoC.InversionOfControl;
using Glass.Mapper.Configuration;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Fluent;
using Sitecore.Feature.Cart.Models;

namespace Sitecore.Feature.Cart
{
    public class ModelConfigurationLoader : IContainerInitializer
    {
        public static bool Disabled { get; set; }
        private readonly IDependencyResolver _resolver;

        public ModelConfigurationLoader(IDependencyResolver resolver)
        {
            this._resolver = resolver;
        }

        public void Execute()
        {
            if (Disabled)
                return;
            var loader = new SitecoreFluentConfigurationLoader();

            //datasources
            AddProductVariants(loader);
            AddCartOverview(loader);
			AddOrderConfirmationEmail(loader);


			_resolver.RegisterInstance<IConfigurationLoader>(loader);
		}

		private static void AddCartOverview(SitecoreFluentConfigurationLoader loader)
		{
			var config = loader.Add<CartOverview>()
				.CodeFirst()
				.TemplateName("_CartOverview")
				.TemplateId("{e87b838e-4c01-4a44-a56a-54e0f5a3a8bc}")
				.AutoMap();

			config.Field(p => p.CartIsEmptyText)
				.IsCodeFirst()
				.FieldId("{f87000c5-80e8-4836-94fb-423e9d685cf9}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.RichText)
				.FieldSortOrder(10);

			config.Field(p => p.AllArticlesInStockText)
				.IsCodeFirst()
				.FieldId("{159fc6f9-9b98-414c-ba72-e755372038e1}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.RichText)
				.FieldSortOrder(14);

			config.Field(p => p.NotAllArticlesInStockText)
				.IsCodeFirst()
				.FieldId("{0a95bcc6-a97e-4712-9e07-ddacb543ea86}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.RichText)
				.FieldSortOrder(16);

			config.Field(p => p.CheckoutLink)
				.IsCodeFirst()
				.FieldId("{a22e1349-5feb-47b8-8efe-efaa40b03998}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.GeneralLink)
				.FieldSortOrder(20);

			config.Field(p => p.ContinueShoppingLink)
				.IsCodeFirst()
				.FieldId("{b38d85ef-0f7f-4085-91b6-3a07afcd033c}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.GeneralLink)
				.FieldSortOrder(30);
		}

		private static void AddOrderConfirmationEmail(SitecoreFluentConfigurationLoader loader)
		{
			var config = loader.Add<OrderConfirmationEmail>()
				.CodeFirst()
				.TemplateName("_OrderConfirmationEmail")
				.TemplateId("{7ef6f33b-4567-4a55-b336-7bd3cfbaf450}")
				.AutoMap();

			config.Field(p => p.Header)
				.IsCodeFirst()
				.FieldId("{8d7bdb1d-9934-4942-8e98-323393688659}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.SingleLineText)
				.FieldSortOrder(1);

			config.Field(p => p.Salutation)
				.IsCodeFirst()
				.FieldId("{b1c85f18-dce2-48ea-b268-2c1dafbd97de}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.SingleLineText)
				.FieldSortOrder(10);

			config.Field(p => p.Introduction)
				.IsCodeFirst()
				.FieldId("{828824ac-a4a9-410c-9c17-f15bcbe4018e}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.RichText)
				.FieldSortOrder(20);

			config.Field(p => p.OrderReceived)
				.IsCodeFirst()
				.FieldId("{eb3ccc47-1bfc-4f07-853d-54aeee133259}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.SingleLineText)
				.FieldSortOrder(30);

			config.Field(p => p.OrderWillBeSent)
				.IsCodeFirst()
				.FieldId("{e268adf0-8f27-4e00-9392-0921c89a62d1}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.RichText)
				.FieldSortOrder(40);

			config.Field(p => p.OrderOverview)
				.IsCodeFirst()
				.FieldId("{bf5975c4-558f-4905-b755-86a9a41e8543}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.SingleLineText)
				.FieldSortOrder(50);

			config.Field(p => p.OrderContains)
				.IsCodeFirst()
				.FieldId("{e8fc113d-cd95-42ad-840d-5d861c10d6ea}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.MultiLineText)
				.FieldSortOrder(60);

			config.Field(p => p.Pieces)
				.IsCodeFirst()
				.FieldId("{bdae7105-0ed6-4113-bc96-505b306a3acc}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.SingleLineText)
				.FieldSortOrder(70);

			config.Field(p => p.AccountingDetails)
				.IsCodeFirst()
				.FieldId("{0ebe7ad6-ce99-46ab-ba9e-0f0640bee1ac}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.SingleLineText)
				.FieldSortOrder(80);

			config.Field(p => p.ShippingDetails)
				.IsCodeFirst()
				.FieldId("{527c3074-a2b2-4157-8a32-65b17edd90dc}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.SingleLineText)
				.FieldSortOrder(85);

			config.Field(p => p.SenderDetails)
				.IsCodeFirst()
				.FieldId("{219cc954-015b-4c4f-bce4-e051988d6617}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.RichText)
				.FieldSortOrder(90);

			config.Field(p => p.AdditionalContent)
				.IsCodeFirst()
				.FieldId("{c3f0da68-07ee-4eb6-bf2d-402a8d7ad151}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.RichText)
				.FieldSortOrder(95);

			config.Field(p => p.Copyright)
				.IsCodeFirst()
				.FieldId("{a728fd00-5fe2-4d9f-a71e-df14235f034a}")
				.SectionName("Content")
				.FieldType(SitecoreFieldType.SingleLineText)
				.FieldSortOrder(100);
		}

		private static void AddProductVariants(SitecoreFluentConfigurationLoader loader)
        {
            var productVariantsConfig = loader.Add<ProductVariants>()
                .CodeFirst()
                .TemplateName("_ProductVariants")
                .TemplateId("{36E32D27-1604-4A5B-A257-EC7D90F9C46F}")
                .AutoMap();

            productVariantsConfig.Field(p => p.TextField)
                .IsCodeFirst()
                .FieldId("{5F44F2AF-73D9-43BA-B35A-6A2EFC59B030}")
                .SectionName("Content")
                .FieldType(SitecoreFieldType.DropTree);
        }
    }
}
