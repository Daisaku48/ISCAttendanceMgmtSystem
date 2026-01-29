using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SkillDiscriminantSystem.BL.ApproverSearch;
using SkillDiscriminantSystem.COM.Models;
using SkillDiscriminantSystem.COM.Util.Controll;
using SkillDiscriminantSystem.COM.Util.Database;
using System.IO;
using SkillDiscriminantSystem.BL.Login;
using System.Web.Services;
using SkillDiscriminantSystem.COM.Util.Config;
using System.Configuration;
using SkillDiscriminantSystem.COM.Entity.Session;
using SkillDiscriminantSystem.COM.Util.Convert;

namespace SkillDiscriminantSystem.Web.Controllers {
    public class ApproverSearchController : Controller {
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
                    //2016-01-21 iwai-tamura upd str -----
                    //一般職の場合は利用不可
                    
                    //2016-04-14 iwai-tamura upd str -----
                    if (!lu.IsPost)
                    {
                        TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                        return RedirectToAction("Index", "Top");
                    }
                    //if (!lu.IsPost) return RedirectToAction("Index", "Top");
                    //2016-04-14 iwai-tamura upd end -----

                    //if (DataConv.IntParse(lu.PostNo, 999) > 500) return RedirectToAction("Index", "Top");
                    //2016-01-21 iwai-tamura upd end -----
                }

                //初期化
                ApproverSearchViewModels model = new ApproverSearchViewModels {
                    Search = new ApproverSearchModel()
                    ,SearchResult = new List<ApproverSearchListModel>()
                    //,Down = new ApproverDownLoadModel()
                };

                //TempDataある場合
                if(TempData["ApproverSearch"] != null) {
                    model.Search = (ApproverSearchModel)TempData["ApproverSearch"];

                    ModelState.Clear();
                    //2016-01-21 iwai-tamura upd str -----
                    //詳細設定画面から戻った時に前回の検索結果データを表示する(部下表示に対応)
                    if ((string)Session["SearchType"] == "Sub")
                    {
                        //「部下表示」ボタンにて検索した場合
                        return View((new ApproverSearchBL()).Search(model, (LoginUser)Session["LoginUser"], "Sub"));
                    }else{
                        //「検索」ボタンにて検索した場合
                        return View((new ApproverSearchBL()).Search(model, (LoginUser)Session["LoginUser"], "Main"));
                    }
                    //return View((new ApproverSearchBL()).Search(model, (LoginUser)Session["LoginUser"], "Main"));
                    //2016-01-21 iwai-tamura upd end -----
                }

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
        public ActionResult SearchEx(ApproverSearchViewModels model, string value) {
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
                return View((new ApproverSearchBL()).Search(model, (LoginUser)Session["LoginUser"], "Main"));

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
        public ActionResult SubView(ApproverSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //TODO:ロジックは次の段階で

                //2016-01-21 iwai-tamura add str -----
                //検索時、検索方法を保存するよう変更
                Session["SearchType"] = "Sub";
                //2016-01-21 iwai-tamura add end -----
                
                //表示
                return View((new ApproverSearchBL()).Search(model, (LoginUser)Session["LoginUser"], "Sub"));

                

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
        public ActionResult Back(ApproverSearchViewModels model, string value) {
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
        /// 表示
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "View")]
        //[AcceptButton(ButtonName = "View")]
        public ActionResult ViewRedirect(ApproverSearchViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");
                //検索条件セット
                TempData["ApproverSearch"] = model.Search;
                //詳細へ
                var val = value.Split(',');
                return RedirectToAction("Index", "ApproverSetting", new { year = val[0], empNum = val[1],depart = val[2] });
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