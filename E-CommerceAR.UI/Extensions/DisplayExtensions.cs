using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web.Mvc;

namespace E_CommerceAR.UI.Extensions
{
	public static class DisplayExtensions
	{
        public static HtmlString Translate(this IHtmlHelper html, string Arabic, string English)
        {
            var httpContextAccessor = html.ViewContext.HttpContext.RequestServices.GetService<IHttpContextAccessor>();

            if (httpContextAccessor != null)
            {
                string language = (string)httpContextAccessor.HttpContext.Session.GetString("language");

                if (language == "en")
                {
                    return new HtmlString(English);
                }
                else
                {
                    return new HtmlString(Arabic);
                }
            }
            else
            {
                throw new InvalidOperationException("IHttpContextAccessor is null.");
            }
        }

    }
}
