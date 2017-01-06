using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.IO;
using Sitecore.Sites;

namespace Sitecore.Foundation.Alerts.Repositories
{
	public class ErrorRepository
	{
		public static object HttpContextStatusKey = typeof(ErrorRepository);
		public static string HeaderStatusKey = "X-Sitecore-Error-Status";
		public static Item GetErrorItem(SiteContext siteContext, int statusCode, Database database)
		{
			var startPath = string.IsNullOrEmpty(siteContext.ContentStartItem) ? siteContext.StartPath : siteContext.ContentStartPath;
			var errorPath = FileUtil.MakePath(startPath, "Errors/" + statusCode);
			return database.GetItem(errorPath) ?? database.GetItem(FileUtil.MakePath(startPath, "Errors/Generic"));
		}
	}
}