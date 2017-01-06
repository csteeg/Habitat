using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using BoC.InversionOfControl;
using BoC.Persistence.SitecoreGlass.Models;
using BoC.Services;
using Sitecore.Data;

namespace Sitecore.Foundation.GlassMapper.Converters
{
	//TODO: this gets called for ProductBase.CategoriesList because it has [IndexField(Constants.FieldIndexes.CategoriesList)]
	//we should however make this lazy somehow, this can be slow
	public class SitecoreItemConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (typeof(SitecoreItem).IsAssignableFrom(sourceType))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return ItemService.Get(ShortID.Parse((string)value).Guid);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return new ShortID(((SitecoreItem)value).Id).ToString().ToLowerInvariant();
		}

		private IModelService<SitecoreItem> _itemService;
		public IModelService<SitecoreItem> ItemService
		{
			get { return _itemService ?? (_itemService = IoC.Resolver.Resolve<IModelService<SitecoreItem>>()); }
		}
	}
}