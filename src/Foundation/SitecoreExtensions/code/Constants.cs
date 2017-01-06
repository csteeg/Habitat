using Sitecore.Data;

namespace Sitecore.Foundation.SitecoreExtensions
{
	public struct Constants
	{
		public struct DynamicPlaceholdersLayoutParameters
		{
			public static string UseStaticPlaceholderNames => "UseStaticPlaceholderNames";
		}

		public struct FieldIds
		{
		    public const string FieldType = "{AB162CC0-DC80-4ABF-8871-998EE5D7BA32}";
			public struct Caching
			{
				public const string VaryByUrl = "{BDB4124B-47D8-4BAB-B9A5-CE82EC0C6387}";
				public const string VaryByRenderingId = "{31DF8451-5C6A-430A-8806-978EC7FD020C}";
			}
		}
	}
}