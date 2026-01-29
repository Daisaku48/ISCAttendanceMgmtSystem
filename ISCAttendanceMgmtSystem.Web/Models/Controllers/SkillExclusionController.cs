using SkillDiscriminantSystem.BL.Login;
using SkillDiscriminantSystem.BL.SkillExclusion;
using SkillDiscriminantSystem.COM.Models;
using SkillDiscriminantSystem.COM.Util.Controll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkillDiscriminantSystem.Web.Controllers
{
    public class SkillExclusionController : Controller
    {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        // GET: SkillExclusion
        public ActionResult Search(SkillExclusionModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //BL
                SkillExclusionBL bl = new SkillExclusionBL();
                //処理区分ドロップダウンリストアイテム取得
                model.ExecuteTypeItems = bl.GetCreateExecuteType();
                //判定除外区分ドロップダウンリストアイテム取得
                model.ExclusionTypeItems = bl.GetCreateExclusionType();
                ////社員番号ドロップダウンリストアイテム取得
                //model.EmployeeNoItems = bl.GetCreateEmployeeNo();
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

        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "DoCommit")]
        public ActionResult DoCommit(SkillExclusionModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //BL
                SkillExclusionBL bl = new SkillExclusionBL();
                //処理区分ドロップダウンリストアイテム取得
                model.ExecuteTypeItems = bl.GetCreateExecuteType();
                //判定除外区分ドロップダウンリストアイテム取得
                model.ExclusionTypeItems = bl.GetCreateExclusionType();
                //社員番号ドロップダウンリストアイテム取得
                //model.EmployeeNoItems = bl.GetCreateEmployeeNo();

                if (!bl.EmployeeExsistCheck(model))
                {
                    TempData["Confirmation"] = string.Format("無効な社員番号です。");
                    return View(model);
                }

                model.ProvisionValue = bl.GetProvisionValue(model);
                ResultItems RI = new ResultItems();
                RI = bl.GetResult(model);

                if (model.Search.ExecuteType == "1" && RI.YMD1 != null)
                {
                    //登録かつデータ有
                    TempData["Confirmation"] = string.Format("除外判定既登録の社員番号です。");
                    return View(model);
                }
                if (model.Search.ExecuteType != "1" && RI.YMD1 == null)
                {
                    //修正or削除かつデータ無
                    TempData["Confirmation"] = string.Format("除外判定未登録の社員番号です。");
                    return View(model);
                }

                model.Result = RI;
                model.Result = bl.ShowResult(model);

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

        /// <summary>
        /// 結果表示
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "ShowResult")]
        public ActionResult ShowResult(SkillExclusionModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //BL
                SkillExclusionBL bl = new SkillExclusionBL();
                //処理区分ドロップダウンリストアイテム取得
                model.ExecuteTypeItems = bl.GetCreateExecuteType();
                //判定除外区分ドロップダウンリストアイテム取得
                model.ExclusionTypeItems = bl.GetCreateExclusionType();
                //社員番号ドロップダウンリストアイテム取得
                //model.EmployeeNoItems = bl.GetCreateEmployeeNo();

                model.Result = bl.ShowResult(model);

                return View(model);
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
        /// 確定(登録/修正/削除)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "DoExecute")]
        public ActionResult DoExecute(SkillExclusionModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //BL
                SkillExclusionBL bl = new SkillExclusionBL();
                //処理区分ドロップダウンリストアイテム取得
                model.ExecuteTypeItems = bl.GetCreateExecuteType();
                //判定除外区分ドロップダウンリストアイテム取得
                model.ExclusionTypeItems = bl.GetCreateExclusionType();
                //社員番号ドロップダウンリストアイテム取得
                //model.EmployeeNoItems = bl.GetCreateEmployeeNo();

                model.Result = bl.ShowResult(model);

                switch (model.Search.ExecuteType)
                {
                    case "1":
                        //登録処理
                        if (bl.Insert(model))
                        {
                            TempData["Success"] = string.Format("除外判定登録に成功しました。");
                            //成功時各データ破棄
                            model.Search = null;
                            model.ProvisionValue = null;
                            model.Result = null;
                        }
                        else
                        {
                            TempData["Confirmation"] = string.Format("除外判定登録に失敗しました。");
                        }
                        break;
                    case "2":
                        //修正処理
                        if (bl.Update(model))
                        {
                            TempData["Success"] = string.Format("除外判定修正に成功しました。");
                            //成功時各データ破棄
                            model.Search = null;
                            model.ProvisionValue = null;
                            model.Result = null;
                        }
                        else
                        {
                            TempData["Confirmation"] = string.Format("除外判定修正に失敗しました。");
                        }
                        break;
                    case "3":
                        //削除処理
                        if (bl.Delete(model))
                        {
                            TempData["Success"] = string.Format("除外判定削除に成功しました。");
                            //成功時各データ破棄
                            model.Search = null;
                            model.ProvisionValue = null;
                            model.Result = null;
                        }
                        else
                        {
                            TempData["Confirmation"] = string.Format("除外判定削除に失敗しました。");
                        }
                        break;
                }

                return View(model);
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
        /// 取消
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "Cancel")]
        public ActionResult Cancel(SkillExclusionModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //BL
                SkillExclusionBL bl = new SkillExclusionBL();
                //処理区分ドロップダウンリストアイテム取得
                model.ExecuteTypeItems = bl.GetCreateExecuteType();
                //判定除外区分ドロップダウンリストアイテム取得
                model.ExclusionTypeItems = bl.GetCreateExclusionType();
                //社員番号ドロップダウンリストアイテム取得
                //model.EmployeeNoItems = bl.GetCreateEmployeeNo();

                model.Search = null;
                model.ProvisionValue = null;
                model.Result = null;

                return View(model);
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
        [ActionName("Search")]
        [ButtonHandler(ButtonName = "Back")]
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
    }
}
