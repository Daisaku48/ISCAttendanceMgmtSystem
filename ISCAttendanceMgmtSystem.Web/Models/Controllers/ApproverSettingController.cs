using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using SkillDiscriminantSystem.COM.Models;
using SkillDiscriminantSystem.BL.Login;
using SkillDiscriminantSystem.BL.ApproverSetting;
using SkillDiscriminantSystem.COM.Entity.Session;
using SkillDiscriminantSystem.COM.Util.Controll;

namespace SkillDiscriminantSystem.Web.Controllers {
    public class ApproverSettingController : Controller {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 初期表示
        /// </summary>
        /// <param name="year">一覧の年度</param>
        /// <param name="empNum">一覧の社員番号</param>
        /// <param name="depart">一覧の所属番号</param>
        /// <returns></returns>
        public ActionResult Index(string year, string empNum, string depart) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッション情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];
                //return View(new ApproverSettingBL().Select(year, empNum, depart, lu));
                var model = new ApproverSettingBL().Select(year, empNum, depart, lu);

                //TempDataある場合
                if(TempData["ApproverSearch"] != null) {
                    model.Search = (ApproverSearchModel)TempData["ApproverSearch"];
                }

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
        /// 設定
        /// </summary>
        /// <param name="model"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index")]
        [ButtonHandler(ButtonName = "ViewEx")]
        //[AcceptButton(ButtonName = "View")]
        public ActionResult ViewRedirect(ApproverSettingViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッション情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];
                var bl = new ApproverSettingBL();

                //決裁権限設定
                //検索に必要な項目を抜き出す
                string year = model.Head.Year;
                string empNo = model.Head.EmployeeNo;
                string departNo = model.Head.DepartmentNo;

                //必要な項目の退避
                List<ApproverDetail> saveList = new List<ApproverDetail>();
                saveList = bl.ToSaveList(model);
                //検索条件退避
                var search = model.Search;

                //モデルクリア
                ModelState.Clear();

                //社員番号存在確認(個別入力優先)
                string keyEmpNum = "";
                if(!string.IsNullOrEmpty(model.Detail[int.Parse(value)].SetSearch.Select)) {
                    keyEmpNum = model.Detail[int.Parse(value)].SetSearch.Select;
                }
                if(!string.IsNullOrEmpty(model.Detail[int.Parse(value)].SetSearch.DirectEmpNum)) {
                    keyEmpNum = model.Detail[int.Parse(value)].SetSearch.DirectEmpNum;
                }
                //2016-10-14 iwai-tamura upd str ------
                //目標と職能で機能を分けるため引数に追加
                if(!bl.CheckEmpNum(keyEmpNum, model.Head.Year,int.Parse(value))) {
                //if(!bl.CheckEmpNum(keyEmpNum, model.Head.Year)) {
                //2016-10-14 iwai-tamura upd end ------
                    TempData["Success"] = "権限者社員情報を取得できませんでした。";
                    model = bl.Select(model.Head.Year, model.Head.EmployeeNo, model.Head.DepartmentNo, lu);

                    //退避項目を全てそのまま復活
                    for(int i = 0; i < saveList.Count; i++) {
                        //退避項目の復活
                        model.Detail[i].Area1 = saveList[i].Area1;                       //大区分
                        model.Detail[i].Area2 = saveList[i].Area2;                       //中区分
                        model.Detail[i].Area3 = saveList[i].Area3;                       //小区分
                        model.Detail[i].DispNameDepart = saveList[i].DispNameDepart;     //画面表示の権限者情報
                        model.Detail[i].RightEmpNo = saveList[i].RightEmpNo;             //権限社員番号
                        model.Detail[i].RightDepartment = saveList[i].RightDepartment;   //権限者所属部署名
                        model.Detail[i].CustomFlag = saveList[i].CustomFlag;             //設定ボタン押下フラグ
                        model.Detail[i].DispNameDepart = saveList[i].DispNameDepart;     //画面表示権限者情報
                    }
                    return View(model);
                }

                //権限者取得し初期表示の結果に追加する
                model = bl.Set(year, empNo, departNo, keyEmpNum, value, lu);

                //退避項目の復帰
                model = bl.RetFromSave(model, saveList, value);
                model.Search = search;

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
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index")]
        [AcceptButton(ButtonName = "Save")]
        public ActionResult Save(ApproverSettingViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッション情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];
                var bl = new ApproverSettingBL();

                //保存と検索に必要な項目を抜き出す
                string year = model.Head.Year;
                string empNo = model.Head.EmployeeNo;
                string departNo = model.Head.DepartmentNo;

                //検索条件退避
                var search = model.Search;

                //モデルクリア(このあとキー項目全部参照できるか)
                ModelState.Clear();

                //保存したものをSelect(全部Falseになっているか)
                model = bl.DoSave(model, year, empNo, departNo, lu);
                model.Search = search;

                TempData["Success"] = "完了しました。";

                //トップへ
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
        /// 戻る
        /// </summary>
        /// <param name="model"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index")]
        [AcceptButton(ButtonName = "Back")]
        public ActionResult Back(ApproverSettingViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //検索条件セット
                TempData["ApproverSearch"] = model.Search;

                //検索トップへ
                return RedirectToAction("Search", "ApproverSearch");
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
        /// クリア
        /// </summary>
        /// <param name="model"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index")]
        [ButtonHandler(ButtonName = "ClearEx")]
        //[AcceptButton(ButtonName = "View")]
        public ActionResult ClearRedirect(ApproverSettingViewModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //セッション情報取得
                LoginUser lu = (LoginUser)Session["LoginUser"];
                var bl = new ApproverSettingBL();

                //決裁権限設定
                //検索に必要な項目を抜き出す
                string year = model.Head.Year;
                string empNo = model.Head.EmployeeNo;
                string departNo = model.Head.DepartmentNo;

                //必要な項目の退避
                List<ApproverDetail> saveList = new List<ApproverDetail>();
                saveList = bl.ToSaveList(model);
                //検索条件退避
                var search = model.Search;

                //モデルクリア
                ModelState.Clear();

                ////権限者取得し初期表示の結果に追加する
                model = bl.Set(year, empNo, departNo, value, lu);

                //退避した項目の復帰
                for(int i = 0; i < saveList.Count; i++) {
                    if(saveList[i].CustomFlag && i != int.Parse(value)) {            //設定押下フラグ = true かつ直前に設定ボタンが押された項目でない場合は復活
                        //退避項目の復活
                        model.Detail[i].Area1 = saveList[i].Area1;                       //大区分
                        model.Detail[i].Area2 = saveList[i].Area2;                       //中区分
                        model.Detail[i].Area3 = saveList[i].Area3;                       //小区分
                        model.Detail[i].DispNameDepart = saveList[i].DispNameDepart;     //画面表示の権限者情報
                        model.Detail[i].RightEmpNo = saveList[i].RightEmpNo;             //権限社員番号
                        model.Detail[i].RightDepartment = saveList[i].RightDepartment;   //権限者所属部署名
                        model.Detail[i].CustomFlag = saveList[i].CustomFlag;             //設定ボタン押下フラグ
                        model.Detail[i].DispNameDepart = saveList[i].DispNameDepart;     //画面表示権限者情報
                    }
                }
                model.Search = search;

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
    }
}