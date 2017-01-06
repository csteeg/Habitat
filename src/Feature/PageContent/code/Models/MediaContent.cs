using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using BoC.Persistence.SitecoreGlass.Models;
using Glass.Mapper.Sc.Fields;

namespace Sitecore.Feature.PageContent.Models
{
    [SitecoreType(true, "{C89B969E-A40C-4DDD-B35A-7DAB374AB638}")]
    public class MediaContent : SitecoreItem
    {
        [SitecoreField("{B4F36B0C-79D9-410C-B93D-724CC7D0C95F}", SitecoreFieldType.Image, FieldName = "Image", FieldSortOrder = 100)]
        public virtual Image Image{ get; set; }

        [SitecoreField("{B87440BE-C45A-48B9-8981-D2E4A043EC7F}", SitecoreFieldType.GeneralLink, FieldName = "Video", FieldSortOrder = 300)]
        public virtual Link Video { get; set; }

		[SitecoreField("{d9d84eca-748c-4c1e-a6da-84a9628f6fda}", SitecoreFieldType.Checkbox, FieldName = "Scale image off", FieldSortOrder = 200)]
		public virtual bool ScaleImage { get; set; }
	}
}