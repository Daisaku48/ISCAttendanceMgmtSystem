using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ISCAttendanceMgmtSystem.BL.Common;
using ISCAttendanceMgmtSystem.COM.Entity.Session;
using ISCAttendanceMgmtSystem.COM.Enum;
using ISCAttendanceMgmtSystem.COM.Models;
using ISCAttendanceMgmtSystem.COM.Util.Config;
using ISCAttendanceMgmtSystem.COM.Util.Convert;
using ISCAttendanceMgmtSystem.COM.Util.Database;
using ISCAttendanceMgmtSystem.COM.Util.Zip;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Style;


namespace ISCAttendanceMgmtSystem.BL.AttendanceRecordUpdate {
    /// <summary>
    /// 勤務実績修正
    /// </summary>
    public class AttendanceRecordUpdateBL {
        #region メンバ変数

        /// <summary>
        /// 帳票作成ディレクトリフルパス
        /// </summary>
        private readonly string TempDir;

        /// <summary>
        /// フォルダ削除基準日数
        /// </summary>
        private readonly int R_P;

        /// <summary>
        /// TBL区分(現在)
        /// </summary>
        private const string TBL_TYPE_G = "G";
        /// <summary>
        /// TBL区分(履歴)
        /// </summary>
        private const string TBL_TYPE_R = "R";

        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();
        #endregion


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AttendanceRecordUpdateBL() {
            Configuration config = WebConfig.GetConfigFile();

            //帳票作成ディレクトリ内保存日数を取得(int変換に失敗した場合は3とする。)
            if (!int.TryParse(config.AppSettings.Settings["RETENTIO_PERIOD"].Value, out R_P)) { R_P = 3; };
        }

        /// <summary>
        /// 修正可能従業員一覧取得
        /// </summary>
        public IList<SelectListItem> AttendanceRecordUpdateEmployeeList(LoginUser lu) {

            //従業員の当日の勤務状況データを取得
            var sql = "";
            var sqlWhere = "";
            var sqlOrderby = "";
            sql = " SELECT "
                + "     TM基本.従業員Code"
                + "     ,TM基本.氏名"

            //2023-10-30 iwai-tamura upd-str ------
                + "     ,TM基本.入社年月日"
                + "     ,TM基本.退職年月日"
            //2023-10-30 iwai-tamura upd-end ------


                + " FROM TM010従業員情報Master TM基本 ";

            //選択可能従業員条件
            switch (lu.AdminType) {
                case "4":
                    //担当管理職：自分自身はのぞく
                    sqlWhere = " WHERE 1=1 "
                            + "     AND ( "
                            + "         所属Code BETWEEN @ManageDepartmentStart AND @ManageDepartmentEnd"
                            + "         OR 所属Code in(@ManageDepartment1,@ManageDepartment2,@ManageDepartment3,@ManageDepartment4,@ManageDepartment5)"
                            + "         OR 従業員Code in(@ManageEmployee1,@ManageEmployee2,@ManageEmployee3,@ManageEmployee4,@ManageEmployee5)"
                            + "     )"
                            + "     AND 従業員Code <> @Myself";
                    break;

                case "5":
                case "9":
                    //管理職、総務職
                    sqlWhere = " WHERE 1=1 "
                            + "     AND ( "
                            + "         所属Code BETWEEN @ManageDepartmentStart AND @ManageDepartmentEnd"
                            + "         OR 所属Code in(@ManageDepartment1,@ManageDepartment2,@ManageDepartment3,@ManageDepartment4,@ManageDepartment5)"
                            + "         OR 従業員Code in(@ManageEmployee1,@ManageEmployee2,@ManageEmployee3,@ManageEmployee4,@ManageEmployee5)"
                            + "     )";
                    break;
                case "Z":
                    //システム管理者：全データ閲覧可能
                    sqlWhere = " WHERE 1=1 ";
                    break;
                default:
                    //その他：閲覧不可
                    sqlWhere = " WHERE 1=2 ";
                    break;
            }


            //並び替え条件
            sqlOrderby = "  ORDER BY TM基本.所属Code,TM基本.役職Code,TM基本.従業員Code   ";

            sql += sqlWhere + sqlOrderby;
                
            IList<SelectListItem> itemList = new List<SelectListItem>();
            using (DbManager dm = new DbManager())
            using (IDbCommand cmd = dm.CreateCommand(sql))
            using (DataSet ds = new DataSet()) {
                DbHelper.AddDbParameter(cmd, "@ManageDepartmentStart", DbType.String);
                DbHelper.AddDbParameter(cmd, "@ManageDepartmentEnd", DbType.String);
                DbHelper.AddDbParameter(cmd, "@ManageDepartment1", DbType.String);
                DbHelper.AddDbParameter(cmd, "@ManageDepartment2", DbType.String);
                DbHelper.AddDbParameter(cmd, "@ManageDepartment3", DbType.String);
                DbHelper.AddDbParameter(cmd, "@ManageDepartment4", DbType.String);
                DbHelper.AddDbParameter(cmd, "@ManageDepartment5", DbType.String);
                DbHelper.AddDbParameter(cmd, "@ManageEmployee1", DbType.String);
                DbHelper.AddDbParameter(cmd, "@ManageEmployee2", DbType.String);
                DbHelper.AddDbParameter(cmd, "@ManageEmployee3", DbType.String);
                DbHelper.AddDbParameter(cmd, "@ManageEmployee4", DbType.String);
                DbHelper.AddDbParameter(cmd, "@ManageEmployee5", DbType.String);
                DbHelper.AddDbParameter(cmd, "@Myself", DbType.String);
                ((IDbDataParameter)cmd.Parameters[0]).Value = lu.ManageDepartmentStart; //入力可能所属範囲_開始
                ((IDbDataParameter)cmd.Parameters[1]).Value = lu.ManageDepartmentEnd;   //入力可能所属範囲_終了
                ((IDbDataParameter)cmd.Parameters[2]).Value = lu.ManageDepartment1;     //入力可能所属個別1
                ((IDbDataParameter)cmd.Parameters[3]).Value = lu.ManageDepartment2;     //入力可能所属個別2
                ((IDbDataParameter)cmd.Parameters[4]).Value = lu.ManageDepartment3;     //入力可能所属個別3
                ((IDbDataParameter)cmd.Parameters[5]).Value = lu.ManageDepartment4;     //入力可能所属個別4
                ((IDbDataParameter)cmd.Parameters[6]).Value = lu.ManageDepartment5;     //入力可能所属個別5
                ((IDbDataParameter)cmd.Parameters[7]).Value = lu.ManageEmployee1;       //入力可能従業員個別1
                ((IDbDataParameter)cmd.Parameters[8]).Value = lu.ManageEmployee2;       //入力可能従業員個別2
                ((IDbDataParameter)cmd.Parameters[9]).Value = lu.ManageEmployee3;       //入力可能従業員個別3
                ((IDbDataParameter)cmd.Parameters[10]).Value = lu.ManageEmployee4;      //入力可能従業員個別4
                ((IDbDataParameter)cmd.Parameters[11]).Value = lu.ManageEmployee5;      //入力可能従業員個別5
                ((IDbDataParameter)cmd.Parameters[12]).Value = lu.UserCode;             //自分自身
                IDataAdapter da = dm.CreateSqlDataAdapter(cmd);

                // データセットに設定する
                da.Fill(ds);

                foreach (DataRow row in ds.Tables[0].Rows) {
                    SelectListItem item = new SelectListItem {
                        //2023-10-30 iwai-tamura upd-str ------
                        Value = row["従業員Code"].ToString() + "," + row["入社年月日"].ToString() + "," + row["退職年月日"].ToString(),
                        //Value = row["従業員Code"].ToString(),
                        //2023-10-30 iwai-tamura upd-end ------
                        Text = row["氏名"].ToString()
                    };
                    itemList.Add(item);
                }
            }
            return itemList;
        }

        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="model">勤務実績修正検索モデル</param>
        /// <returns>検索モデル</returns>
        public AttendanceRecordUpdateModels Search(AttendanceRecordUpdateModels model, LoginUser lu) {
            //開始
            nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
            try {
                //無記名関数
                //打刻データを表示用変換  HH
                Func<string, string> dbDateToViewDateHour = (val) => {
                    if (val == "") {
                        return "";
                    }
                    return DateTime.Parse(val).ToString("HH");
                };

                //打刻データを表示用変換  mm
                Func<string, string> dbDateToViewDateMinutes = (val) => {
                    if (val == "") {
                        return "";
                    }
                    return DateTime.Parse(val).ToString("mm");
                };


                //打刻データを表示用変換  HH:mm
                Func<string, string> dbDateToViewDate = (val) => {
                    if (val == "") {
                        return "";
                    }
                    return DateTime.Parse(val).ToString("HH:mm");
                };


                string searchDate = DateTime.Parse(model.Search.Date.ToString()).ToString("yyyyMMdd");


                //データ取得
                var sql = " "
                        + " SELECT"
                        + "     * "
                        + " FROM"
                        + "     TA100勤怠実績Data"
                        + " WHERE"
                        + "     勤務年月日='{0}'"
                        + "     And 従業員Code='{1}'";

                sql = string.Format(sql, searchDate, model.Search.EmployeeNo);
                using (DbManager dm = new DbManager())
                using (IDbCommand cmd = dm.CreateCommand(sql))
                using (DataSet ds = new DataSet()) {
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    // データセットに設定する
                    da.Fill(ds);
                    model.Input = new AttendanceRecordUpdateInputModel();
                    if (ds.Tables[0].Rows.Count == 0) {
                        AttendanceRecordUpdateInputModel resultInput = new AttendanceRecordUpdateInputModel();

                        resultInput.InputBeginHour = "";
                        resultInput.InputBeginMinutes = "";
                        resultInput.InputFinishHour = "";
                        resultInput.InputFinishMinutes = "";
                        //2023-10-30 iwai-tamura upd-str ------
                        resultInput.AttendanceStatus = "";
                        //2023-10-30 iwai-tamura upd-end ------
                        resultInput.InputComment = "";
                        model.Input = resultInput;
                    } else { 
                        DataRow row = ds.Tables[0].Rows[0];
                        AttendanceRecordUpdateInputModel resultInput = new AttendanceRecordUpdateInputModel();

                        resultInput.InputBeginHour = dbDateToViewDateHour(row["開始打刻"].ToString());
                        resultInput.InputBeginMinutes = dbDateToViewDateMinutes(row["開始打刻"].ToString());
                        resultInput.InputFinishHour = dbDateToViewDateHour(row["終了打刻"].ToString());
                        resultInput.InputFinishMinutes = dbDateToViewDateMinutes(row["終了打刻"].ToString());
                        //2023-10-30 iwai-tamura upd-str ------
                        resultInput.AttendanceStatus = row["勤務状況区分"].ToString();
                        //2023-10-30 iwai-tamura upd-end ------
                        resultInput.InputComment = "";
                        model.Input = resultInput;
                    }
                }

                //履歴データ取得
                int resultNo = 1;
                sql = " "
                        + " SELECT"
                        + "     T履歴.* "
                        + "     ,T従業員.氏名 As 処理者従業員氏名"
                        //2023-10-30 iwai-tamura upd-str ------
                        + "     ,T勤務状況.勤務状況区分名 "
                        + "     ,T処理区分.勤怠実績処理区分名 "
                        //2023-10-30 iwai-tamura upd-end ------
                        + " FROM"
                        + "     TA110勤怠実績履歴Data T履歴 "
                        + "     LEFT JOIN TM010従業員情報Master T従業員 "
                        + "         ON T履歴.処理者従業員Code = T従業員.従業員Code "
                        //2023-10-30 iwai-tamura upd-str ------
                        + "     LEFT JOIN TM030勤務状況区分Master T勤務状況 "
                        + "         ON T履歴.勤務状況区分 = T勤務状況.勤務状況区分 "
                        + "         AND T勤務状況.発足日 <= T履歴.勤務年月日 "
                        + "         AND T勤務状況.廃止日 >= T履歴.勤務年月日 "
                        + "     LEFT JOIN TM031勤怠実績処理区分Master T処理区分 "
                        + "         ON T履歴.処理区分 = T処理区分.勤怠実績処理区分 "
                        + "         AND T処理区分.発足日 <= T履歴.勤務年月日 "
                        + "         AND T処理区分.廃止日 >= T履歴.勤務年月日 "
                        //2023-10-30 iwai-tamura upd-end ------
                        + " WHERE"
                        + "     勤務年月日='{0}'"
                        + "     And T履歴.従業員Code='{1}'";

                sql = string.Format(sql, searchDate, model.Search.EmployeeNo);
                using (DbManager dm = new DbManager())
                using(IDbCommand cmd = dm.CreateCommand(sql))
                using(DataSet ds = new DataSet()) {
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    // データセットに設定する
                    da.Fill(ds);

                    List<AttendanceRecordUpdateListModel> resultList = new List<AttendanceRecordUpdateListModel>();
                    foreach (DataRow row in ds.Tables[0].Rows) {

                        //打刻データを挿入
                        AttendanceRecordUpdateListModel result = new AttendanceRecordUpdateListModel {
                            Number = resultNo.ToString(),
                            UpdateDateTimeView = DateTime.Parse(row["処理年月日"].ToString()).ToString("MM/dd HH:mm"),
                            UpdateEmployeeNo = row["処理者従業員Code"].ToString(),
                            UpdateEmployeeName = row["処理者従業員氏名"].ToString(),
                            Comment = row["処理Comment"].ToString(),
                            UpdateType = row["処理区分"].ToString(),
                            StampTimeBegin = dbDateToViewDate(row["開始打刻"].ToString()),
                            StampTimeFinish = dbDateToViewDate(row["終了打刻"].ToString()),
                            //2023-10-30 iwai-tamura upd-str ------
                            AttendanceStatusName = row["勤務状況区分名"].ToString(),
                            UpdateTypeName = row["勤怠実績処理区分名"].ToString(),
                            //2023-10-30 iwai-tamura upd-end ------

                        };

                        //2023-10-30 iwai-tamura upd-str ------
                        ////処理区分名取得(テーブル化もしくは列挙型予定)
                        //switch (result.UpdateType) {
                        //    case "01":
                        //        result.UpdateTypeName = "本人登録";
                        //        break;

                        //    case "02":
                        //        result.UpdateTypeName = "修正登録";
                        //        break;
                        //}
                        //2023-10-30 iwai-tamura upd-end ------
                        resultList.Add(result);
                        resultNo += 1; //を加算
                    }

                    model.SearchResult = resultList;
                }
                model.Head.ViewType = "2";
                return model;
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 勤務実績修正処理
        /// </summary>
        /// <param name="AttendanceRecordUpdateModels"></param>
        /// <param name="ユーザーID"></param>
        /// <returns>処理成否</returns>

        public bool AttendanceRecordUpdate(AttendanceRecordUpdateModels model, LoginUser lu) {
            //開始
            nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
            try {
                //勤怠実績データ登録
                if (!AttendanceRegisterDataUpdate(model, lu)) {
                    return false;
                }

                return true;
            }
            catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return false;
            }
            finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// 勤怠実績データ登録・修正
        /// </summary>
        /// <param name="AttendanceRecordUpdateModels"></param>
        /// <param name="ユーザーID"></param>
        /// <returns>処理成否</returns>

        public bool AttendanceRegisterDataUpdate(AttendanceRecordUpdateModels model, LoginUser lu) {
            //開始
            nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
            try {
                //無記名関数
                //打刻データを表示用変換  HH:mm
                Func<string, string> dbDateToViewDate = (val) => {
                    if (val == "") {
                        return "";
                    }
                    return DateTime.Parse(val).ToString("HH:mm");
                };

                //修正前データ存在チェック
                bool bolWorkDate;

                string originStampTimeBegin = "";
                string originStampTimeFinish = "";

                string updateStampTimeBegin = "";
                string updateStampTimeFinish = "";

                //データ存在チェック
                bool updateBegin = false;
                bool updateFinish = false;

                var sql = " SELECT Format(開始打刻,'HH:mm') As 開始打刻 "
                        + "     ,Format(終了打刻,'HH:mm') As 終了打刻 "
                        + " FROM TA100勤怠実績Data "
                        + "  WHERE 従業員Code = @EmployeeNo and 勤務年月日 = @Date";
                using (DbManager dm = new DbManager())
                using (IDbCommand cmd = dm.CreateCommand(sql)) {
                    DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
                    DbHelper.AddDbParameter(cmd, "@Date", DbType.String);
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = model.Search.EmployeeNo;   //IDを指定
                    parameters[1].Value = model.Search.Date.ToString("yyyyMMdd");   //年月日を指定
                    using (DataSet ds = new DataSet()) {
                        IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                        // データセットに設定する
                        da.Fill(ds);
                        if (ds.Tables[0].Rows.Count == 0) {
                            bolWorkDate = false;
                        } else {
                            //修正前データを取得
                            DataRow row = ds.Tables[0].Rows[0];
                            originStampTimeBegin = dbDateToViewDate(row["開始打刻"].ToString());
                            originStampTimeFinish = dbDateToViewDate(row["終了打刻"].ToString());
                            bolWorkDate = true;
                        }
                    }

                    //開始打刻修正判断
                    if (((model.Input.InputBeginHour ?? "") != "") && ((model.Input.InputBeginMinutes ?? "") != "")) {
                        updateStampTimeBegin = model.Search.Date.ToString("yyyy-MM-dd ") + model.Input.InputBeginHour + ":" + model.Input.InputBeginMinutes;
                        if (bolWorkDate) {
                            //修正前と比較
                            if (originStampTimeBegin != model.Input.InputBeginHour + ":" + model.Input.InputBeginMinutes) {
                                updateBegin = true;
                            }
                        } else {
                            updateBegin = true;
                        }
                    }

                    //終了打刻修正判断
                    if (((model.Input.InputFinishHour ?? "") != "") && ((model.Input.InputFinishMinutes ?? "") != "")) {
                        updateStampTimeFinish = model.Search.Date.ToString("yyyy-MM-dd ") + model.Input.InputFinishHour + ":" + model.Input.InputFinishMinutes;
                        if (bolWorkDate) {
                            //修正前と比較
                            if (originStampTimeFinish != model.Input.InputFinishHour + ":" + model.Input.InputFinishMinutes) {
                                updateFinish = true;
                            }
                        } else {
                            updateFinish = true;
                        }
                    }
                }

                //勤怠実績登録修正処理
                using (var scope = new TransactionScope()) {
                    using (DbManager dm = new DbManager()) {
                        if (bolWorkDate) {
                            //更新処理
                            sql = "UPDATE TA100勤怠実績Data"
                                + " SET "
                                +"      打刻種類Flg = 打刻種類Flg ";
                            if (updateBegin) {
                                sql += "    ,開始打刻 = '" + updateStampTimeBegin + "'"
                                    + "     ,開始打刻更新回数 = 開始打刻更新回数 + 1";
                            }

                            if (updateFinish) {
                                sql += "    ,終了打刻 = '" + updateStampTimeFinish + "'"
                                    + "     ,終了打刻更新回数 = 終了打刻更新回数 + 1";
                            }

                            //共通更新
                            //2023-10-30 iwai-tamura upd-str ------
                            sql += "    ,勤務状況区分 = @AttendanceStatus"; //勤務状況更新
                            //2023-10-30 iwai-tamura upd-end ------

                            sql += "    ,最終更新者Code = @UpdateEmployeeNo"
                                + "     ,更新年月日 = @NowDatetime"
                                + "     ,更新回数 = 更新回数 + 1";

                            //条件文
                            sql += " WHERE "
                                + "     従業員Code = @TargetEmployeeNo"
                                + "     AND 勤務年月日 = @TargetDate";

                            var cmd = dm.CreateCommand(sql);
                            DbHelper.AddDbParameter(cmd, "@TargetEmployeeNo", DbType.String);
                            DbHelper.AddDbParameter(cmd, "@UpdateEmployeeNo", DbType.String);
                            DbHelper.AddDbParameter(cmd, "@TargetDate", DbType.String);
                            DbHelper.AddDbParameter(cmd, "@NowDatetime", DbType.String);
                            //2023-10-30 iwai-tamura upd-str ------
                            DbHelper.AddDbParameter(cmd, "@AttendanceStatus", DbType.String);
                            //2023-10-30 iwai-tamura upd-end ------

                            //パラメータ設定
                            var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                            parameters[0].Value = model.Search.EmployeeNo;                          //修正対象データ従業員
                            parameters[1].Value = model.Head.EmployeeNo;                            //修正者
                            parameters[2].Value = model.Search.Date.ToString("yyyyMMdd");           //修正対象勤務年月日
                            parameters[3].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");     //登録・更新年月日
                            //2023-10-30 iwai-tamura upd-str ------
                            parameters[4].Value = model.Input.AttendanceStatus;                     //勤務状況区分
                            //2023-10-30 iwai-tamura upd-end ------
                            if (cmd.ExecuteNonQuery() <= 0) {
                                throw new Exception("打刻修正処理エラー");
                            }
                        } else {
                            //追加処理
                            sql = "INSERT INTO TA100勤怠実績Data"
                                + " VALUES ("
                                + " @TargetEmployeeNo"
                                + " ,@TargetDate"
                                + " ,'1'" //打刻種類Flg
                                + " ,''" //社内勤務体系Code
                                + " ,''" //申請種類Flg
                                + " ,null" //予定開始打刻
                                + " ,null" //予定終了打刻
                                + " ,@StampTimeBegin" //開始打刻(終了打刻時によりデータ挿入の為空で挿入)
                                + " ,@StampTimeFinish"
                                + " ,null" //確定開始打刻
                                + " ,null" //確定終了打刻
                                + " ,@StampNumBegin" //開始打刻更新回数
                                + " ,@StampNumFinish" //終了打刻更新回数
                                + " ,''" //確定開始打刻更新者
                                + " ,''" //確定終了打刻更新者
                                //2023-10-30 iwai-tamura upd-str ------
                                + " ,@AttendanceStatus" //勤務状況区分
                                //2023-10-30 iwai-tamura upd-end ------
                                + " ,''" //予定外出Flg
                                + " ,''" //外出先Code1
                                + " ,''" //外出先Code2
                                + " ,''" //外出先Code3
                                + " ,''" //外出先Code4
                                + " ,''" //外出先Code5
                                + " ,@UpdateEmployeeNo"
                                + " ,@UpdateEmployeeNo"
                                + " ,@NowDatetime"
                                + " ,@NowDatetime"
                                + " ,'1'"
                                + " )";
                            var cmd = dm.CreateCommand(sql);
                            DbHelper.AddDbParameter(cmd, "@TargetEmployeeNo", DbType.String);
                            DbHelper.AddDbParameter(cmd, "@UpdateEmployeeNo", DbType.String);
                            DbHelper.AddDbParameter(cmd, "@TargetDate", DbType.String);
                            DbHelper.AddDbParameter(cmd, "@StampTimeBegin", DbType.String);
                            DbHelper.AddDbParameter(cmd, "@StampTimeFinish", DbType.String);
                            DbHelper.AddDbParameter(cmd, "@StampNumBegin", DbType.Int32);
                            DbHelper.AddDbParameter(cmd, "@StampNumFinish", DbType.Int32);
                            DbHelper.AddDbParameter(cmd, "@NowDatetime", DbType.String);
                            //2023-10-30 iwai-tamura upd-str ------
                            DbHelper.AddDbParameter(cmd, "@AttendanceStatus", DbType.String);
                            //2023-10-30 iwai-tamura upd-end ------



                            //パラメータ設定
                            var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                            parameters[0].Value = model.Search.EmployeeNo;                      //修正対象データ従業員
                            parameters[1].Value = model.Head.EmployeeNo;                        //修正者
                            parameters[2].Value = model.Search.Date.ToString("yyyyMMdd");       //修正対象勤務年月日
                            parameters[3].Value = DataConv.IfNull(updateBegin ? updateStampTimeBegin : null);    //開始打刻
                            parameters[4].Value = DataConv.IfNull(updateFinish ? updateStampTimeFinish : null);  //終了打刻
                            parameters[5].Value = updateBegin ? 1 : 0;                          //開始打刻回数
                            parameters[6].Value = updateFinish ? 1 : 0;                         //終了打刻回数
                            parameters[7].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //登録・更新年月日
                            //2023-10-30 iwai-tamura upd-str ------
                            parameters[8].Value = model.Input.AttendanceStatus;                 //勤務状況区分
                            //2023-10-30 iwai-tamura upd-end ------

                            if (cmd.ExecuteNonQuery() <= 0) {
                                throw new Exception("打刻処理エラー");
                            }
                        }

                        //勤怠実績履歴データ追加
                        //追加処理
                        sql = "INSERT INTO TA110勤怠実績履歴Data "
                            + " SELECT "
                            + "     @NowDatetime As 処理年月日"
                            + "     ,@UpdateType As 処理区分"
                            + "     ,@UpdateEmployeeNo As 処理者従業員Code"
                            + "     ,@Comment As 処理Comment"
                            + "     ,T実績.* "
                            + " FROM "
                            + "     TA100勤怠実績Data As T実績 "
                            + " WHERE "
                            + "     従業員Code = @TargetEmployeeNo"
                            + "     AND 勤務年月日 = @TargetDate"
                            + " ";
                        var cmd2 = dm.CreateCommand(sql);
                        DbHelper.AddDbParameter(cmd2, "@TargetEmployeeNo", DbType.String);
                        DbHelper.AddDbParameter(cmd2, "@UpdateEmployeeNo", DbType.String);
                        DbHelper.AddDbParameter(cmd2, "@TargetDate", DbType.String);
                        DbHelper.AddDbParameter(cmd2, "@UpdateType", DbType.String);
                        DbHelper.AddDbParameter(cmd2, "@Comment", DbType.String);
                        DbHelper.AddDbParameter(cmd2, "@NowDatetime", DbType.String);

                        //パラメータ設定
                        var parameters2 = cmd2.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters2[0].Value = model.Search.EmployeeNo;                         //修正対象データ従業員
                        parameters2[1].Value = model.Head.EmployeeNo;                           //修正者
                        parameters2[2].Value = model.Search.Date.ToString("yyyyMMdd");          //修正対象勤務年月日
                        //2023-10-30 iwai-tamura upd-end ------
                        parameters2[3].Value = model.Input.UpdateType;                          //修正区分(修正登録データ)
                        //parameters2[3].Value = "02";                                          //修正区分(修正登録データ)
                        //2023-10-30 iwai-tamura upd-end ------
                        parameters2[4].Value = model.Input.InputComment ?? "";       //備考
                        parameters2[5].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");    //登録・更新年月日
                        if (cmd2.ExecuteNonQuery() <= 0) {
                            throw new Exception("打刻処理エラー");
                        }
                    }
                    scope.Complete();
                }
                return true;
            }
            catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return false;
            }
            finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
    }
}
