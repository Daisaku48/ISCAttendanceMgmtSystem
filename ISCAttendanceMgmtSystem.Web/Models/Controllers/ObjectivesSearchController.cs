using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SkillDiscriminantSystem.BL.ObjectivesSearch;
using SkillDiscriminantSystem.COM.Models;
using SkillDiscriminantSystem.COM.Util.Controll;
using SkillDiscriminantSystem.COM.Util.Database;
using System.IO;
using SkillDiscriminantSystem.BL.Login;
using System.Web.Services;
using SkillDiscriminantSystem.COM.Util.Config;
using System.Configuration;
using SkillDiscriminantSystem.COM.Entity.Session;
using SkillDiscriminantSystem.COM.Util.File;

namespace SkillDiscriminantSystem.Web.Controllers {
    /// <summary>
    /// 目標管理検索コントローラー
    /// </summary>
    public class ObjectivesSearchController : Controller {

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
                //2016-04-14 iwai-tamura upd str -----
                //if (!(new LoginBL()).IsLogin()){
                //    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                //    return RedirectToAction("Login", "Login");
                //}
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");
                //2016-04-14 iwai-tamura upd end -----

                //初期化
                ObjectivesSearchViewModels model = new ObjectivesSearchViewModels {
                    Search = new ObjectivesSearchModel(),
                    SearchResult = new List<ObjectivesSearchListModel>(),
                    Down = new ObjectivesDownLoadModel()
                };

                //2016-01-21 iwai-tamura upd str -----
                //TempDataある場合
                if (TempData["ObjectivesSearch"] != null)
                {
                    //詳細設定画面から戻った時に前回の検索結果データを表示する
                    model.Search = (ObjectivesSearchModel)TempData["ObjectivesSearch"];
                    ModelState.Clear();
                    if ((string)Session["SearchType"] == "Sub")
                    {
                        //「部下表示」ボタンにて検索した場合
                        return View((new ObjectivesSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }else{
                        //「検索」ボタンにて検索した場合
                        return View((new ObjectivesSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    }
                }
                // 2017-03-31 sbc-sagara add str
                else
                {
                    Session["SearchType"] = "";
                }
                // 2017-03-31 sbc-sagara add end
                //2016-01-21 iwai-tamura upd end -----

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
        public ActionResult SearchEx(ObjectivesSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //2016-01-21 iwai-tamura add str -----
                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "Main";
                //2016-01-21 iwai-tamura add end -----
                
                //表示
                return View((new ObjectivesSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                
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
        /// 本人入力
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "SelfInput")]
        public ActionResult SelfInput(ObjectivesSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //2016-01-21 iwai-tamura add str -----
                //検索条件セット
                TempData["ObjectivesSearch"] = model.Search;
                //2016-01-21 iwai-tamura add end -----


                //入力画面へ
                int? id = (new ObjectivesSearchBL()).GetManageNo((LoginUser)Session["LoginUser"]);
                if(id != null) {
                    return RedirectToAction("Index", "ObjectivesManagement", new { id = id, tableType = "G" });
                }
                //表示
                return View((new ObjectivesSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
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
        /// 部下表示
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [AcceptButton(ButtonName = "SubView")]
        public ActionResult SubView(ObjectivesSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //2016-01-21 iwai-tamura add str -----
                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "Sub";
                //2016-01-21 iwai-tamura add end -----

                //表示
                return View((new ObjectivesSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                
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
        public ActionResult ViewRedirect(ObjectivesSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //2016-01-21 iwai-tamura add str -----
                //検索条件セット
                TempData["ObjectivesSearch"] = model.Search;
                //2016-01-21 iwai-tamura add end -----

                //詳細へ
                var val = value.Split(',');
                return RedirectToAction("Index", "ObjectivesManagement", new { id = val[0], tableType = val[1] });
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
                ObjectivesSearchPrintBL bl = new ObjectivesSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
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
        public ActionResult Back(ObjectivesSearchViewModels model, string value) {
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
        /// 帳票作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "Print")]
        public ActionResult Print(ObjectivesSearchViewModels model, string[] selPrint) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //対象選択エラーチェック
                if(selPrint == null) {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    // 2017-03-31 sbc-sagara add str 選択エラー時検索結果再表示対応
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new ObjectivesSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new ObjectivesSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    // 2017-03-31 sbc-sagara add end 選択エラー時検索結果再表示対応
                    return View("Search", model);
                }
                
                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                ObjectivesSearchPrintBL bl = new ObjectivesSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));
                //結果
                //model.Down.DownloadPath = bl.Print(selPrint);
                //model.Down.DownloadFlag = string.IsNullOrEmpty(model.Down.DownloadPath) ? false : true;
                ////表示
                //return View((new ObjectivesSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));

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
        public ActionResult Sign(ObjectivesSearchViewModels model, string[] selPrint) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //対象選択エラーチェック
                if(selPrint == null) {
                    //エラー判定
                    ModelState.AddModelError("", "承認対象を選択してください。");
                    // 2017-03-31 sbc-sagara add str
                    if ((string)Session["SearchType"] == "Sub")
                    {
                        //「部下表示」ボタンにて検索した場合
                        return View(new ObjectivesSearchBL().SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    else
                    {
                        //「検索」ボタンにて検索した場合
                        return View(new ObjectivesSearchBL().Search(model, (LoginUser)Session["LoginUser"]));
                    }
                    // 2017-03-31 sbc-sagara add end
                    return View("Search", model);
                }

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                ObjectivesSearchBL bl = new ObjectivesSearchBL();

                //2016-01-21 iwai-tamura upd end -----
                int cntTaget = selPrint.Length;                                         //選択した件数
                int cntUpdate = bl.Sign(selPrint, (LoginUser)Session["LoginUser"]);     //承認処理：承認した件数を取得
                //bl.Sign(selPrint,(LoginUser)Session["LoginUser"]);
                //2016-01-21 iwai-tamura upd end -----                

                //結果
                //model.Down.DownloadFlag = true;
                //2016-01-21 iwai-tamura upd str -----
                //承認結果を表示するよう変更
                TempData["Success"] = string.Format("{0}名中{1}名承認しました", cntTaget, cntUpdate);
                //TempData["Success"] = "承認しました";
                //2016-01-21 iwai-tamura upd end -----                

                //表示
                //2016-01-21 iwai-tamura upd str -----
                if ((string)Session["SearchType"] == "Sub")
                {
                    //「部下表示」ボタンにて検索した場合
                    return View(bl.SubSearch(model, (LoginUser)Session["LoginUser"]));
                }
                else
                {
                    //「検索」ボタンにて検索した場合
                    return View(bl.Search(model, (LoginUser)Session["LoginUser"]));
                }
                //return View(bl.Search(model, (LoginUser)Session["LoginUser"]));
                //2016-01-21 iwai-tamura upd end -----                
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

        //2017-03-31 sbc-sagara add str 一括Excel出力ボタン追加
        /// <summary>
        /// Excel作成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        //[ButtonHandler(ButtonName = "Print")]
        [AcceptButton(ButtonName = "PrintXls")]
        public ActionResult PrintXls(ObjectivesSearchViewModels model, string[] selPrint)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                
                //対象選択エラーチェック
                if (selPrint == null)
                {
                    //エラー判定
                    ModelState.AddModelError("", "出力対象を選択してください。");
                    
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new ObjectivesSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new ObjectivesSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);
                }
                
                // 2017-03-31 sbc-sagara add str 支社管理機能の追加
                //管理者判定
                if (((LoginUser)Session["LoginUser"]).目標DATA出力 != "1")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    if ((string)Session["SearchType"] == "Main") {
                        return View("Search", (new ObjectivesSearchBL()).Search(model, (LoginUser)Session["LoginUser"]));
                    } else if ((string)Session["SearchType"] == "Sub") {
                        return View("Search", (new ObjectivesSearchBL()).SubSearch(model, (LoginUser)Session["LoginUser"]));
                    }
                    return View("Search", model);                }
                // 2017-03-31 sbc-sagara add end 支社管理機能の追加

                //帳票出力ロジックを実行
                //帳票作成ディレクトリを取得
                ObjectivesSearchPrintBL bl = new ObjectivesSearchPrintBL(Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile())));

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
        //2017-03-31 sbc-sagara add end 一括Excel出力ボタン追加

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
