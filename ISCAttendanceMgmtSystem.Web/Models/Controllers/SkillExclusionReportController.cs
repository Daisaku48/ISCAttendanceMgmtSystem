using SkillDiscriminantSystem.BL.Login;
using SkillDiscriminantSystem.BL.SkillExclusionReport;
using SkillDiscriminantSystem.COM.Entity.Session;
using SkillDiscriminantSystem.COM.Models;
using SkillDiscriminantSystem.COM.Util.Config;
using SkillDiscriminantSystem.COM.Util.Controll;
using SkillDiscriminantSystem.COM.Util.File;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkillDiscriminantSystem.Web.Controllers
{
    public class SkillExclusionReportController : Controller
    {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion
        // GET: SkillExclusionReport
        public ActionResult Search(SkillExclusionReportModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                LoginUser lu = (LoginUser)Session["LoginUser"];
                if (lu.職能判定除外 == "0")
                {
                        TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                        return RedirectToAction("Index", "Top");
                }

                SkillExclusionReportBL SDbl = new SkillExclusionReportBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
                model = SDbl.Search(model);

                return View(model);
            }
            catch (Exception ex)
            {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally
            {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        
        /// <summary>
        /// 出力処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ActionName("Search")]
        [AcceptButton(ButtonName = "Print")]
        public ActionResult Print(SkillExclusionReportModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SkillExclusionReportBL SERbl = new SkillExclusionReportBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));

                //DL処理変更
                var dlpath = SERbl.Print(model.Search);
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            }
            catch (Exception ex)
            {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally
            {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        
        /// <summary>
        /// 戻る
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "Back")]
        public ActionResult Back(ObjectivesSearchViewModels model, string value)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //トップへ
                return RedirectToAction("Index", "Top");
            }
            catch (Exception ex)
            {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally
            {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// コンフィグから帳票出力フォルダ取得
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private string GetTempDir(Configuration config)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                return FileUtil.GetTempDir(config, "DOWNLOAD_TEMP_DIR_SE");
            }
            catch (Exception ex)
            {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            }
            finally
            {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
    }
}