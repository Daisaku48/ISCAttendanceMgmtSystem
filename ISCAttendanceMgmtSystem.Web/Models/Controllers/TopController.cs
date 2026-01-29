using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using SkillDiscriminantSystem.COM.Models;
using SkillDiscriminantSystem.COM.Util.Controll;
using SkillDiscriminantSystem.COM.Util.Database;
using SkillDiscriminantSystem.BL.Login;
using SkillDiscriminantSystem.Bl.Top;
using System.Collections;

namespace SkillDiscriminantSystem.Web.Controllers {

    /// <summary>
    /// TOP画面用コントローラー
    /// </summary>
    public class TopController : Controller {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// ボタンリンクアクションテーブル
        /// </summary>
        /// <remarks>
        /// 目標管理、職能判定、システム管理、パスワード変更、ログアウト
        /// </remarks>
        private Hashtable linkTable = new Hashtable() { { "ObjectivesSearch","Search" }
                                                      , { "SkillSearch","Search" }
                                                      , { "SystemManagement", "Index" }
                                                      , { "ChangePassword", "ChangePassword"} 
                                                      , { "Login", "Login" } 
                                                      , { "ApproverSearch", "Search"}
                                                      , { "SkillDistribution", "Search"} //2017-03-31 sbc-sagara add 職能・職務判定集計追加
                                                      , { "SkillExclusion", "Search"}   //2017-04-30 sbc-sagara add 職能・職務判定除外者登録/削除追加
                                                      , { "SkillExclusionReport", "Search"}};  //2017-04-30 sbc-sagara add 職能・職務判定除外調書追加
        #endregion

        /// <summary>
        /// 初期表示
        /// </summary>
        /// <returns>画面表示</returns>
        public ActionResult Index() {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");
                
                //2017-08-31 iwai-tamura upd-str ------
                //未承認データ確認

                
                //2017-08-31 iwai-tamura upd-end ------
                
                //表示
                return View((new TopBL()).Index());
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
        /// 画面遷移アクション
        /// </summary>
        /// <param name="value">遷移先キー名</param>
        /// <returns>画面遷移</returns>
        [ActionName("Link")]
        [ButtonHandler(ButtonName = "Transition")]
        public ActionResult TransitionAll(string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッション全削除判定 ログアウトボタンのみセッション削除
                if("Login".Equals(value)) Session.RemoveAll();

                //遷移
                return RedirectToAction((string)linkTable[value], value);
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
    }
}
