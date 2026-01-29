using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using SkillDiscriminantSystem.BL.Login;
using SkillDiscriminantSystem.BL.SkillAssessment;
using SkillDiscriminantSystem.COM.Util.Controll;
using SkillDiscriminantSystem.COM.Models;
using SkillDiscriminantSystem.COM.Util.Convert;
using SkillDiscriminantSystem.COM.Entity.Session;
using SkillDiscriminantSystem.BL.Common;
using SkillDiscriminantSystem.BL.SkillSearch;
using SkillDiscriminantSystem.COM.Util.File;
using SkillDiscriminantSystem.COM.Util.Config;
using System.IO;

namespace SkillDiscriminantSystem.Web.Controllers {
    /// <summary>
    /// 職能判定入力コントローラー
    /// </summary>
    public class SkillAssessmentController : Controller {
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
        public ActionResult Index(int? id,string tableType) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //SkillAssessmentViewModels model = (new SkillAssessmentBL(tableType)).Select(id);
                //管理者判定
                //LoginUser lu = (LoginUser)Session["LoginUser"];
                var bl = new SkillAssessmentBL(tableType);
                var model = bl.Select(id);
                var lu = (LoginUser)Session["LoginUser"];
                bl.SetMode(model, lu);
                
                if(!lu.IsRoot) {
                    //利用可能判定
                    //2016-10-06 iwai-tamura upd str -----
                    if(!lu.IsPost) return RedirectToAction("Index", "Top");
                    //if (!SkillCommonBL.IsValid()) return RedirectToAction("Index", "Top");
                    //2016-10-06 iwai-tamura upd end -----
                    //自分のデータか判定
                    if(model.Head.EmployeeNo == lu.UserCode) return RedirectToAction("Index", "Top");
                }

                //ビジネスロジックから取得
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
        public ActionResult Sign(SkillAssessmentViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                var query = from row in model.Evals
                            where row.ItemType == "radio"
                            select row;
                foreach(var row in query) {
                    var selected = Request.Form["radio_" + row.LargeNo + "_" + row.MediumNo];
                    row.Value = (string)selected == row.EnumValue.ToString() ? "1" : "0";
                }

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];
                //ビジネスロジックから
                SkillAssessmentBL bl = new SkillAssessmentBL();

                //承認
                bl.Sign(model, value, lu);

                var mNo = DataConv.IntParse(model.Head.ManageNo);
                var tableType = model.Head.IsRireki ? "R" : "G";
                ModelState.Clear();
                //再表示
                bl = new SkillAssessmentBL(tableType);
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
        public ActionResult SignCancel(SkillAssessmentViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];

                var query = from row in model.Evals
                            where row.ItemType == "radio"
                            select row;
                foreach(var row in query) {
                    var selected = Request.Form["radio_" + row.LargeNo + "_" + row.MediumNo];
                    row.Value = (string)selected == row.EnumValue.ToString() ? "1" : "0";
                }

                //ビジネスロジックから
                SkillAssessmentBL bl = new SkillAssessmentBL();

                //承認
                bl.Sign(model, value, lu, false);

                var mNo = DataConv.IntParse(model.Head.ManageNo);
                var tableType = model.Head.IsRireki ? "R" : "G";
                ModelState.Clear();
                //再表示
                bl = new SkillAssessmentBL(tableType);
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
        public ActionResult Save(SkillAssessmentViewModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                var query = from row in model.Evals
                            where row.ItemType == "radio"
                            select row;
                foreach(var row in query) {
                    var selected = Request.Form["radio_" + row.LargeNo + "_" + row.MediumNo];
                    row.Value = (string)selected == row.EnumValue.ToString() ? "1":"0";
                }

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];
                //ビジネスロジック
                SkillAssessmentBL bl = new SkillAssessmentBL();

                //保存
                bl.Save(model);

                var mNo = DataConv.IntParse(model.Head.ManageNo);
                var tableType = model.Head.IsRireki ? "R" : "G";
                ModelState.Clear();
                //再表示
                bl = new SkillAssessmentBL(tableType);
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
        public ActionResult Save2Print(SkillAssessmentViewModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                var query = from row in model.Evals
                            where row.ItemType == "radio"
                            select row;
                foreach(var row in query) {
                    var selected = Request.Form["radio_" + row.LargeNo + "_" + row.MediumNo];
                    row.Value = (string)selected == row.EnumValue.ToString() ? "1" : "0";
                }

                //セッションからログイン情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];
                //ビジネスロジック
                SkillAssessmentBL bl = new SkillAssessmentBL();

                //保存
                bl.Save(model);
                var mNo = DataConv.IntParse(model.Head.ManageNo);
                var tableType = model.Head.IsRireki ? "R" : "G";
                ModelState.Clear();

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SkillSearchPrintBL pbl = new SkillSearchPrintBL(Server.MapPath(FileUtil.GetTempDir(WebConfig.GetConfigFile(), "DOWNLOAD_TEMP_DIR_S")));
                //結果
                var dlpath = pbl.Print(new string[] { mNo + "," + tableType });
                //ダウンロード時のファイル名を取得
                string mappath = Server.MapPath(FileUtil.GetTempDir(WebConfig.GetConfigFile(), "DOWNLOAD_TEMP_DIR_S") + dlpath);
                string fileName = Path.GetFileName(mappath);

                return File(mappath, "application/zip", fileName);
                ////初期化
                //model.Down = new ObjectivesDownLoadModel();

                //Response.ContentType = "application/zip";                               //送信されるコンテンツの型を指定
                //Response.AppendHeader("Content-Disposition", "attachment; filename="
                //    + HttpUtility.UrlEncode(fileName) + ".zip");                        //コンテンツの処理方法を指定
                //byte[] bs = System.IO.File.ReadAllBytes(path);                          //ファイルをバイト型配列に読み込む
                //Response.BinaryWrite(bs);                                               //Resbonseにバイナリとして出力？

                ////再表示
                //bl = new ObjectivesManagementBL(tableType);
                //model = bl.Select(mNo);
                //bl.SetMode(model, lu);
                //return View(model);

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
                //検索へ
                return RedirectToAction("Search", "SkillSearch");
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
