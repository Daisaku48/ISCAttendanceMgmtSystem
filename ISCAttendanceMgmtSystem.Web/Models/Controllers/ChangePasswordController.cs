using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using SkillDiscriminantSystem.BL.ChangePassword;
using SkillDiscriminantSystem.BL.Login;
using SkillDiscriminantSystem.COM.Entity.Session;
using SkillDiscriminantSystem.COM.Enum;
using SkillDiscriminantSystem.COM.Models;
using SkillDiscriminantSystem.COM.Util.Controll; // 2017-03-31 sbc-sagara add 戻るボタン追加
using SkillDiscriminantSystem.COM.Util.Convert;
using SkillDiscriminantSystem.COM.Util.Database;

namespace SkillDiscriminantSystem.Web.Controllers {
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

        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModelSd model) {

            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //未入力チェック
                if(!ModelState.IsValid) return View(model);

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //パスワード変更
                ChangePasswordBL bl = new ChangePasswordBL();
                if(!bl.ChangePassword(model, lu)) throw new Exception("パスワード更新エラー");

                return View(model);

            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                //TODO:エラーは思案中
                ModelState.AddModelError("", ex.Message);
                return View(model);
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        
        // 2017-03-31 sbc-sagara add str 戻るボタン追加
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
        // 2017-03-31 sbc-sagara add end 戻るボタン追加
    }
}
