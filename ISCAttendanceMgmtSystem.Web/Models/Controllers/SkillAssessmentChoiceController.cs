using SkillDiscriminantSystem.BL.Login;
using SkillDiscriminantSystem.BL.SkillAssessmentChoice;
using SkillDiscriminantSystem.COM.Models;
using SkillDiscriminantSystem.COM.Util.Controll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkillDiscriminantSystem.Web.Controllers
{
    public class SkillAssessmentChoiceController : Controller
    {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        
        // GET: SkillAssessmentChoice
        /// <summary>
        /// 入力画面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Index(SkillAssessmentChoiceViewModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                SkillAssessmentChoiceBL bl = new SkillAssessmentChoiceBL();
                model = bl.CreateBranchAdjustment(model);

                List<string> IDList = new List<string>();
                if (Session["selID"] != null && Session["selID"].GetType().Name == "String[]")
                {
                    // 一括入力用一時保存データ有
                    string[] tmpID = (string[])(Session["selID"]);
                    foreach (string trg in tmpID)
                    {
                        IDList.Add(trg);
                    }
                }

                if (IDList.Count > 0) {
                    model.ResultItems = bl.Search(IDList);
                    return View(model);
                }

                // 選択値無
                return RedirectToAction("Search", "SkillSearch");
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


        /// <summary>
        /// 登録
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index")]
        [AcceptButton(ButtonName = "DoRegist")]
        public ActionResult DoRegist(SkillAssessmentChoiceViewModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                SkillAssessmentChoiceBL bl = new SkillAssessmentChoiceBL();

                SkillAssessmentChoiceViewModels checkModel = new SkillAssessmentChoiceViewModels(); // チェックされたアイテムのみ抽出
                List<AssessmentResultItems> List = new List<AssessmentResultItems>();
                foreach (AssessmentResultItems Item in model.ResultItems)
                {
                    if (Item.Selected == true)
                    {
                        List.Add(Item);
                    }
                }
                checkModel.ResultItems = List;

                //対象選択エラーチェック
                if (checkModel.ResultItems.Count == 0)
                {
                    //エラー判定
                    ModelState.AddModelError("", "登録対象を選択してください。");

                    model = bl.CreateBranchAdjustment(model);


                    List<string> IDList = new List<string>();
                    if (Session["selID"] != null && Session["selID"].GetType().Name == "String[]")
                    {
                        // 一括入力用一時保存データ有
                        string[] tmpID = (string[])(Session["selID"]);
                        foreach (string trg in tmpID)
                        {
                            IDList.Add(trg);
                        }
                    }

                    if (IDList.Count > 0)
                    {
                        model.ResultItems = bl.Search(IDList);

                        return View(model);
                    }

                    return RedirectToAction("Search", "SkillSearch");
                }

                //登録処理
                bl.Regist(checkModel);

                //セッション情報破棄
                Session["selID"] = null;

                //結果
                TempData["Success"] = "登録しました";

                return RedirectToAction("Search", "SkillSearch");
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
        
        /// <summary>
        /// 戻る
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index")]
        [AcceptButton(ButtonName = "Back")]
        public ActionResult Back()
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //トップへ
                return RedirectToAction("Search", "SkillSearch");
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