using System.Web;
using System.Web.Optimization;

namespace ISCAttendanceMgmtSystem.Web
{
    public class BundleConfig
    {
        // バンドルの詳細については、http://go.microsoft.com/fwlink/?LinkId=301862  を参照してください
        public static void RegisterBundles(BundleCollection bundles)
        {

            //バンドル変更フラグ true・・開発バージョン false・・最少バージョン
            var isDev = true;
            //ブラウザ判定
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //jsライブラリ追加
            if (isDev)
            {
                /* jquery系 開発バージョン*/
                //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                //            "~/Scripts/jquery-{version}.js"));
                bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                            "~/Scripts/jquery-2.1.3.js"));
                //jqueryold
                bundles.Add(new ScriptBundle("~/bundles/jqueryold").Include(
                "~/Scripts/jquery-1.11.2.js"));
                bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                            "~/Scripts/jquery.validate*"));
                bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                            "~/Scripts/jquery-ui-{version}.js"));

                bundles.Add(new ScriptBundle("~/bundles/jqueryuidp").Include(
                            "~/Scripts/jquery.ui.datepicker-ja.min.js"));

                //bootstrap　フルバージョン
                bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                          "~/Scripts/bootstrap.js"));
                //IE8対策用
                bundles.Add(new ScriptBundle("~/bundles/respond").Include(
                          "~/Scripts/respond.js"));
                bundles.Add(new ScriptBundle("~/bundles/html5shiv").Include(
                          "~/Scripts/html5shiv.js"));
                ////Bootstrap-Select
                //bundles.Add(new ScriptBundle("~/bundles/bootstrapsel").Include(
                //          "~/Scripts/bootstrap-select.js"));
            }
            else
            {
                /* jquery系 最少バージョン*/
                bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                            "~/Scripts/jquery-2.1.3.min.js"));
                bundles.Add(new ScriptBundle("~/bundles/jqueryold").Include(
                            "~/Scripts/jquery-1.11.2.min.js"));
                bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                            "~/Scripts/jquery.validate.min.js"));
                bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                            "~/Scripts/jquery-ui-1.11.2.min.js"));

                bundles.Add(new ScriptBundle("~/bundles/jqueryuidp").Include(
                            "~/Scripts/jquery.ui.datepicker-ja.min.js"));
                //bootstrap　最少バージョン
                bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                          "~/Scripts/bootstrap.min.js"));
                //IE8対策用
                bundles.Add(new ScriptBundle("~/bundles/respond").Include(
                          "~/Scripts/respond.js"));
                bundles.Add(new ScriptBundle("~/bundles/html5shiv").Include(
                          "~/Scripts/html5shiv.min.js"));
            }

            //自作ライブラリ追加
            //共通
            bundles.Add(new ScriptBundle("~/Scripts/js/cmn").Include(
                        "~/Scripts/js/common.js"));
            //LOGIN画面
            bundles.Add(new StyleBundle("~/Content/css/login").Include(
                      "~/Content/login.css"));

            //TOP
            bundles.Add(new StyleBundle("~/Content/css/top").Include(
                    "~/Content/top.css",
                    "~/Content/datetime.css"));
            bundles.Add(new ScriptBundle("~/Scripts/js/top").Include(
                    "~/Scripts/js/top.js"));
            //パスワード変更
            bundles.Add(new StyleBundle("~/Content/css/c-pass").Include(
                    "~/Content/change-password.css"));
            bundles.Add(new StyleBundle("~/Scripts/js/cpw").Include(
                    "~/Scripts/js/change-password.js"));

            //勤務実績確認
            bundles.Add(new StyleBundle("~/Content/css/attend-view").Include(
                    "~/Content/attendance-record-view.css"));
            bundles.Add(new ScriptBundle("~/Scripts/js/attend-view").Include(
                    "~/Scripts/js/attendance-record-view.js"));

            //2023-02-28 iwai-tamura upd-str ------
            bundles.Add(new StyleBundle("~/Content/css/attend-mgrview").Include(
                    "~/Content/attendance-record-managerview.css"));
            bundles.Add(new ScriptBundle("~/Scripts/js/attend-mgrview").Include(
                    "~/Scripts/js/attendance-record-managerview.js"));
            //2023-02-28 iwai-tamura upd-end ------

            //2023-02-01 iwai-tamura upd-str ------
            //勤務実績修正
            bundles.Add(new StyleBundle("~/Content/css/attend-upd").Include(
                    "~/Content/attendance-record-update.css"));
            bundles.Add(new ScriptBundle("~/Scripts/js/attend-upd").Include(
                    "~/Scripts/js/attendance-record-update.js"));
            //2023-02-01 iwai-tamura upd-end ------


            //cssファイル追加
            //bootstrap、自作、レスポンシブOFF
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      //                      "~/Content/site.css",
                      "~/Content/application.css",
                      "~/Content/themes/base/all.css",
                      "~/Content/non-responsive.css",
                      "~/Content/bootstrap-select.css"
                      ));

        }
    }
}
