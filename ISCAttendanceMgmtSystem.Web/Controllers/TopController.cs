using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using ISCAttendanceMgmtSystem.COM.Models;
using ISCAttendanceMgmtSystem.COM.Util.Controll;
using ISCAttendanceMgmtSystem.COM.Util.Database;
using ISCAttendanceMgmtSystem.COM.Entity.Session;
using ISCAttendanceMgmtSystem.BL.Login;
using ISCAttendanceMgmtSystem.Bl.Top;
using System.Collections;
using ISCAttendanceMgmtSystem.COM.Enum;

namespace ISCAttendanceMgmtSystem.Web.Controllers {

    /// <summary>
    /// TOP画面用コントローラー
    /// </summary>
    public class TopController : Controller {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();
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
                
                //ログイン情報取得
                var lu = (LoginUser)Session["LoginUser"];
                
                //表示
                return View((new TopBL()).Index(lu.UserCode));
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



        //[HttpPost]
        //[ActionName("Begin")]
        [HttpPost]
        [ButtonHandler(ButtonName = "Begin")]
        [ValidateAntiForgeryToken]
        public ActionResult BeginButton(TopViewModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                LoginUser lu = (LoginUser)Session["LoginUser"];

                TopBL bl = new TopBL();
                if(bl.WorkBegin(model, lu.UserCode)) {
                    //2023-10-30 iwai-tamura upd-str ------
                    TempData["AttendanceStatusMessage"] = string.Format("おはようございます。<br>出勤の打刻をしました。<br>本日の出勤状況を選択してください。");
                    //TempData["Confirmation"] = string.Format("おはようございます。<br>出勤の打刻をしました。");
                    //2023-10-30 iwai-tamura upd-end ------
                } else {
                    TempData["Confirmation"] = string.Format("打刻エラー<br>管理者に問い合わせください。");
                }

                return View((new TopBL()).Index(lu.UserCode));
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


        [HttpPost]
        [ButtonHandler(ButtonName = "Finish")]
        [ValidateAntiForgeryToken]
        public ActionResult FinishButton(TopViewModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                LoginUser lu = (LoginUser)Session["LoginUser"];

                TopBL bl = new TopBL();
                if(bl.WorkFinish(model, lu.UserCode)) {
                    TempData["Confirmation"] = string.Format("退勤の打刻をしました。<br>本日もお疲れ様でした。");
                } else {
                    TempData["Confirmation"] = string.Format("打刻エラー<br>管理者に問い合わせください。");
                }

                return View((new TopBL()).Index(lu.UserCode));
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

        //2023-10-30 iwai-tamura upd-str ------
        //出勤状況変更:出社(出勤処理と同時)
        [HttpPost]
        [ButtonHandler(ButtonName = "ChangeStatusAttendBegin")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStatusAttendBeginButton(TopViewModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                LoginUser lu = (LoginUser)Session["LoginUser"];

                TopBL bl = new TopBL();
                if (bl.ChangeStatus(model, lu.UserCode, ((int)AttendanceStatus.Attend).ToString("00"),true))
                {
                } else
                {
                    TempData["Confirmation"] = string.Format("打刻エラー<br>管理者に問い合わせください。");
                }

                return View((new TopBL()).Index(lu.UserCode));
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

        //出勤状況変更:テレワーク(出勤処理と同時)
        [HttpPost]
        [ButtonHandler(ButtonName = "ChangeStatusTeleworkBegin")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStatusTeleworkBeginButton(TopViewModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                LoginUser lu = (LoginUser)Session["LoginUser"];

                TopBL bl = new TopBL();
                if (bl.ChangeStatus(model, lu.UserCode, ((int)AttendanceStatus.telework).ToString("00"),true))
                {
                } else
                {
                    TempData["Confirmation"] = string.Format("打刻エラー<br>管理者に問い合わせください。");
                }

                return View((new TopBL()).Index(lu.UserCode));
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

        //出勤状況変更:外出(出勤処理と同時)
        [HttpPost]
        [ButtonHandler(ButtonName = "ChangeStatusOutBegin")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStatusOutBeginButton(TopViewModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                LoginUser lu = (LoginUser)Session["LoginUser"];

                TopBL bl = new TopBL();
                if (bl.ChangeStatus(model, lu.UserCode, ((int)AttendanceStatus.outing).ToString("00"),true))
                {
                } else
                {
                    TempData["Confirmation"] = string.Format("打刻エラー<br>管理者に問い合わせください。");
                }

                return View((new TopBL()).Index(lu.UserCode));
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



        //出勤状況変更:出社
        [HttpPost]
        [ButtonHandler(ButtonName = "ChangeStatusAttend")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStatusAttendButton(TopViewModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                LoginUser lu = (LoginUser)Session["LoginUser"];

                TopBL bl = new TopBL();
                if (bl.ChangeStatus(model, lu.UserCode, ((int)AttendanceStatus.Attend).ToString("00"),false))
                {
                    TempData["Confirmation"] = string.Format("ステータスを変更しました。");
                } else
                {
                    TempData["Confirmation"] = string.Format("打刻エラー<br>管理者に問い合わせください。");
                }

                return View((new TopBL()).Index(lu.UserCode));
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

        //出勤状況変更:テレワーク
        [HttpPost]
        [ButtonHandler(ButtonName = "ChangeStatusTelework")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStatusTeleworkButton(TopViewModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                LoginUser lu = (LoginUser)Session["LoginUser"];

                TopBL bl = new TopBL();
                if (bl.ChangeStatus(model, lu.UserCode, ((int)AttendanceStatus.telework).ToString("00"),false))
                {
                    TempData["Confirmation"] = string.Format("ステータスを変更しました。");
                } else
                {
                    TempData["Confirmation"] = string.Format("打刻エラー<br>管理者に問い合わせください。");
                }

                return View((new TopBL()).Index(lu.UserCode));
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

        //出勤状況変更:外出
        [HttpPost]
        [ButtonHandler(ButtonName = "ChangeStatusOut")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStatusOutButton(TopViewModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                LoginUser lu = (LoginUser)Session["LoginUser"];

                TopBL bl = new TopBL();
                if (bl.ChangeStatus(model, lu.UserCode, ((int)AttendanceStatus.outing).ToString("00"),false))
                {
                    TempData["Confirmation"] = string.Format("ステータスを変更しました。");
                } else
                {
                    TempData["Confirmation"] = string.Format("打刻エラー<br>管理者に問い合わせください。");
                }

                return View((new TopBL()).Index(lu.UserCode));
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
        //2023-10-30 iwai-tamura upd-end ------


    }
}
