using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NLog;

namespace ISCAttendanceMgmtSystem.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// アプリケーション開始時
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// アプリケーションエラー時処理
        /// </summary>
        /// <param name="sender">実体</param>
        /// <param name="e">付加情報</param>
        protected void Application_Error(object sender, EventArgs e) {
            //最終エラーをログに記録
            Exception lastException = Server.GetLastError();
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Fatal(lastException);
        }
    }
}
