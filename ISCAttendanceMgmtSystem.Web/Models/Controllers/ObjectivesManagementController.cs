using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using SkillDiscriminantSystem.BL.Common;
using SkillDiscriminantSystem.BL.ObjectivesManagement;
using SkillDiscriminantSystem.COM.Enum;
using SkillDiscriminantSystem.COM.Models;
using SkillDiscriminantSystem.COM.Util.Controll;
using SkillDiscriminantSystem.COM.Util.Convert;
using SkillDiscriminantSystem.COM.Util.Database;
using SkillDiscriminantSystem.COM.Entity.Session;
using SkillDiscriminantSystem.BL.Login;
using SkillDiscriminantSystem.BL.ObjectivesSearch;
using SkillDiscriminantSystem.COM.Util.File;
using SkillDiscriminantSystem.COM.Util.Config;
using System.IO;

namespace SkillDiscriminantSystem.Web.Controllers {
    /// <summary>
    /// 目標管理入力コントローラー
    /// </summary>
    public class ObjectivesManagementController : Controller {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 初期表示
        /// </summary>
        /// <param name="id">管理番号</param>
        /// <param name="tableType">テーブルタイプ</param>
        /// <returns>ビュー</returns>
        public ActionResult Index(int? id, string tableType) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //ビジネスロジックから取得
                //return View((new ObjectivesManagementBL(tableType)).Select(id));
                var bl = new ObjectivesManagementBL(tableType);
                var model = bl.Select(id);
                var lu = (LoginUser)Session["LoginUser"];
                bl.SetMode(model, lu);
                
                return View(model);
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 承認
        /// </summary>
        /// <param name="model">目標管理入力モデル</param>
        /// <param name="value">ボタンのValue</param>
        /// <returns>ビュー</returns>
        [HttpPost]
        [ActionName("Index")]
        [ButtonHandler(ButtonName = "Signature")]
        //[AcceptButton(ButtonName = "Signature")]
        public ActionResult Sign(ObjectivesManagementViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //ビジネスロジックから
                ObjectivesManagementBL bl = new ObjectivesManagementBL();

                //承認
                bl.Sign(model, value, lu);

                var mNo = DataConv.IntParse(model.Head.ManageNo);
                var tableType = model.Head.IsRireki ? "R":"G";
                ModelState.Clear();
                //再表示
                bl = new ObjectivesManagementBL(tableType);
                model = bl.Select(mNo);
                bl.SetMode(model, lu);
                TempData["Success"] = "承認しました";
                return View(model);

                ////再表示
                //return View(bl.Select(mNo));
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 承認キャンセル
        /// </summary>
        /// <param name="model">目標管理入力モデル</param>
        /// <param name="value">ボタンのValue</param>
        /// <returns>ビュー</returns>
        [HttpPost]
        [ActionName("Index")]
        [ButtonHandler(ButtonName = "SignCancel")]
        //[AcceptButton(ButtonName = "SignCancel")]
        public ActionResult SignCancel(ObjectivesManagementViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //ビジネスロジックから
                ObjectivesManagementBL bl = new ObjectivesManagementBL();

                //承認
                bl.Sign(model, value, lu, false);

                var mNo = DataConv.IntParse(model.Head.ManageNo);
                var tableType = model.Head.IsRireki ? "R" : "G";
                ModelState.Clear();
                //再表示
                bl = new ObjectivesManagementBL(tableType);
                model = bl.Select(mNo);
                bl.SetMode(model, lu);
                TempData["Success"] = "承認をキャンセルしました";
                return View(model);
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model">目標管理入力モデル</param>
        /// <returns>ビュー</returns>
        [HttpPost]
        [ActionName("Index")]
        //[ButtonHandler(ButtonName = "Save")]
        [AcceptButton(ButtonName = "Save")]
        public ActionResult Save(ObjectivesManagementViewModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //ビジネスロジック
                ObjectivesManagementBL bl = new ObjectivesManagementBL();

                //保存
                bl.Save(model);

                var mNo = DataConv.IntParse(model.Head.ManageNo);
                var tableType = model.Head.IsRireki ? "R" : "G";
                ModelState.Clear();
                //再表示
                bl = new ObjectivesManagementBL(tableType);
                model = bl.Select(mNo);
                bl.SetMode(model, lu);
                TempData["Success"] = "保存しました";
                return View(model);
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 保存&PDF出力
        /// </summary>
        /// <param name="model">目標管理入力モデル</param>
        /// <returns>ビュー</returns>
        [HttpPost]
        [ActionName("Index")]
        //[ButtonHandler(ButtonName = "Save")]
        [AcceptButton(ButtonName = "Save2Print")]
        public ActionResult Save2Print(ObjectivesManagementViewModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];

                //ビジネスロジック
                ObjectivesManagementBL bl = new ObjectivesManagementBL();

                //保存
                bl.Save(model);
                var mNo = DataConv.IntParse(model.Head.ManageNo);
                var tableType = model.Head.IsRireki ? "R" : "G";
                ModelState.Clear();

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                ObjectivesSearchPrintBL pbl = new ObjectivesSearchPrintBL(Server.MapPath(FileUtil.GetTempDir(WebConfig.GetConfigFile(), "DOWNLOAD_TEMP_DIR_O")));
                //結果
                var dlpath = pbl.Print(new string[] { mNo + "," + tableType });
                //ダウンロード時のファイル名を取得
                string mappath = Server.MapPath(FileUtil.GetTempDir(WebConfig.GetConfigFile(), "DOWNLOAD_TEMP_DIR_O") + dlpath);
                string fileName = Path.GetFileName(mappath);

                return File(mappath, "application/zip", fileName);
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// 戻る
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index")]
        [AcceptButton(ButtonName = "Back")]
        public ActionResult Back(ObjectivesManagementViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //検索画面へ
                return RedirectToAction("Search", "ObjectivesSearch");
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
