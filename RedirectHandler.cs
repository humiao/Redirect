using System.Web;
using System.Web.UI.WebControls;

namespace Redirect
{
    public class RedirectHandler : IHttpHandler
    {
        /// <summary>
        /// 判断域名是否存在，不存在当过期处理，显示默认过期字符串
        /// 判断是否已经过期，过期显示过期字符串
        /// 判断是否需要显示提示文案，需要就显示，不需要就直接跳转
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            var cfg = (RedirectConfigItem)context.Application[context.Request.Url.Host];
            if (cfg == null)
            {
                context.Response.Write("<html><body><script>alert('" + RedirectConfigItem.DefaultExpiredInfo + "');</script></body></html>");
                return;
            }

            cfg = (RedirectConfigItem)cfg;

            if (cfg.IsExpired)
            {
                context.Response.Write("<html><body><script>alert('" + cfg.ExpiredInfo + "');</script></body></html>");
                return;
            }

            var newUrl = context.Request.Url.AbsoluteUri.Replace(cfg.OldDomain, cfg.NewDomain);
            if (!cfg.IsKeepPath) newUrl = newUrl.Replace(context.Request.Url.AbsolutePath, string.Empty);

            if (cfg.IsNeedWarning)
            {
                context.Response.Write("<html><body><script>alert('" + cfg.WarningInfo +
                    "');window.open('" + newUrl + "','_self')</script></body></html>");
                return;
            }
            else
            {
                HttpContext.Current.Response.StatusCode = 301;
                HttpContext.Current.Response.Status = "301 Moved Permanently";
                HttpContext.Current.Response.AddHeader("Location", newUrl);
                HttpContext.Current.Response.End();
                return;
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}