using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using BoC.Persistence.SitecoreGlass.Models;

namespace Sitecore.Feature.PageContent.Models
{
    [SitecoreType(true, "{5682B289-AD03-41BE-8B49-E93795C92504}")]
    public class Content : SitecoreItem
    {
        [SitecoreField("{FD1A277D-1A20-4F0C-BE81-39A4D6F71B5D}", SitecoreFieldType.RichText, FieldName = "Content", FieldSortOrder = 100)]
        public virtual string RTEContent { get; set; }
    }
}