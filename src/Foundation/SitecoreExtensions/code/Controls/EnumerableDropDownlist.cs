using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentEditor;

namespace Sitecore.Foundation.SitecoreExtensions.Controls
{
	public class EnumerableDropList : LookupEx
	{
	    public struct FieldType
	    {
	        public const string EnumerableDropList = "Enumerable DropList";
	    }

	    protected bool IsSelected(string item)
		{
			return string.Compare(this.Value, item, StringComparison.InvariantCulture) == 0;
		}

		protected override void DoRender(System.Web.UI.HtmlTextWriter output)
		{
			Assert.ArgumentNotNull(output, "output");

			List<KeyValuePair<string, string>> items;

			try
			{
				var enumType = Type.GetType(this.Source);
				items = new List<KeyValuePair<string, string>>();
				if (enumType != null && enumType.IsEnum)
				{
					items.AddRange(
						Enum.GetValues(enumType)
							.Cast<Enum>()
							.Select(value => new KeyValuePair<string, string>(value.ToString(), GetEnumDescription(value))));
				}
			}
			catch (Exception)
			{
				items = this.Source.Split(new[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries)
							  .Select(i => new KeyValuePair<string, string>(i, i)).ToList();
			}

			output.Write("<select" + this.GetControlAttributes() + ">");

			var selectedItemFound = false;
			foreach (var obj in items)
			{
				var isSelected = this.IsSelected(obj.Key);
				if (isSelected)
					selectedItemFound = true;
				output.Write("<option value=\"" + obj.Key + "\"" + (isSelected ? " selected=\"selected\"" : string.Empty) + ">" + obj.Value + "</option>");
			}
			var isOptGoup = !string.IsNullOrEmpty(this.Value) && !selectedItemFound;
			if (isOptGoup)
			{
				output.Write("<optgroup label=\"" + Translate.Text("Value not in the selection list.") + "\">");
				output.Write("<option value=\"" + this.Value + "\" selected=\"selected\">" + this.Value + "</option>");
				output.Write("</optgroup>");
			}
			output.Write("</select>");
			if (!isOptGoup) return;

			output.Write("<div style=\"color:#999999;padding:2px 0px 0px 0px\">{0}</div>", Translate.Text("The field contains a value that is not in the selection list."));
		}

		public static string GetEnumDescription(Enum value)
		{
			var fi = value.GetType().GetField(value.ToString());

			var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
								typeof(DescriptionAttribute), false);

			return attributes.Any()
				   ? attributes.First().Description
				   : value.ToString();
		}
	}
}