using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using ISCAttendanceMgmtSystem.BL.ChangePassword;
using ISCAttendanceMgmtSystem.BL.Login;
using ISCAttendanceMgmtSystem.COM.Entity.Session;
using ISCAttendanceMgmtSystem.COM.Enum;
using ISCAttendanceMgmtSystem.COM.Models;
using ISCAttendanceMgmtSystem.COM.Util.Controll;
using ISCAttendanceMgmtSystem.COM.Util.Convert;
using ISCAttendanceMgmtSystem.COM.Util.Database;
using ISCAttendanceMgmtSystem.Log.Common;

namespace ISCAttendanceMgmtSystem.Web.Controllers {
    public class ChangePasswordController : Controller {
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
        public ActionResult ChangePassword() {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                return View();
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                //TODO:エラーは思案中
                ModelState.AddModelError("", ex.Message);
                return View();
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModelSd model) {

            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"パスワード変更開始","");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //未入力チェック
                if(!ModelState.IsValid) return View(model);

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //パスワード変更
                ChangePasswordBL bl = new ChangePasswordBL();
                if (bl.ChangePassword(model, lu)) {
                    TempData["Confirmation"] = string.Format("パスワードを変更しました。");
                } else {
                    TempData["Confirmation"] = string.Format("パスワード更新エラー。<br>入力内容の確認を行ってください。");
                }

                return View(model);

            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
                return View(model);
            } finally {
                //終了
                CommonLog.WriteOperationLog((((LoginUser)Session["LoginUser"]).UserCode),"パスワード変更終了","");

                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        /// <summary>
        /// 戻る
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ChangePassword")]
        [AcceptButton(ButtonName = "Back")]
        public ActionResult Back()
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
    }
}