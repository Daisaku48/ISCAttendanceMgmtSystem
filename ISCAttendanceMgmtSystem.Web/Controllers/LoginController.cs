using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ISCAttendanceMgmtSystem.COM.Util.Database;
using System.Data;
using System.Text;
using ISCAttendanceMgmtSystem.COM.Entity.Session;
using ISCAttendanceMgmtSystem.COM.Models;
using ISCAttendanceMgmtSystem.BL.Login;
using ISCAttendanceMgmtSystem.COM.Util.Encrypt;

using ISCAttendanceMgmtSystem.Log.Common;

namespace ISCAttendanceMgmtSystem.Web.Controllers
{

    /// <summary>
    /// ログイン用コントローラー
    /// </summary>
    public class LoginController : Controller
    {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoginController()
        {
        }

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 初期表示
        /// </summary>
        /// <returns>ビュー</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //セッション全削除
            Session.RemoveAll();
            ///URL設定
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// ログオフ
        /// </summary>
        /// <returns>初期画面へリダイレクト</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //セッション全削除
            Session.RemoveAll();
            return RedirectToAction("Login", "Login");
        }


        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="model">ログインモデル</param>
        /// <param name="returnUrl"></param>
        /// <returns>TOPへリダイレクト</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModelSd model, string returnUrl)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //入力チェック
                if (!ModelState.IsValid) { return View(model); }

                //ログイン処理
                LoginBL bl = new LoginBL();
                // パスワード有効期限追加
                if (bl.Login(model.Id, model.Password, model))
                {
                    //if ((bool)HttpContext.Session["IsPassExpiration"]==true){
                    //    TempData["Confirmation"] = string.Format("パスワードの有効期限が過ぎています。<br>パスワードの変更を行ってください。");
                    //}

                    CommonLog.WriteOperationLog(model.Id, "ログイン成功", "");

                    return RedirectToAction("Index", "Top");
                }
                CommonLog.WriteOperationLog("Err", "ログイン失敗", "");

                //エラー判定
                TempData["Confirmation"] = string.Format("無効なログイン試行です。");
                return View(model);
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
