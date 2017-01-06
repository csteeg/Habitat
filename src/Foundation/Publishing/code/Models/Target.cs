using BoC.Persistence.SitecoreGlass.Models;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace Valtech.Foundation.Publishing.Models
{
    [SitecoreType(false, "{E130C748-C13B-40D5-B6C6-4B150DC3FAB3}")]
    public class Target : SitecoreItem
    {
        [SitecoreField(CodeFirst = false, FieldId = "{39ECFD90-55D2-49D8-B513-99D15573DE41}")]
        public virtual string DatabaseName { get; set; }
    }
}
