using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ISCAttendanceMgmtSystem.BL.AttendanceRecordManagerView;
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
using ISCAttendanceMgmtSystem.BL.AttendanceRecordUpdate;

namespace ISCAttendanceMgmtSystem.Web.Controllers {
    /// <summary>
    /// 管理者用勤務実績確認コントローラー
    /// </summary>
    public class AttendanceRecordManagerViewController : Controller {

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
                AttendanceRecordManagerViewModels model = new AttendanceRecordManagerViewModels {
                    Search = new AttendanceRecordManagerViewModel(),
                    SearchResult = new List<AttendanceRecordManagerViewListModel>(),
                    Down = new AttendanceRecordDownLoadModel()
                };

                var lu = (LoginUser)Session["LoginUser"];
                model.Search.EmployeeNo = lu.UserCode;
                model.Search.EmployeeName = lu.UserName;
                model.Search.EntryYear = DateTime.Now.Year.ToString();
                model.Search.EntryMonth = DateTime.Now.Month.ToString();
                model.Search.EmployeeItemList = (new AttendanceRecordManagerViewBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()))).AttendanceRecordManagerViewEmployeeList((LoginUser)Session["LoginUser"]));

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
        public ActionResult SearchEx(AttendanceRecordManagerViewModels model, string value)
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
                model.Search.EmployeeItemList = (new AttendanceRecordManagerViewBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()))).AttendanceRecordManagerViewEmployeeList((LoginUser)Session["LoginUser"]));
                return View((new AttendanceRecordManagerViewBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())))).Search(model, (LoginUser)Session["LoginUser"]));

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
        /// Excel出力処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ButtonHandler(ButtonName = "DateDownload")]
        public ActionResult DateDownload(AttendanceRecordManagerViewModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //2023-10-30 iwai-tamura upd-str ---
                //従業員リストから従業員Codeを抜き出す
                string[] arrayTmp = model.Search.EmployeeData.Split(',');
                model.Search.EmployeeNo = arrayTmp[0];                  //従業員Code
                //2023-10-30 iwai-tamura upd-end ---

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "勤務データ出力開始", "");

                // 対象(出力フォルダ取得用)
                //string trg = model.OutputTarget;

                //帳票作成ディレクトリを取得
                string fullPath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()));

                //帳票出力ロジックを実行
                AttendanceRecordManagerViewBL bl = new AttendanceRecordManagerViewBL(fullPath);

                // データ取得・ファイル作成
                var dlpath = bl.ExcelOutput(model, (LoginUser)Session["LoginUser"]);

                //DL処理
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            }
            catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            }
            finally {
                //終了
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode), "勤務データ出力終了", "");

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

        /// <summary>
        /// コンフィグからフォーマット用出力ファイル取得
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private string GetFormatFilePath(Configuration config, string proc = "", string trg = "") {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                switch (proc) {
                    case "TakeOverMoveBulk":
                        if (trg == "Objective") {
                            return FileUtil.GetTempFile(config, "DOWNLOAD_FORMATFILE_DIR_MOVE_O");
                        } else {
                            return "";
                        }
                    case "TakeOverAmendmentCompanyBulk":
                        if (trg == "Objective") {
                            return FileUtil.GetTempFile(config, "DOWNLOAD_FORMATFILE_DIR_AMENDMENT_O");
                        } else {
                            return "";
                        }
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


        /// <summary>
        /// 戻る
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "Back")]
        public ActionResult Back(AttendanceRecordManagerViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //トップへ
                return RedirectToAction("Index", "Top");
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
        /// コンフィグから帳票出力フォルダ取得
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private string GetTempDir(Configuration config) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                return FileUtil.GetTempDir(config, "DOWNLOAD_TEMP_DIR_O");
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
    }
}
