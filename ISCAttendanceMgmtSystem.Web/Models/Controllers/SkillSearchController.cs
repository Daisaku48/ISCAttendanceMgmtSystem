using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SkillDiscriminantSystem.BL.Login;
using SkillDiscriminantSystem.COM.Models;
using SkillDiscriminantSystem.COM.Util.Controll;
using SkillDiscriminantSystem.BL.SkillSearch;
using SkillDiscriminantSystem.BL.Common;
using SkillDiscriminantSystem.COM.Entity.Session;
using SkillDiscriminantSystem.COM.Util.Config;
using System.Configuration;
using SkillDiscriminantSystem.COM.Util.File;

namespace SkillDiscriminantSystem.Web.Controllers {
    /// <summary>
    /// 職能検索コントローラー
    /// </summary>
    public class SkillSearchController : Controller {

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
        public ActionResult Search() {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                LoginUser lu = (LoginUser)Session["LoginUser"];
                if(!lu.IsRoot) {
                    //利用可能判定
                    //2016-04-14 iwai-tamura upd str -----
                    //2016-10-06 iwai-tamura upd str -----
                    if (!lu.IsPost)
                    //if (!SkillCommonBL.IsValid())
                    //2016-10-06 iwai-tamura upd str -----
                    {
                        TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                        return RedirectToAction("Index", "Top");
                    }
                    //if (!SkillCommonBL.IsValid()) return RedirectToAction("Index", "Top");
                    //2016-04-14 iwai-tamura upd end -----
                }

                //初期化
                SkillSearchViewModels model = new SkillSearchViewModels {
                    Search = new SkillSearchModel(),
                    SearchResult = new List<SkillSearchListModel>(),
                    SkillClassItems = SkillSearchBL.GetSkillClass(),
                    Down = new SkillDownLoadModel()
                };

                //ダウンロード用フラグクリア
                //model.Down.DownloadFlag = false;

                //2016-04-14 iwai-tamura upd str -----
                //TempDataある場合
                if (TempData["SkillSearch"] != null)
                {
                    //詳細設定画面から戻った時に前回の検索結果データを表示する
                    model.Search = (SkillSearchModel)TempData["SkillSearch"];
                    ModelState.Clear();

                    switch ((string)Session["SearchType"])
                    {
                        case "SearchEx":
                            //「検索」ボタンにて検索した場合
                            return View((new SkillSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));

                        case "Primary":
                            //「一次判定」ボタンにて検索した場合
                            //2017-08-31 iwai-tamura upd-str ------
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                            //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], true));
                            //2017-08-31 iwai-tamura upd-end ------

                        case "Secondary":
                            //「二次判定」ボタンにて検索した場合
                            //2017-08-31 iwai-tamura upd-str ------
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                            //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], false));
                            //2017-08-31 iwai-tamura upd-end ------

                        //2017-08-31 iwai-tamura upd-str ------
                        case "Department":
                            //「部門調整」ボタンにて検索した場合
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));

                        case "Branch":
                            //「支社調整」ボタンにて検索した場合
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                        //2017-08-31 iwai-tamura upd-end ------
                    
                    }

                }
                // 2017-03-31 sbc-sagara add str
                else
                {
                    Session["SearchType"] = "";
                }
                // 2017-03-31 sbc-sagara add end

                //2016-04-14 iwai-tamura upd end -----
                
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
        [ActionName("Search")]
        [AcceptButton(ButtonName = "SearchEx")]
        public ActionResult SearchEx(SkillSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                model.SkillClassItems = SkillSearchBL.GetSkillClass();

                //2016-04-14 iwai-tamura add str -----
                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "SearchEx";
                //2016-04-14 iwai-tamura add end -----

                //表示
                return View((new SkillSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
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

        //2017-08-31 iwai-tamura upd-str ------
        /// <summary>
        /// 支社調整検索
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "Branch")]
        public ActionResult Branch(SkillSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                model.SkillClassItems = SkillSearchBL.GetSkillClass();

                //2016-04-14 iwai-tamura add str -----
                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "Branch";
                //2016-04-14 iwai-tamura add end -----

                //表示
                //2017-08-31 iwai-tamura upd-str ------
                return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"],(string)Session["SearchType"]));
                //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"],false));
                //2017-08-31 iwai-tamura upd-end ------
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
        /// 部門調整検索
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "Department")]
        public ActionResult Department(SkillSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                model.SkillClassItems = SkillSearchBL.GetSkillClass();

                //2016-04-14 iwai-tamura add str -----
                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "Department";
                //2016-04-14 iwai-tamura add end -----

                //表示
                //2017-08-31 iwai-tamura upd-str ------
                return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"],(string)Session["SearchType"]));
                //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"],false));
                //2017-08-31 iwai-tamura upd-end ------
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
        //2017-08-31 iwai-tamura upd-end ------

        
        /// <summary>
        /// ２次検索
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "Secondary")]
        public ActionResult Secondary(SkillSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                model.SkillClassItems = SkillSearchBL.GetSkillClass();

                //2016-04-14 iwai-tamura add str -----
                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "Secondary";
                //2016-04-14 iwai-tamura add end -----

                //表示
                //2017-08-31 iwai-tamura upd-str ------
                return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"],(string)Session["SearchType"]));
                //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"],false));
                //2017-08-31 iwai-tamura upd-end ------
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
        /// １次検索
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "Primary")]
        public ActionResult Primary(SkillSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                model.SkillClassItems = SkillSearchBL.GetSkillClass();

                //2016-04-14 iwai-tamura add str -----
                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "Primary";
                //2016-04-14 iwai-tamura add end -----
                
                //表示
                //2017-08-31 iwai-tamura upd-str ------
                return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"],(string)Session["SearchType"]));
                //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], true));
                //2017-08-31 iwai-tamura upd-end ------
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
        /// 表示
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "View")]
        //[AcceptButton(ButtonName = "View")]

        //2016-04-14 iwai-tamura upd str -----
        public ActionResult ViewRedirect(SkillSearchViewModels model, string value)
        //public ActionResult ViewRedirect(ObjectivesSearchViewModels model, string value)
        //2016-04-14 iwai-tamura upd end -----
        {
            
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //2016-04-14 iwai-tamura add str -----
                //検索条件セット
                TempData["SkillSearch"] = model.Search;
                //2016-04-14 iwai-tamura add end -----


                //詳細へ
                var val = value.Split(',');
                return RedirectToAction("Index", "SkillAssessment", new { id = val[0], tableType = val[1] });
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
        /// 単票出力
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "RowPrint")]
        //[AcceptButton(ButtonName = "View")]
        public ActionResult RowPrint(ObjectivesSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");
                
                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SkillSearchPrintBL bl = new SkillSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
                //結果
                //model.Down.DownloadPath = bl.Print(new string[] { value });
                //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;

                ////表示
                //return View((new ObjectivesSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                //DL処理変更
                var dlpath = bl.Print(new string[] { value });
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);

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
        /// 戻る
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "Back")]
        public ActionResult Back(SkillSearchViewModels model, string value) {
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
        /// 印刷
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "Print")]
        public ActionResult Print(SkillSearchViewModels model, string[] selPrint) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //職能用のリスト生成
                model.SkillClassItems = SkillSearchBL.GetSkillClass();

                //対象選択エラーチェック
                if(selPrint == null) {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    // 2017-03-31 sbc-sagara add str 出力エラー時再検索
                    switch ((string)Session["SearchType"]) {
                        case "SearchEx":
                            //「検索」ボタンにて検索した場合
                            return View((new SkillSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                        case "Primary":
                            //「一次判定」ボタンにて検索した場合
                            //2017-08-31 iwai-tamura upd-str ------
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                            //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], true));
                            //2017-08-31 iwai-tamura upd-end ------
                        case "Secondary":
                            //「二次判定」ボタンにて検索した場合
                            //2017-08-31 iwai-tamura upd-str ------
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                            //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], false));
                            //2017-08-31 iwai-tamura upd-end ------

                        //2017-08-31 iwai-tamura upd-str ------
                        case "Department":
                            //「部門調整」ボタンにて検索した場合
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));

                        case "Branch":
                            //「支社調整」ボタンにて検索した場合
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                        //2017-08-31 iwai-tamura upd-end ------                    
                    }
                    // 2017-03-31 sbc-sagara add end 出力エラー時再検索
                    return View(model);
                }

                //帳票作成ディレクトリを取得
                string fullPath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()));         
                
                //帳票出力ロジックを実行
                SkillSearchPrintBL bl = new SkillSearchPrintBL(fullPath);
                //model.Down.DownloadPath = bl.Print(selPrint);
                //model.Down.DownloadFlag = true;

                ////表示
                //return View((new SkillSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                //DL処理変更
                var dlpath = bl.Print(selPrint);
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()) + dlpath);
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
        /// 一括承認
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "Sign")]
        public ActionResult Sign(SkillSearchViewModels model, string[] selPrint) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                // 2017-03-31 sbc-sagara add str
                //職能用のリスト生成
                model.SkillClassItems = SkillSearchBL.GetSkillClass();
                // 2017-03-31 sbc-sagara add end

                //対象選択エラーチェック
                if(selPrint == null) {
                    //エラー判定
                    ModelState.AddModelError("", "承認対象を選択してください。");
                    // 2017-03-31 sbc-sagara add str
                    switch ((string)Session["SearchType"])
                    {
                        case "SearchEx":
                            //「検索」ボタンにて検索した場合
                            return View((new SkillSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));

                        case "Primary":
                            //「一次判定」ボタンにて検索した場合
                            //2017-08-31 iwai-tamura upd-str ------
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                            //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], true));
                            //2017-08-31 iwai-tamura upd-end ------

                        case "Secondary":
                            //「二次判定」ボタンにて検索した場合
                            //2017-08-31 iwai-tamura upd-str ------
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                            //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], false));
                            //2017-08-31 iwai-tamura upd-end ------
                        //2017-08-31 iwai-tamura upd-str ------
                        case "Department":
                            //「部門調整」ボタンにて検索した場合
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));

                        case "Branch":
                            //「支社調整」ボタンにて検索した場合
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                        //2017-08-31 iwai-tamura upd-end ------
                    }
                    // 2017-03-31 sbc-sagara add end
                    return View("Search", model);
                }

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                SkillSearchBL bl = new SkillSearchBL();
                bl.Sign(selPrint, (LoginUser)Session["LoginUser"]);
                //結果
                //model.Down.DownloadFlag = true;
                TempData["Success"] = "承認しました";
                // 2017-03-31 sbc-sagara del str
                ////職能用のリスト生成
                //model.SkillClassItems = SkillSearchBL.GetSkillClass();
                // 2017-03-31 sbc-sagara del end

                //表示
                //2016-04-14 iwai-tamura upd str -----
                switch ((string)Session["SearchType"])
                {
                    case "SearchEx":
                        //「検索」ボタンにて検索した場合
                        return View((new SkillSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));

                    case "Primary":
                        //「一次判定」ボタンにて検索した場合
                        //2017-08-31 iwai-tamura upd-str ------
                        return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                        //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], true));
                        //2017-08-31 iwai-tamura upd-end ------

                    case "Secondary":
                        //「二次判定」ボタンにて検索した場合
                        //2017-08-31 iwai-tamura upd-str ------
                        return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                        //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], false));
                        //2017-08-31 iwai-tamura upd-end ------

                    //2017-08-31 iwai-tamura upd-str ------
                    case "Department":
                        //「部門調整」ボタンにて検索した場合
                        return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));

                    case "Branch":
                        //「支社調整」ボタンにて検索した場合
                        return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                    //2017-08-31 iwai-tamura upd-end ------

                }                
                return View(bl.Search(model, (LoginUser)Session["LoginUser"]));
                //2016-04-14 iwai-tamura upd end -----                  

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

        //2017-03-31 sbc-sagara add str 一括Excel出力
        /// <summary>
        /// 一括Excel出力処理
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintXls")]
        public ActionResult PrintXls(SkillSearchViewModels model, string[] selPrint)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //職能用のリスト生成
                model.SkillClassItems = SkillSearchBL.GetSkillClass();

                //管理者判定
                if (((LoginUser)Session["LoginUser"]).職能DATA出力 != "1") {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    if (TempData["SkillSearch"] != null) {
                        //詳細設定画面から戻った時に前回の検索結果データを表示する
                        model.Search = (SkillSearchModel)TempData["SkillSearch"];
                        ModelState.Clear();

                        switch ((string)Session["SearchType"]) {
                            case "SearchEx":
                                //「検索」ボタンにて検索した場合
                                return View((new SkillSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                            case "Primary":
                                //「一次判定」ボタンにて検索した場合
                                //2017-08-31 iwai-tamura upd-str ------
                                return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                                //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], true));
                                //2017-08-31 iwai-tamura upd-end ------
                            case "Secondary":
                                //「二次判定」ボタンにて検索した場合
                                //2017-08-31 iwai-tamura upd-str ------
                                return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                                //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], false));
                                //2017-08-31 iwai-tamura upd-end ------
                            //2017-08-31 iwai-tamura upd-str ------
                            case "Department":
                                //「部門調整」ボタンにて検索した場合
                                return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));

                            case "Branch":
                                //「支社調整」ボタンにて検索した場合
                                return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                            //2017-08-31 iwai-tamura upd-end ------
                        
                        }
                    }
                    return View(model);
                }

                //対象選択エラーチェック
                if (selPrint == null) {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    switch ((string)Session["SearchType"]) {
                        case "SearchEx":
                            //「検索」ボタンにて検索した場合
                            return View((new SkillSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                        case "Primary":
                            //「一次判定」ボタンにて検索した場合
                            //2017-08-31 iwai-tamura upd-str ------
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"],(string)Session["SearchType"]));
                            //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], true));
                            //2017-08-31 iwai-tamura upd-end ------
                        case "Secondary":
                            //「二次判定」ボタンにて検索した場合
                            //2017-08-31 iwai-tamura upd-str ------
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"],(string)Session["SearchType"]));
                            //return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"], false));
                            //2017-08-31 iwai-tamura upd-end ------
                        //2017-08-31 iwai-tamura upd-str ------
                        case "Department":
                            //「部門調整」ボタンにて検索した場合
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"],(string)Session["SearchType"]));

                        case "Branch":
                            //「支社調整」ボタンにて検索した場合
                            return View((new SkillSearchBL()).SearchEx(model, (LoginUser)Session["LoginUser"],(string)Session["SearchType"]));
                        //2017-08-31 iwai-tamura upd-end ------
                    
                    }
                    return View(model);
                }

                //帳票作成ディレクトリを取得
                string fullPath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile()));

                //帳票出力ロジックを実行
                SkillSearchPrintBL bl = new SkillSearchPrintBL(fullPath);
                //model.Down.DownloadPath = bl.Print(selPrint);
                //model.Down.DownloadFlag = true;

                ////表示
                //return View((new SkillSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                //DL処理変更
                var dlpath = bl.PrintXls(selPrint);
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
        //2017-03-31 sbc-sagara add end 一括Excel出力

        //2017-04-30 sbc-sagara add str 一括入力
        /// <summary>
        /// 一括登録
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "Regist")]
        public ActionResult Regist(SkillSearchViewModels model, string[] selPrint)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");


                //一時保存データ破棄
                Session["selID"] = null;

                SkillSearchBL bl = new SkillSearchBL();

                //対象選択エラーチェック
                // 未選択
                if (selPrint == null) {
                    //エラー判定
                    ModelState.AddModelError("", "登録対象を選択してください。");
                    
                    //職能用のリスト生成
                    model.SkillClassItems = SkillSearchBL.GetSkillClass();

                    switch ((string)Session["SearchType"]) {
                        case "SearchEx":
                            //「検索」ボタンにて検索した場合
                            return View(bl.Search(model, (LoginUser)Session["LoginUser"]));
                        case "Primary":
                            //「一次判定」ボタンにて検索した場合
                            //2017-08-31 iwai-tamura upd-str ------
                            return View(bl.SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                            //return View(bl.SearchEx(model, (LoginUser)Session["LoginUser"], true));
                            //2017-08-31 iwai-tamura upd-end ------
                        case "Secondary":
                            //「二次判定」ボタンにて検索した場合
                            //2017-08-31 iwai-tamura upd-str ------
                            return View(bl.SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                            //return View(bl.SearchEx(model, (LoginUser)Session["LoginUser"], false));
                            //2017-08-31 iwai-tamura upd-end ------
                        //2017-08-31 iwai-tamura upd-str ------
                        case "Department":
                            //「部門調整」ボタンにて検索した場合
                            return View(bl.SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));

                        case "Branch":
                            //「支社調整」ボタンにて検索した場合
                            return View(bl.SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                        //2017-08-31 iwai-tamura upd-end ------
                    
                    }
                    return View(model);
                }
                // 支社調整入力不可レコードを選択
                if (!bl.AdjustStatusCheck(selPrint, (LoginUser)Session["LoginUser"])) {
                    TempData["Confirmation"] = string.Format("入力対象外の対象が選択されています。");

                    //職能用のリスト生成
                    model.SkillClassItems = SkillSearchBL.GetSkillClass();

                    switch ((string)Session["SearchType"]) {
                        case "SearchEx":
                            //「検索」ボタンにて検索した場合
                            return View(bl.Search(model, (LoginUser)Session["LoginUser"]));
                        case "Primary":
                            //「一次判定」ボタンにて検索した場合
                            //2017-08-31 iwai-tamura upd-str ------
                            return View(bl.SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                            //return View(bl.SearchEx(model, (LoginUser)Session["LoginUser"], true));
                            //2017-08-31 iwai-tamura upd-end ------
                        case "Secondary":
                            //「二次判定」ボタンにて検索した場合
                            //2017-08-31 iwai-tamura upd-str ------
                            return View(bl.SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                            //return View(bl.SearchEx(model, (LoginUser)Session["LoginUser"], false));
                            //2017-08-31 iwai-tamura upd-end ------

                        //2017-08-31 iwai-tamura upd-str ------
                        case "Department":
                            //「部門調整」ボタンにて検索した場合
                            return View(bl.SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));

                        case "Branch":
                            //「支社調整」ボタンにて検索した場合
                            return View(bl.SearchEx(model, (LoginUser)Session["LoginUser"], (string)Session["SearchType"]));
                        //2017-08-31 iwai-tamura upd-end ------
                    
                    }
                    return View(model);
                }
                
                //未選択エラー等複数回使う可能性があるのでSessionに選択値一時保存
                Session["selID"] = selPrint;

                //検索状態保存
                TempData["SkillSearch"] = model.Search;

                //表示
                return RedirectToAction("Index", "SkillAssessmentChoice");
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
        //2017-04-30 sbc-sagara add end 一括入力

        /// <summary>
        /// コンフィグから帳票出力フォルダ取得
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private string GetTempDir(Configuration config) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                return FileUtil.GetTempDir(config, "DOWNLOAD_TEMP_DIR_S");

                ////末尾がpathの区切り文字かしらべ、違っていたら追加する。
                //string tempDir = config.AppSettings.Settings["DOWNLOAD_TEMP_DIR_S"].Value;
                //if(!(tempDir.EndsWith(Path.DirectorySeparatorChar.ToString()))) {
                //    tempDir += Path.DirectorySeparatorChar;
                //}

                //return tempDir;
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
