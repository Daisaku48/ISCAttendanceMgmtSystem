using System.Web;
using System.Web.Optimization;

namespace SkillDiscriminantSystem.Web {
    public class BundleConfig {
        // バンドルの詳細については、http://go.microsoft.com/fwlink/?LinkId=301862  を参照してください
        public static void RegisterBundles(BundleCollection bundles) {

            //バンドル変更フラグ true・・開発バージョン false・・最少バージョン
            var isDev = true;
            //ブラウザ判定
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //jsライブラリ追加
            if(isDev) {
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
            } else {
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
                //Bootstrap-Select
                //bundles.Add(new ScriptBundle("~/bundles/bootstrapsel").Include(
                //          "~/Scripts/bootstrap-select.min.js"));
            }

            //自作ライブラリ追加
            //共通
            bundles.Add(new ScriptBundle("~/Scripts/js/cmn").Include(
                        "~/Scripts/js/common.js"));
            //TOP
            bundles.Add(new ScriptBundle("~/Scripts/js/top").Include(
                        "~/Scripts/js/top.js"));
            //パスワード変更
            bundles.Add(new ScriptBundle("~/Scripts/js/cpw").Include(
                        "~/Scripts/js/change-password.js"));
            //目標管理
            bundles.Add(new ScriptBundle("~/Scripts/js/obj").Include(
                        "~/Scripts/js/objects-management.js"));
            //目標管理検索
            bundles.Add(new ScriptBundle("~/Scripts/js/objs").Include(
                        "~/Scripts/js/objects-search.js"));
            //目標管理印刷
            bundles.Add(new ScriptBundle("~/Scripts/js/objp").Include(
                        "~/Scripts/js/objects-print.js"));
            //職能職務判定
            bundles.Add(new ScriptBundle("~/Scripts/js/skl").Include(
                        "~/Scripts/js/skill-assessment.js"));
            //職能職務判定検索
            bundles.Add(new ScriptBundle("~/Scripts/js/skls").Include(
                        "~/Scripts/js/skill-search.js"));
            //職能職務判定印刷
            bundles.Add(new ScriptBundle("~/Scripts/js/sklp").Include(
                        "~/Scripts/js/skill-print.js"));
            //2017-03-31 sbc-sagara add str 職能職務判定集計追加
            //職能職務判定集計
            bundles.Add(new ScriptBundle("~/Scripts/js/skld").Include(
                        "~/Scripts/js/skill-distribution.js"));
            //2017-03-31 sbc-sagara add end 職能職務判定集計追加

            //2017-04-30 sbc-sagara add str 職能判定除外登録/修正追加
            //職能職務判定除外登録
            bundles.Add(new ScriptBundle("~/Scripts/js/skle").Include(
                        "~/Scripts/js/skill-exclusion.js"));
            //2017-04-30 sbc-sagara add end 職能判定除外登録/修正追加
            //2017-04-30 sbc-sagara add str 職能判定除外調書追加
            //職能職務判定除外登録
            bundles.Add(new ScriptBundle("~/Scripts/js/skler").Include(
                        "~/Scripts/js/skill-exclusion-report.js"));
            //2017-04-30 sbc-sagara add end 職能判定除外調書追加
            // 2017-04-30 sbc-sagara add str 職能判定一括登録
            bundles.Add(new StyleBundle("~/Scripts/js/sklc").Include(
                      "~/Scripts/js/skill-assessment-choice.js"));
            // 2017-04-30 sbc-sagara add end 職能判定一括登録


            //システム管理画面
            //職能職務判定印刷
            bundles.Add(new ScriptBundle("~/Scripts/js/sys").Include(
                        "~/Scripts/js/system-management.js"));
            //決裁権限検索
            bundles.Add(new ScriptBundle("~/Scripts/js/apps").Include(
                        "~/Scripts/js/approver-search.js"));
            //決裁権限詳細
            bundles.Add(new ScriptBundle("~/Scripts/js/app").Include(
                        "~/Scripts/js/approver-setting.js"));

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

            //自作cssファイル追加
            //目標管理
            bundles.Add(new StyleBundle("~/Content/css/objs").Include(
                      "~/Content/objects-search.css"));
            bundles.Add(new StyleBundle("~/Content/css/obj").Include(
                      "~/Content/objects-management.css"));
            //職能職務判定
            bundles.Add(new StyleBundle("~/Content/css/skls").Include(
                      "~/Content/skill-search.css"));
            bundles.Add(new StyleBundle("~/Content/css/skl").Include(
                      "~/Content/skill-assessment.css"));
            // 2017-04-30 sbc-sagara add str 職能判定一括登録
            bundles.Add(new StyleBundle("~/Content/css/sklc").Include(
                      "~/Content/skill-assessment-choice.css"));
            // 2017-04-30 sbc-sagara add end 職能判定一括登録
            //決裁
            bundles.Add(new StyleBundle("~/Content/css/aprs").Include(
                      "~/Content/appro-search.css"));
        }
    }
}
