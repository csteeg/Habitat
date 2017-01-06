using BoC.Sitecore.CodeFirstRenderings.DataProviders;
using System.Web.Mvc;
using Sitecore.Feature.PageContent.Models;

namespace Sitecore.Feature.PageContent.Controllers
{
    public class PageContentController : Controller
    {
        [DataSourceLocation("query:./ancestor-or-self::*[@@templatekey='site root']/Global")]
        [FieldValue(Constants.FieldIds.SupportsLocalDataSource, "1")]
        [Cacheable(VaryByData = true)]
        public ActionResult Content(Content dataSource)
        {
            return View(dataSource);
        }

        [DataSourceLocation("query:./ancestor-or-self::*[@@templatekey='site root']/Global")]
        [FieldValue(Constants.FieldIds.SupportsLocalDataSource, "1")]
        [Cacheable(VaryByData = true)]
        public ActionResult MediaContent(MediaContent dataSource)
        {
            return View(dataSource);
        }
    }
}