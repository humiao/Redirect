using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Redirect
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RedirectConfigSection coll =
               (RedirectConfigSection)System.Configuration.ConfigurationManager.GetSection(
               "redirections");
            if (!string.IsNullOrEmpty(coll.DefaultExpiredInfo))
                RedirectConfigItem.DefaultExpiredInfo = coll.DefaultExpiredInfo;
            foreach (var item in coll.ConfigCollection)
            {
                var cfg = new RedirectConfigItem((RedirectConfigElement)item);
                Application[cfg.OldDomain] = cfg;
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}