using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SkillDiscriminantSystem.BL.Login;
using SkillDiscriminantSystem.COM.Util.Controll;
using SkillDiscriminantSystem.COM.Models;
using SkillDiscriminantSystem.BL.SystemManagement;
using SkillDiscriminantSystem.COM.Entity.Session;
using SkillDiscriminantSystem.COM.Util.Config;
using SkillDiscriminantSystem.COM.Util.File;
// 2017-04-30 sbc-sagara add str Excel出力
using System.Configuration;
using System.IO;
// 2017-04-30 sbc-sagara add end Excel出力


namespace SkillDiscriminantSystem.Web.Controllers {
    public class SystemManagementController : Controller {

        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 目標管理の種別
        /// </summary>
        private const string objective = "Objective";
        private const string skill = "Skill";
        
        /// <summary>
        /// 初期表示
        /// </summary>
        /// <returns></returns>
        public ActionResult Index() {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                //2016-04-14 iwai-tamura upd str -----
                
                // 2017-03-31 sbc-sagara upd str 支社管理機能の追加
                //if (!((LoginUser)Session["LoginUser"]).IsRoot)
                if (((LoginUser)Session["LoginUser"]).SYS管理MENU != "1")
                // 2017-03-31 sbc-sagara upd end 支社管理機能の追加
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    return RedirectToAction("Index");
                }
                //if (!((LoginUser)Session["LoginUser"]).IsRoot) return RedirectToAction("Index", "Top");
                //2016-04-14 iwai-tamura upd end -----

                TempData["Success"] = null;


                //セッションから権限情報取得
                LoginUser pm = (LoginUser)Session["Permission"];


                //お知らせ情報を検索して表示
                SystemManagementModels model = new SystemManagementModels();
                return View(new InfoManagementBL().SearchInfo());
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
        /// 一括作成処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [ButtonHandler(ButtonName = "DoBulk")]
        //[AcceptButton(ButtonName = "DoBulk")]
        public ActionResult DoBulk(SystemManagementModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                // 2017-03-31 sbc-sagara upd str 支社管理機能の追加
                //if (!((LoginUser)Session["LoginUser"]).IsRoot) return RedirectToAction("Index", "Top");
                if (((LoginUser)Session["LoginUser"]).SYS管理初期処理 != "1")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    return RedirectToAction("Index");
                }
                // 2017-03-31 sbc-sagara upd end 支社管理機能の追加

                string message = "";
                
                if(model.BulkType == objective) {
                    //目標管理一括作成
                    message = new ObjectivesCreateBL().CreateObjective(model, value, (LoginUser)Session["LoginUser"]);
                } else {

                    // 2017-10-31 iwai-tamura add str Accessシステムの処理状態確認
                    //職能判定の場合、処理状態を確認
                    SkillCreateBL bl = new SkillCreateBL();
                    message = bl.GetSkillStatus(model.BulkYear,model.BulkDuration);
                    if (message != "") {
                        TempData["Confirmation"] = message;
                        return View("Index");
                    }
                    // 2017-10-31 iwai-tamura add end Accessシステムの処理状態確認
                    
                    //職能職務判定表一括作成
                    message = new SkillCreateBL().CreateSkill(model, value, (LoginUser)Session["LoginUser"]);
                }


                //完了メッセージ設定
                TempData["Success"] = message;
                return View("Index");
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
        /// 個別作成処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [ButtonHandler(ButtonName = "DoDesignate")]
        //[AcceptButton(ButtonName = "DoDesignate")]
        public ActionResult DoDesignate(SystemManagementModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                // 2017-03-31 sbc-sagara upd str 支社管理機能の追加
                //if(!((LoginUser)Session["LoginUser"]).IsRoot) return RedirectToAction("Index", "Top");
                if (((LoginUser)Session["LoginUser"]).SYS管理初期処理 != "1")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    return RedirectToAction("Index");
                }
                // 2017-03-31 sbc-sagara upd end 支社管理機能の追加



                string message = "";
                // TODO:エラーチェック検討中
                if(model.DesignateType == objective) {
                    //目標管理一括作成
                    message = new ObjectivesCreateBL().CreateObjective(model, value, (LoginUser)Session["LoginUser"]);
                } else {
                    // 2017-10-31 iwai-tamura add str Accessシステムの処理状態確認
                    //職能判定の場合、処理状態を確認
                    SkillCreateBL bl = new SkillCreateBL();
                    message = bl.GetSkillStatus(model.DesignateYaer,model.DesignateDuration);
                    if (message != "") {
                        TempData["Confirmation"] = message;
                        return View("Index");
                    }
                    // 2017-10-31 iwai-tamura add end Accessシステムの処理状態確認

                    //職能職務判定表一括作成
                    message = new SkillCreateBL().CreateSkill(model, value, (LoginUser)Session["LoginUser"]);
                }


                //完了メッセージ設定
                TempData["Success"] = message;
                return View("Index", model);
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

        // 2018-03-20 iwai-tamura upd str ------
        /// <summary>
        /// 異動によるデータ引継ぎ処理_上書き確認後処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [ButtonHandler(ButtonName = "DoTakeOverMoveSingle_Reference")]
        public ActionResult DoTakeOverMoveSingle_Reference(SystemManagementModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                TakeOverMoveBL bl = new TakeOverMoveBL();
                // 目標管理引継ぎ処理
                if (bl.ObjectiveTakeOverMoveSingle(model)) {
                    TempData["Confirmation"] = "引継ぎが完了しました。";
                } else {
                    TempData["Confirmation"] = "引継ぎに失敗しました。";
                }
                return View("Index");
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
        /// 異動によるデータ引継ぎ処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [ButtonHandler(ButtonName = "DoTakeOverMoveSingle")]
        public ActionResult DoTakeOverMoveSingle(SystemManagementModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                if (((LoginUser)Session["LoginUser"]).SYS管理異動DATA引継処理 == "0")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    return RedirectToAction("Index");
                }

                TakeOverMoveBL bl = new TakeOverMoveBL();

                //引継ぎ処理
                switch (model.TakeOverMoveBulkType) { // 引継ぎ対象
                    case "Objective": // 目標管理
                        //前任者存在チェック
                        switch (bl.ObjectiveConfirm(model.TakeOverMoveSingleYaer,model.TakeOverMoveSingleBefEmployeeNo,model.TakeOverMoveSingleBefDepartment))
                        {
                            case "Nothing": //存在しない
                                TempData["Confirmation"] = "前任者のデータが存在しません。確認してください。";
                                return View("Index");
                            case "Committed": //確定済み
                                //問題なし
                                break;

                            case "Updated": //更新済み
                                //問題なし
                                break;

                            case "Exist":   //存在する
                                //問題なし
                                break;
                        }

                        //後任者存在チェック
                        switch (bl.ObjectiveConfirm(model.TakeOverMoveSingleYaer,model.TakeOverMoveSingleAftEmployeeNo,model.TakeOverMoveSingleAftDepartment))
                        {
                            case "Nothing": //存在しない
                                TempData["Confirmation"] = "後任者のデータが存在しません。確認してください。";
                                return View("Index");
                            case "Committed": //確定済み
                                TempData["Confirmation"] = "後任者のデータが既に確定済みです。確認してください。";
                                return View("Index");

                            case "Updated": //更新済み
                                TempData["TakeOverMoveReference"] = "後任者のデータが既に入力されています。<br />上書きしてよろしいですか？";
                                return View("Index");

                            case "Exist":   //存在する
                                //問題なし
                                break;
                        }            

                        // 目標管理引継ぎ処理
                        if (bl.ObjectiveTakeOverMoveSingle(model)) {
                            TempData["Confirmation"] = "引継ぎが完了しました。";
                        } else {
                            TempData["Confirmation"] = "引継ぎに失敗しました。";
                            break;
                        }
                        break;

                    case "Skill": // 職能判定
                            TempData["Confirmation"] = "職能判定はこの処理を行えません。確認してください。";
                            break;
                }

                return View("Index");
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
        /// 異動によるデータ引継ぎ一括処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [ButtonHandler(ButtonName = "DoTakeOverMoveBulk")]
        public ActionResult DoTakeOverMoveBulk(SystemManagementModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                if (((LoginUser)Session["LoginUser"]).SYS管理異動DATA引継処理 != "K")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    return RedirectToAction("Index");
                }

                string message = "";
                if(model.TakeOverMoveBulkType != objective) {
                    TempData["Confirmation"] = "この機能は目標管理しか行えません";
                    return View("Index");
                }

                if (model.TakeOverMoveBulkUploadFile == null) {
                    TempData["Confirmation"] = string.Format("ファイルが選択されていません。");
                    return RedirectToAction("Index");
                }

                // 対象(出力フォルダ取得用)
                string trg = model.TakeOverMoveBulkType;

                // サーバーにファイル保存
                string filename = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile(), trg));
                filename += Path.GetFileName(model.TakeOverMoveBulkUploadFile.FileName);
                model.TakeOverMoveBulkUploadFile.SaveAs(filename);

                TakeOverMoveBL bl = new TakeOverMoveBL();

                ArrayList ReadData = new ArrayList();

                if (model.TakeOverMoveBulkUploadFile.FileName.EndsWith("xls") || model.TakeOverMoveBulkUploadFile.FileName.EndsWith("xlsx"))
                {
                    //エクセルは不可とする
                    TempData["Confirmation"] = string.Format("ファイル形式が対象外です。");
                    return View("Index");
                    //// Excel用データ取得
                    //ReadData = bl.ExcelOpen(model.UploadFile);
                } else if (model.TakeOverMoveBulkUploadFile.FileName.EndsWith("csv")) {
                    // CSV用データ取得
                    ReadData = bl.CSVOpen(model.TakeOverMoveBulkUploadFile);
                } else {
                    TempData["Confirmation"] = string.Format("ファイル形式が対象外です。");
                    return View("Index");
                }

                //アップロード処理
                switch (model.TakeOverMoveBulkType) { // アップロード対象
                    case "Objective": // 目標管理
                        TempData["Confirmation"] = "ファイルの内容が正しくありません。確認してください。";
                        //ファイルチェック
                        if (bl.ObjectiveTakeOverMoveFileCheck(model, ReadData)==false) {
                            TempData["Confirmation"] = "ファイルの内容が正しくありません。確認してください。";
                            break;
                        }

                        //ファイルチェック データ存在チェック
                        message = bl.ObjectiveTakeOverMoveDataCheck(model, ReadData);
                        if (message!="") {
                            TempData["Confirmation"] = message;
                            break;
                        }

                        // 目標管理引継ぎ処理
                        if (bl.ObjectiveTakeOverMoveBulk(model, ReadData)) {
                            TempData["Confirmation"] = "アップロードしました。";
                        } else {
                            TempData["Confirmation"] = "アップロードに失敗しました。";
                            break;
                        }
                        break;

                    case "Skill": // 職能判定
                            TempData["Confirmation"] = "職能判定はこの処理を行えません。確認してください。";
                            break;
                }

                return View("Index");

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
        /// 異動によるデータ引継ぎ一括処理フォーマット用Excel出力処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [AcceptButton(ButtonName = "DoTakeOverMoveFormat")]
        public ActionResult DoTakeOverMoveFormat(SystemManagementModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //DL処理
                string mappath = Server.MapPath(this.GetFormatFilePath(WebConfig.GetConfigFile(),"TakeOverMoveBulk", model.TakeOverMoveBulkType));
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            } catch (Exception ex) {
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
        /// 組編によるデータ引継ぎ処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [ButtonHandler(ButtonName = "DoTakeOverAmendmentCompany")]
        public ActionResult DoTakeOverAmendmentCompany(SystemManagementModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                if (((LoginUser)Session["LoginUser"]).SYS管理組編DATA引継処理 == "0")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    return RedirectToAction("Index");
                }

                TakeOverAmendmentCompanyBL bl = new TakeOverAmendmentCompanyBL();

                //引継ぎ処理
                switch (model.TakeOverAmendmentCompanyType) { // 引継ぎ対象
                    case "Objective": // 目標管理
                        //引継ぎ元存在チェック
                        switch (bl.ObjectiveConfirm(model.TakeOverAmendmentCompanyYaer,model.TakeOverAmendmentCompanyEmployeeNo,model.TakeOverAmendmentCompanyBefDepartment))
                        {
                            case "Nothing": //存在しない
                                TempData["Confirmation"] = "引継ぎ元のデータが存在しません。確認してください。";
                                return View("Index");
                            case "Committed": //確定済み
                                //問題なし
                                break;

                            case "Updated": //更新済み
                                //問題なし
                                break;

                            case "Exist":   //存在する
                                //問題なし
                                break;
                        }

                        //引継ぎ先存在チェック
                        switch (bl.ObjectiveConfirm(model.TakeOverAmendmentCompanyYaer,model.TakeOverAmendmentCompanyEmployeeNo,model.TakeOverAmendmentCompanyAftDepartment))
                        {
                            case "Nothing": //存在しない
                                TempData["Confirmation"] = "引継ぎ先のデータが存在しません。確認してください。";
                                return View("Index");
                            case "Committed": //確定済み
                                TempData["Confirmation"] = "引継ぎ先のデータが既に確定済みです。確認してください。";
                                return View("Index");

                            case "Updated": //更新済み
                                TempData["TakeOverAmendmentCompanyReference"] = "引継ぎ先のデータが既に入力されています。<br />上書きしてよろしいですか？";
                                return View("Index");

                            case "Exist":   //存在する
                                //問題なし
                                break;
                        }            

                        // 目標管理引継ぎ処理
                        if (bl.ObjectiveTakeOverAmendmentCompany(model)) {
                            TempData["Confirmation"] = "引継ぎが完了しました。";
                        } else {
                            TempData["Confirmation"] = "引継ぎに失敗しました。";
                            break;
                        }
                        break;

                    case "Skill": // 職能判定
                            TempData["Confirmation"] = "職能判定はこの処理を行えません。確認してください。";
                            break;
                }

                return View("Index");
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
        /// 組編によるデータ引継ぎ処理_上書き確認後処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [ButtonHandler(ButtonName = "DoTakeOverAmendmentCompany_Reference")]
        public ActionResult DoTakeOverAmendmentCompany_Reference(SystemManagementModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                TakeOverAmendmentCompanyBL bl = new TakeOverAmendmentCompanyBL();
                // 目標管理引継ぎ処理
                if (bl.ObjectiveTakeOverAmendmentCompany(model)) {
                    TempData["Confirmation"] = "引継ぎが完了しました。";
                } else {
                    TempData["Confirmation"] = "引継ぎに失敗しました。";
                }
                return View("Index");
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
        /// 組編によるデータ引継ぎ一括処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [ButtonHandler(ButtonName = "DoTakeOverAmendmentCompanyBulk")]
        public ActionResult DoTakeOverAmendmentCompanyBulk(SystemManagementModels model, string value) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                if (((LoginUser)Session["LoginUser"]).SYS管理異動DATA引継処理 != "K")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    return RedirectToAction("Index");
                }

                string message = "";
                if(model.TakeOverAmendmentCompanyBulkType != objective) {
                    TempData["Confirmation"] = "この機能は目標管理しか行えません";
                    return View("Index");
                }

                if (model.TakeOverAmendmentCompanyBulkUploadFile == null) {
                    TempData["Confirmation"] = string.Format("ファイルが選択されていません。");
                    return RedirectToAction("Index");
                }

                // 対象(出力フォルダ取得用)
                string trg = model.TakeOverAmendmentCompanyBulkType;

                // サーバーにファイル保存
                string filename = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile(), trg));
                filename += Path.GetFileName(model.TakeOverAmendmentCompanyBulkUploadFile.FileName);
                model.TakeOverAmendmentCompanyBulkUploadFile.SaveAs(filename);

                TakeOverAmendmentCompanyBL bl = new TakeOverAmendmentCompanyBL();

                ArrayList ReadData = new ArrayList();

                if (model.TakeOverAmendmentCompanyBulkUploadFile.FileName.EndsWith("xls") || model.TakeOverAmendmentCompanyBulkUploadFile.FileName.EndsWith("xlsx"))
                {
                    //エクセルは不可とする
                    TempData["Confirmation"] = string.Format("ファイル形式が対象外です。");
                    return View("Index");
                    //// Excel用データ取得
                    //ReadData = bl.ExcelOpen(model.UploadFile);
                } else if (model.TakeOverAmendmentCompanyBulkUploadFile.FileName.EndsWith("csv")) {
                    // CSV用データ取得
                    ReadData = bl.CSVOpen(model.TakeOverAmendmentCompanyBulkUploadFile);
                } else {
                    TempData["Confirmation"] = string.Format("ファイル形式が対象外です。");
                    return View("Index");
                }

                //アップロード処理
                switch (model.TakeOverAmendmentCompanyBulkType) { // アップロード対象
                    case "Objective": // 目標管理
                        TempData["Confirmation"] = "ファイルの内容が正しくありません。確認してください。";
                        //ファイルチェック
                        if (bl.ObjectiveTakeOverAmendmentCompanyFileCheck(model, ReadData)==false) {
                            TempData["Confirmation"] = "ファイルの内容が正しくありません。確認してください。";
                            break;
                        }

                        //ファイルチェック データ存在チェック
                        message = bl.ObjectiveTakeOverAmendmentCompanyDataCheck(model, ReadData);
                        if (message!="") {
                            TempData["Confirmation"] = message;
                            break;
                        }

                        // 目標管理引継ぎ処理
                        if (bl.ObjectiveTakeOverAmendmentCompanyBulk(model, ReadData)) {
                            TempData["Confirmation"] = "アップロードしました。";
                        } else {
                            TempData["Confirmation"] = "アップロードに失敗しました。";
                            break;
                        }
                        break;

                    case "Skill": // 職能判定
                            TempData["Confirmation"] = "職能判定はこの処理を行えません。確認してください。";
                            break;
                }

                return View("Index");

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
        /// 組編によるデータ引継ぎ一括処理フォーマット用Excel出力処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [AcceptButton(ButtonName = "DoTakeOverAmendmentCompanyFormat")]
        public ActionResult DoTakeOverAmendmentCompanyFormat(SystemManagementModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //DL処理
                string mappath = Server.MapPath(this.GetFormatFilePath(WebConfig.GetConfigFile(),"TakeOverAmendmentCompanyBulk", model.TakeOverAmendmentCompanyBulkType));
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            } catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                TempData["Error"] = ex.ToString();
                return View("Error");
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        // 2018-03-20 iwai-tamura upd end ------


        /// <summary>
        /// 確定処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [AcceptButton(ButtonName = "DoCommit")]
        public ActionResult DoCommit(SystemManagementModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                // 2017-03-31 sbc-sagara upd str 支社管理機能の追加
                //if(!((LoginUser)Session["LoginUser"]).IsRoot) return RedirectToAction("Index", "Top");
                if (((LoginUser)Session["LoginUser"]).SYS管理確定処理 != "1")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    return RedirectToAction("Index");
                }
                // 2017-03-31 sbc-sagara upd end 支社管理機能の追加

                string message = new DataCommitBL().DoCommit(model);

                //テキストエリアのモデルに結果をセット
                ModelState.Clear();
                model.CommitResult = message;

                //結果がない場合は完了メッセージを表示
                string msg = "支社確定";
                if(model.CommitArea == "All") msg = "全体確定";     //全体確定が選ばれていた場合は、メッセージ変数に全体確定をセット
                msg = msg + "を完了しました";

                if(string.IsNullOrEmpty(message)) TempData["Success"] = msg;

                return View("Index",model);
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
        /// パスワードリセット
        /// </summary>
        /// <param name="model">システム管理画面モデル</param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [AcceptButton(ButtonName = "ResetPwd")]
        public ActionResult ResetPwd(SystemManagementModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                // 2017-03-31 sbc-sagara upd str 支社管理機能の追加
                //if (!((LoginUser)Session["LoginUser"]).IsRoot) return RedirectToAction("Index", "Top");
                if (((LoginUser)Session["LoginUser"]).SYS管理PASSRESET == "0")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    return RedirectToAction("Index");
                }
                // 2017-03-31 sbc-sagara upd end 支社管理機能の追加
                
                //リセット処理実行
                string message = new ResetPasswordBL().Reset(model);

                TempData["Success"] = message;

                return View("Index");
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return View("Index");//パスワードリセットは詳細表示しなくていいと考えられる。
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// お知らせ情報登録
        /// </summary>
        /// <param name="model">システム管理画面モデル</param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [AcceptButton(ButtonName = "CreateInfo")]
        public ActionResult CreateInfo(SystemManagementModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //ログイン判定
                if(!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                // 2017-03-31 sbc-sagara upd str 支社管理機能の追加
                //if (!((LoginUser)Session["LoginUser"]).IsRoot) return RedirectToAction("Index", "Top");
                if (((LoginUser)Session["LoginUser"]).SYS管理連絡事項 != "1")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    return RedirectToAction("Index");
                }
                // 2017-03-31 sbc-sagara upd end 支社管理機能の追加

                //完了メッセージ用一時変数
                TempData["Success"] = "登録しました";
                return View("Index", new InfoManagementBL().SaveInfo(model));
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

        //2017-03-31 sbc-sagara add str アカウント追加・パスワード確認機能追加
        /// <summary>
        /// アカウント追加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [AcceptButton(ButtonName = "AddAccount")]
        public ActionResult AddAccount(SystemManagementModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                if (((LoginUser)Session["LoginUser"]).SYS管理USER追加 != "1")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    return RedirectToAction("Index");
                }

                // 2017-06-16 iwai-tamura add str アカウント参照追加機能
                string message = "";
                AddAccountBL bl = new AddAccountBL();

                // 参照元社員番号入力時
                if (!string.IsNullOrEmpty(model.ReferenceEmployeeNo)) {
                    message = bl.GetReferenceEmployeeName(model.ReferenceEmployeeNo);

                    if (message != "") {
                        TempData["ReferenceID"] = message;
                    } else {
                        TempData["Confirmation"] = string.Format("入力された参照元社員番号は、無効な社員番号です。");
                    }
                    return View("Index");
                }

                //確認処理実行
                message = bl.Add(model);
                
                ////確認処理実行
                //string message = new AddAccountBL().Add(model);
                // 2017-06-16 iwai-tamura add end アカウント参照追加機能

                TempData["Confirmation"] = message;

                return View("Index");
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

        // 2017-06-16 iwai-tamura add str アカウント参照追加機能
        /// <summary>
        /// アカウント参照追加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [AcceptButton(ButtonName = "AddAccount_Reference")]
        public ActionResult AddAccount_Reference(SystemManagementModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                if (((LoginUser)Session["LoginUser"]).SYS管理USER追加 != "1") {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    return RedirectToAction("Index");
                }

                string message = "";
                AddAccountBL bl = new AddAccountBL();

                //確認処理実行
                message = bl.ReferenceAdd(model);

                TempData["Confirmation"] = message;

                return View("Index");
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
        // 2017-06-16 iwai-tamura add end アカウント参照追加機能


        /// <summary>
        /// パスワード確認
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [AcceptButton(ButtonName = "ConfirmPwd")]
        public ActionResult ConfirmPwd(SystemManagementModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                if (((LoginUser)Session["LoginUser"]).SYS管理PASS確認 == "0")
                {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    return RedirectToAction("Index");
                }

                //確認処理実行
                string message = new ConfirmPasswordBL().Confirm(model);

                TempData["Confirmation"] = message;

                return View("Index");
            }
            catch (Exception ex)
            {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return View("Index");
            }
            finally
            {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        // 2017-03-31 sbc-sagara add end パスワード確認機能追加

        // 2017-03-31 sbc-sagara add str 戻るボタン追加
        /// <summary>
        /// 戻る
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
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


        // 2017-04-30 sbc-sagara add str アップロード,Excel出力機能追加
        /// <summary>
        /// アップロード
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [AcceptButton(ButtonName = "Upload")]
        public ActionResult Upload(SystemManagementModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //ログイン判定
                if (!(new LoginBL()).IsLogin()) return RedirectToAction("Login", "Login");

                //管理者判定
                if (((LoginUser)Session["LoginUser"]).SYS管理PASS確認 == "0") {
                    TempData["Confirmation"] = string.Format("権限が無い為、この機能は使えません。");
                    return RedirectToAction("Index");
                }

                if (model.UploadFile == null) {
                    TempData["Confirmation"] = string.Format("ファイルが選択されていません。");
                    return RedirectToAction("Index");
                }

                // 対象(出力フォルダ取得用)
                string trg = model.UploadTarget;
                // サーバーにファイル保存
                string filename = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile(), trg));
                //2017-10-31 iwai-tamura upd-str ドメイン接続の場合、フルパスを取得する為、ファイル名を抽出------
                filename += Path.GetFileName(model.UploadFile.FileName);
                //filename += model.UploadFile.FileName;
                //2017-10-31 iwai-tamura upd-str ------
                
                model.UploadFile.SaveAs(filename);
                UploadBL bl = new UploadBL();

                ArrayList ReadData = new ArrayList();

                if (model.UploadFile.FileName.EndsWith("xls") || model.UploadFile.FileName.EndsWith("xlsx"))
                {
                    //エクセルは不可とする
                    TempData["Confirmation"] = string.Format("ファイル形式が対象外です。");
                    return View("Index");
                    //// Excel用データ取得
                    //ReadData = bl.ExcelOpen(model.UploadFile);
                } else if (model.UploadFile.FileName.EndsWith("csv")) {
                    // CSV用データ取得
                    ReadData = bl.CSVOpen(model.UploadFile);
                } else {
                    TempData["Confirmation"] = string.Format("ファイル形式が対象外です。");
                    return View("Index");
                }

                //アップロード処理
                switch (model.UploadTarget) { // アップロード対象
                    case "Objective": // 目標管理
                        //ファイルチェック
                        if (bl.ObjectiveApproverFileCheck(model, ReadData)==false) {
                            TempData["Confirmation"] = "ファイルの内容が正しくありません。確認してください。";
                            break;
                        }
                        // 目標管理決裁権限アップデート
                        if (bl.ObjectiveApproverUpdate(model, ReadData)) {
                            TempData["Confirmation"] = "アップロードしました。";
                        } else {
                            TempData["Confirmation"] = "アップロードに失敗しました。";
                            break;
                        }
                        break;
                    case "Skill": // 職能判定
                        //ファイルチェック
                        if (bl.SkillApproverFileCheck(model, ReadData)==false) {
                            TempData["Confirmation"] = "ファイルの内容が正しくありません。確認してください。";
                            break;
                        }
                        // 目標管理決裁権限アップデート
                        if (bl.SkillApproverUpdate(model, ReadData)) {
                            TempData["Confirmation"] = "アップロードしました。";
                        } else {
                            TempData["Confirmation"] = "アップロードに失敗しました。";
                            break;
                        }
                        break;
                }

                return View("Index");
            } catch (Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return View("Index");
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// Excel出力処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Management")]
        [AcceptButton(ButtonName = "ExcelOutput")]
        public ActionResult ExcelOutput(SystemManagementModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                // 対象(出力フォルダ取得用)
                string trg = model.OutputTarget;

                //帳票作成ディレクトリを取得
                string fullPath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile(), trg));

                //帳票出力ロジックを実行
                ExcelOutputBL bl = new ExcelOutputBL(fullPath);

                // データ取得・ファイル作成
                var dlpath = bl.ExcelOutput(model);

                //DL処理
                string mappath = Server.MapPath(this.GetTempDir(WebConfig.GetConfigFile(), trg) + dlpath);
                string fileName = Path.GetFileName(mappath);
                return File(mappath, "application/zip", fileName);
            } catch (Exception ex) {
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
        /// コンフィグから帳票出力フォルダ取得
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private string GetTempDir(Configuration config, string trg = "") {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                if (trg == "Objective") {
                    return FileUtil.GetTempDir(config, "DOWNLOAD_TEMP_DIR_O");
                } else if (trg == "Skill") {
                    return FileUtil.GetTempDir(config, "DOWNLOAD_TEMP_DIR_S");
                }
                return "";
            } catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        // 2017-04-30 sbc-sagara add end アップロード,Excel出力機能追加


        // 2018-03-20 iwai-tamura upd str ------
        /// <summary>
        /// コンフィグからフォーマット用出力ファイル取得
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private string GetFormatFilePath(Configuration config,string proc = "", string trg = "") {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                switch (proc) {
                    case "TakeOverMoveBulk":
                        if (trg == "Objective") {
                            return FileUtil.GetTempFile(config, "DOWNLOAD_FORMATFILE_DIR_MOVE_O");
                        } else {
                            return "";
                        }
                    case "TakeOverAmendmentCompanyBulk":
                        if (trg == "Objective") {
                            return FileUtil.GetTempFile(config, "DOWNLOAD_FORMATFILE_DIR_AMENDMENT_O");
                        } else {
                            return "";
                        }
                }
                return "";
            } catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }    
        // 2018-03-20 iwai-tamura upd end ------        
    }
}
