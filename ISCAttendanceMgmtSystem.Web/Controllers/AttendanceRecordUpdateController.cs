using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ISCAttendanceMgmtSystem.BL.AttendanceRecordUpdate;
using ISCAttendanceMgmtSystem.COM.Models;
using ISCAttendanceMgmtSystem.COM.Util.Controll;
using ISCAttendanceMgmtSystem.COM.Util.Database;
using System.IO;
using ISCAttendanceMgmtSystem.BL.Login;
using System.Web.Services;
using ISCAttendanceMgmtSystem.COM.Util.Config;
using System.Configuration;
using ISCAttendanceMgmtSystem.COM.Entity.Session;
using ISCAttendanceMgmtSystem.COM.Util.File;
using ISCAttendanceMgmtSystem.Log.Common;
using Microsoft.Ajax.Utilities;
using ISCAttendanceMgmtSystem.Bl.Top;
using System.Data.Entity.Infrastructure;

namespace ISCAttendanceMgmtSystem.Web.Controllers {
    /// <summary>
    /// 勤務実績修正検索コントローラー
    /// </summary>
    public class AttendanceRecordUpdateController : Controller {

        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 初期表示
        /// </summary>
        /// <returns></returns>
        public ActionResult Index() {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //初期化
                AttendanceRecordUpdateModels model = new AttendanceRecordUpdateModels {
                    Head = new AttendanceRecordUpdateModel(),
                    Search = new AttendanceRecordUpdateSearchModel(),
                    Input = new AttendanceRecordUpdateInputModel(),
                    SearchResult = new List<AttendanceRecordUpdateListModel>(),
                    Down = new AttendanceRecordDownLoadModel()
                };

                var lu = (LoginUser)Session["LoginUser"];
                model.Head.ViewType = "1";  //初期表示
                model.Head.EmployeeNo = lu.UserCode;
                model.Search.Date = DateTime.Today;
                model.Search.EmployeeItemList = (new AttendanceRecordUpdateBL().AttendanceRecordUpdateEmployeeList((LoginUser)Session["LoginUser"]));
                //表示
                return View(model);
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 検索
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [ActionName("Index")]
        [ButtonHandler(ButtonName = "SearchEx")]
        public ActionResult SearchEx(AttendanceRecordUpdateModels model, string value)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "検索開始", "");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "Main";

                //2023-10-30 iwai-tamura upd-str ---
                //従業員リストから従業員Codeを抜き出す
                string[] arrayTmp = model.Search.EmployeeData.Split(',');
                model.Search.EmployeeNo = arrayTmp[0];                  //従業員Code
                //2023-10-30 iwai-tamura upd-end ---

                //表示
                ModelState.Clear();
                model.Search.EmployeeItemList = (new AttendanceRecordUpdateBL().AttendanceRecordUpdateEmployeeList((LoginUser)Session["LoginUser"]));
                model.Head.ViewType = "2";
                return View((new AttendanceRecordUpdateBL().Search(model, (LoginUser)Session["LoginUser"])));

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
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "検索終了", "");

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// 登録
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index")]
        [ButtonHandler(ButtonName = "Save")]
        public ActionResult Save(AttendanceRecordUpdateModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "登録開始", "");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");


                //2023-10-30 iwai-tamura upd-str ---
                //従業員リストから従業員Codeを抜き出す
                string[] arrayTmp = model.Search.EmployeeData.Split(',');
                model.Search.EmployeeNo = arrayTmp[0];                  //従業員Code
                //2023-10-30 iwai-tamura upd-end ---


                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "Main";

                AttendanceRecordUpdateBL bl = new AttendanceRecordUpdateBL();
                if (bl.AttendanceRecordUpdate(model, (LoginUser)Session["LoginUser"])) {
                    TempData["Confirmation"] = string.Format("勤務実績の修正が完了しました。");
                } else {
                    TempData["Confirmation"] = string.Format("打刻エラー<br>管理者に問い合わせください。");
                }

                //初期化
                model.Input = new AttendanceRecordUpdateInputModel();
                model.SearchResult = new List<AttendanceRecordUpdateListModel>();

                model.Head.ViewType = "1";  //初期表示
                model.Head.EmployeeNo = ((LoginUser)Session["LoginUser"]).UserCode;
                model.Search.EmployeeItemList = (new AttendanceRecordUpdateBL().AttendanceRecordUpdateEmployeeList((LoginUser)Session["LoginUser"]));

                //表示
                ModelState.Clear();
                return View(model);

            }
            catch (Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally {
                //終了
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "登録終了", "");

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// 取消
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index")]
        [ButtonHandler(ButtonName = "Cancel")]
        public ActionResult Cancel(AttendanceRecordUpdateModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "Main";

                //初期化
                model.Input = new AttendanceRecordUpdateInputModel();
                model.SearchResult = new List<AttendanceRecordUpdateListModel>();

                model.Head.ViewType = "1";  //初期表示
                model.Head.EmployeeNo = ((LoginUser)Session["LoginUser"]).UserCode;
                model.Search.EmployeeItemList = (new AttendanceRecordUpdateBL().AttendanceRecordUpdateEmployeeList((LoginUser)Session["LoginUser"]));

                //表示
                ModelState.Clear();
                return View(model);

            }
            catch (Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// コンフィグから帳票出力フォルダ取得
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private string GetTempDir(Configuration config, string trg = "") {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                if (trg == "Objective") {
                    return FileUtil.GetTempDir(config, "DOWNLOAD_TEMP_DIR_O");
                } else if (trg == "Skill") {
                    return FileUtil.GetTempDir(config, "DOWNLOAD_TEMP_DIR_S");
                } else if (trg == "Self") {
                    return FileUtil.GetTempDir(config, "DOWNLOAD_TEMP_DIR_SelfDeclare");
                } else if (trg == "Career") {
                    return FileUtil.GetTempDir(config, "DOWNLOAD_TEMP_DIR_CareerSheet");
                }
                return "";
            }
            catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            }
            finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
    }
}
