using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ISCAttendanceMgmtSystem.COM.Models;
using ISCAttendanceMgmtSystem.COM.Enum;
using ISCAttendanceMgmtSystem.COM.Util.Convert;
using ISCAttendanceMgmtSystem.COM.Util.Database;
using System.Web;
using ISCAttendanceMgmtSystem.Log.Common;
using System.Reflection;
using System.Net.NetworkInformation;


namespace ISCAttendanceMgmtSystem.Bl.Top
{
    /// <summary>
    /// TOP画面用ビジネスロジック
    /// </summary>
    public class TopBL
    {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 初期表示
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>トップ画面用モデル</returns>
        public TopViewModels Index(string uid)
        {
            //開始
            nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

            try
            {
                //無記名関数
                //打刻データを表示用変換  HH:mm:dd
                Func<string, string,string> dbDateToViewDate = (val,format) => {
                    if (val == "") {
                        return "";
                    }
                    return DateTime.Parse(val).ToString(format);
                };

                TopViewModels top = new TopViewModels();
                TopViewModel topView = new TopViewModel();
                //SQL文で打刻データ抽出
                //開始
                var sql = " SELECT TA実績.* "
                        //2023-10-30 iwai-tamura upd-str ------
                        + "     ,TA勤務状況.勤務状況区分名 "
                        //2023-10-30 iwai-tamura upd-end ------
                        + " FROM TM010従業員情報Master TM基本 "
                        + "     LEFT JOIN TA100勤怠実績Data TA実績 "
                        + "         ON TM基本.従業員Code = TA実績.従業員Code "
                        + "         AND TA実績.勤務年月日 = @Date "
                        //2023-10-30 iwai-tamura upd-str ------
                        + "     LEFT JOIN TM030勤務状況区分Master TA勤務状況 "
                        + "         ON TA実績.勤務状況区分 = TA勤務状況.勤務状況区分 "
                        + "         AND TA勤務状況.発足日 <= @Date "
                        + "         AND TA勤務状況.廃止日 >= @Date "
                        //2023-10-30 iwai-tamura upd-end ------
                        + " WHERE"
                        + "     TM基本.従業員Code = @EmployeeNo ";

                using (DbManager dm = new DbManager())
                using (IDbCommand cmd = dm.CreateCommand(sql))
                using (DataSet ds = new DataSet())
                {
                    DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
                    DbHelper.AddDbParameter(cmd, "@Date", DbType.String);
                    ((IDbDataParameter)cmd.Parameters[0]).Value = uid;   //IDを指定
                    ((IDbDataParameter)cmd.Parameters[1]).Value = DateTime.Now.ToString("yyyyMMdd");   //年月日を指定
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);

                    // データセットに設定する
                    da.Fill(ds);

                    StringBuilder sb = new StringBuilder();
                    DataRow row = ds.Tables[0].Rows[0];
                    //2022-12-26 iwai-tamura upd-str ------
                    topView.ViewStartTime = dbDateToViewDate(row["開始打刻"].ToString(), "HH:mm:ss");
                    topView.ViewEndTime = dbDateToViewDate(row["終了打刻"].ToString(), "HH:mm:ss");
                    //topView.ViewStartTime = dbDateToViewDate(row["開始打刻"].ToString(), "HH:mm:dd");
                    //topView.ViewEndTime = dbDateToViewDate(row["終了打刻"].ToString(), "HH:mm:dd");
                    //2022-12-26 iwai-tamura upd-end ------

                    //2023-10-30 iwai-tamura upd-str ------
                    topView.ViewStatus = row["勤務状況区分名"].ToString();
                    //2023-10-30 iwai-tamura upd-end ------

                    top.TopView = topView;
                }

                //従業員の当日の勤務状況データを取得
                //2023-02-08 iwai-tamura upd-str ------

                sql = " SELECT "
                    + "     TM基本.*"
                    + "     ,TM所属.所属名"
                    + "     ,TA実績.* "
                    //2023-10-30 iwai-tamura upd-str ------
                    + "     ,TA勤務状況.勤務状況区分名 "
                    + "     ,TA勤務状況.勤務状況区分文字色 "
                    //2023-10-30 iwai-tamura upd-end ------
                    + "     ,TA履歴.処理年月日"
                    + "     ,TA履歴.処理区分"
                    + "     ,TA履歴.処理者従業員Code"
                    + "     ,TM処理者基本.氏名 AS 処理者氏名"
                    + "     ,TA履歴.処理Comment"
                    + " FROM TM010従業員情報Master TM基本 "
                    + "     LEFT JOIN TM022所属Master TM所属 "
                    + "         ON TM基本.所属Code = TM所属.所属Code "
                    + "     LEFT JOIN TA100勤怠実績Data TA実績 "
                    + "         ON TM基本.従業員Code = TA実績.従業員Code "
                    + "         AND TA実績.勤務年月日 = @Date "
                    //2023-10-30 iwai-tamura upd-str ------
                    + "     LEFT JOIN TM030勤務状況区分Master TA勤務状況 "
                    + "         ON TA実績.勤務状況区分 = TA勤務状況.勤務状況区分 "
                    + "         AND TA勤務状況.発足日 <= @Date "
                    + "         AND TA勤務状況.廃止日 >= @Date "
                    //2023-10-30 iwai-tamura upd-end ------
                    //2023-11-30 iwai-tamura upd-str ---
                    + "     LEFT JOIN TM010従業員情報Master TM本人 "
                    + "         ON TM本人.従業員Code = '" + uid + "'"
                    //2023-11-30 iwai-tamura upd-end ---
                    + " LEFT JOIN "
                    + " 	("
                    + " 		SELECT"
                    + " 			RANK() OVER(PARTITION BY 従業員Code,勤務年月日 ORDER BY 処理年月日 DESC) AS 順位"
                    + " 			,処理年月日"
                    + " 			,処理区分"
                    + " 			,処理者従業員Code"
                    + " 			,処理Comment"
                    + " 			,勤務年月日"
                    + " 			,従業員Code"
                    + " 		FROM TA110勤怠実績履歴Data"
                    //2023-10-30 iwai-tamura upd-str ------
                    + " 		WHERE 処理区分 >= '20'"  //本人は表示不要
                    //+ " 		WHERE 処理区分 <> '01'"
                    //2023-10-30 iwai-tamura upd-end ------
                    + " 	) TA履歴"
                    + " 	ON TA実績.従業員Code = TA履歴.従業員Code"
                    + " 		AND TA実績.勤務年月日 = TA履歴.勤務年月日"
                    + " 		AND TA履歴.順位 = 1"
                    + " LEFT JOIN TM010従業員情報Master TM処理者基本 "
                    + "     ON TM処理者基本.従業員Code = TA履歴.処理者従業員Code"
                    //2023-06-01 iwai-miki upd-str ---
                    + " WHERE 1=1 "
                    + "     AND (TM基本.退職年月日 = '' OR TM基本.退職年月日 >=  CONVERT(nvarchar,@Date,112)) "   //退職日を条件に追加
                    //2023-06-01 iwai-miki upd-end ---
                    //2023-10-30 iwai-tamura upd-str ---
                    + "     AND (TM基本.入社年月日 <> '' AND TM基本.入社年月日 <=  CONVERT(nvarchar,@Date,112)) "   //入社日を条件に追加
                    //2023-10-30 iwai-tamura upd-end ---
                    //2023-11-30 iwai-tamura upd-str ---
                    + "  ORDER BY "
                    + "     CASE WHEN TM所属.所属Code = TM本人.所属Code THEN 0 ELSE 1 END "
                    + "     ,TM所属.所属Code,TM基本.役職Code,TM基本.従業員Code   ";
                    //+ "  ORDER BY TM所属.所属Code,TM基本.役職Code,TM基本.従業員Code   ";
                    //2023-11-30 iwai-tamura upd-end ---
                //sql = " SELECT "
                //    + "     TM基本.*"
                //    + "     ,TM所属.所属名"
                //    + "     ,TA実績.* "
                //    + "      "
                //    + "      "
                //    + "      "
                //    + "      "
                //    + "      "
                //    + " FROM TM010従業員情報Master TM基本 "
                //    + "     LEFT JOIN TM022所属Master TM所属 "
                //    + "         ON TM基本.所属Code = TM所属.所属Code "
                //    + "     LEFT JOIN TA100勤怠実績Data TA実績 "
                //    + "         ON TM基本.従業員Code = TA実績.従業員Code "
                //    + "         AND TA実績.勤務年月日 = @Date "
                //    + "  ORDER BY TM所属.所属Code,TM基本.役職Code,TM基本.従業員Code   ";
                //2023-02-08 iwai-tamura upd-end ------


                using (DbManager dm = new DbManager())
                using (IDbCommand cmd = dm.CreateCommand(sql))
                using (DataSet ds = new DataSet())
                {
                    DbHelper.AddDbParameter(cmd, "@Date", DbType.String);
                    ((IDbDataParameter)cmd.Parameters[0]).Value = DateTime.Now.ToString("yyyyMMdd");   //年月日を指定
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);

                    // データセットに設定する
                    da.Fill(ds);

                    List<TodayAttendanceRecordViewListModel> AttendanceRecordList = new List<TodayAttendanceRecordViewListModel>();
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        TodayAttendanceRecordViewListModel AttendanceRecord = new TodayAttendanceRecordViewListModel{
                            EmployeeNo = row["従業員Code"].ToString(),
                            EmployeeName = row["氏名"].ToString(),
                            DepartmentNo = "9999",
                            DepartmentName = row["所属名"].ToString(),
                            //2023-10-30 iwai-tamura upd-str ------
                            AttendanceStatus = row["勤務状況区分名"].ToString(),
                            AttendanceStatusName = row["勤務状況区分名"].ToString(),
                            AttendanceStatusColor = row["勤務状況区分文字色"].ToString(),
                            //AttendanceStatus = "",
                            //2023-10-30 iwai-tamura upd-end ------
                            BiginTime = dbDateToViewDate(row["開始打刻"].ToString(), "HH:mm"),
                            FinishTime = dbDateToViewDate(row["終了打刻"].ToString(), "HH:mm"),
                            BiginTimePlan = "",
                            FinishTimePlan = "",
                            //2023-02-08 iwai-tamura upd-str ------
                            //2024-09-10 iwai-tamura upd-str ------
                            UpdateTime = dbDateToViewDate(row["処理年月日"].ToString(), "MM/dd HH:mm:ss"),
                            //UpdateTime = dbDateToViewDate(row["処理年月日"].ToString(), "HH:mm:ss"),
                            //2024-09-10 iwai-tamura upd-end ------
                            UpdateEmployeeName = row["処理者氏名"].ToString(),
                            UpdateComment = row["処理Comment"].ToString()
                            //2023-02-08 iwai-tamura upd-end ------
                        };
                        //2023-10-30 iwai-tamura upd-str ------
                        //勤務状況区分を追加した為、不要
                        //if (AttendanceRecord.FinishTime.Length>0) {
                        //    AttendanceRecord.AttendanceStatus = "退勤済";
                        //} else if(AttendanceRecord.BiginTime.Length > 0) {
                        //    AttendanceRecord.AttendanceStatus = "出勤";
                        //}
                        //2023-10-30 iwai-tamura upd-end ------
                        AttendanceRecordList.Add(AttendanceRecord);
                    }
                    top.AttendanceRecordViewList = AttendanceRecordList;
                }

                return top;
            }
            catch (Exception ex)
            {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return new TopViewModels();
            }
            finally
            {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 勤怠データ登録
        /// </summary>
        /// <param name="TopViewModels"></param>
        /// <param name="ユーザーID"></param>
        /// <param name="打刻区分"></param>
        /// <returns>成否</returns>
        //2023-10-30 iwai-tamura upd-str ------
        public bool AttendanceRegisterData(TopViewModels model, string uid, WorkStamp wStamp, string AttendanceStatus){
        //public bool AttendanceRegisterData(TopViewModels model, string uid, WorkStamp wStamp) {
        //2023-10-30 iwai-tamura upd-end ------
            try
            {

                DateTime entryDateTime = DateTime.Parse(model.TopView.StandardDateTime);
                string entryStampTimeBegin = "";
                string entryStampTimeFinish = "";
                int entryStampNumBegin = 0;
                int entryStampNumFinish = 0;

                bool bolWorkDate;
                var sql = " SELECT COUNT(*) "
                        + " FROM TA100勤怠実績Data "
                        +"  WHERE 従業員Code = @EmployeeNo and 勤務年月日 = @Date";
                using (DbManager dm = new DbManager())
                using (IDbCommand cmd = dm.CreateCommand(sql)) {
                    DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
                    DbHelper.AddDbParameter(cmd, "@Date", DbType.String);
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = uid;   //IDを指定
                    parameters[1].Value = DateTime.Now.ToString("yyyyMMdd");   //年月日を指定
                    bolWorkDate = (int)cmd.ExecuteScalar() > 0 ? true :false;   //存在チェック
                }

                using (var scope = new TransactionScope()) {
                    using (DbManager dm = new DbManager()) {
                        if (bolWorkDate) {
                            //更新処理
                            sql = "UPDATE TA100勤怠実績Data"
                                + " SET"
                                + "     打刻種類Flg = '1'"; //打刻種類Flg

                            //開始終了分岐
                            switch (wStamp) {
                                case WorkStamp.Begin:
                                    sql += "    ,開始打刻 = @StampTime"
                                        + "     ,開始打刻更新回数 = 開始打刻更新回数 + 1";
                                    break;
                                case WorkStamp.Finish:
                                    sql += "    ,終了打刻 = @StampTime" //申請種類Flg
                                        + "     ,終了打刻更新回数 = 終了打刻更新回数 + 1"; //予定終了打刻
                                    //2023-10-30 iwai-tamura upd-str ------
                                    sql += "    ,勤務状況区分 = @AttendanceStatus"; //勤務状況更新
                                    //2023-10-30 iwai-tamura upd-str ------
                                    break;

                                //2023-10-30 iwai-tamura upd-str ------
                                case WorkStamp.Status:
                                case WorkStamp.StatusBigin:
                                    sql += "    ,勤務状況区分 = @AttendanceStatus"; //勤務状況更新
                                    break;
                                //2023-10-30 iwai-tamura upd-end ------
                            }

                            //共通更新
                            sql += "    ,最終更新者Code = @EmployeeNo"
                                + "     ,更新年月日 = @NowDatetime"
                                + "     ,更新回数 = 更新回数 + 1";

                            //条件文
                            sql += " WHERE "
                                + "     従業員Code = @EmployeeNo"
                                + "     AND 勤務年月日 = @TargetDate";

                            var cmd = dm.CreateCommand(sql);
                            DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
                            DbHelper.AddDbParameter(cmd, "@TargetDate", DbType.String);
                            DbHelper.AddDbParameter(cmd, "@StampTime", DbType.String);
                            DbHelper.AddDbParameter(cmd, "@NowDatetime", DbType.String);
                            //2023-10-30 iwai-tamura upd-str ------
                            DbHelper.AddDbParameter(cmd, "@AttendanceStatus", DbType.String);
                            //2023-10-30 iwai-tamura upd-end ------
                            //パラメータ設定
                            var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                            parameters[0].Value = uid;                                                              //IDを指定
                            parameters[1].Value = entryDateTime.ToString("yyyyMMdd");                               //勤務年月日
                            parameters[2].Value = DataConv.IfNull(entryDateTime.ToString("yyyy-MM-dd HH:mm:ss"));   //打刻時間
                            parameters[3].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");                     //登録・更新年月日
                            //2023-10-30 iwai-tamura upd-str ------
                            parameters[4].Value = AttendanceStatus;                                                 //勤務状況区分
                            //2023-10-30 iwai-tamura upd-end ------
                            if (cmd.ExecuteNonQuery() <= 0) {
                                throw new Exception("打刻処理エラー");
                            }

                        } else {
                            //追加処理
                            sql = "INSERT INTO TA100勤怠実績Data"
                                + " VALUES ("
                                + " @EmployeeNo"
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
                                + " ,@EmployeeNo"
                                + " ,@EmployeeNo"
                                + " ,@NowDatetime"
                                + " ,@NowDatetime"
                                + " ,'1'"
                                + " )";
                            switch (wStamp) {
                                case WorkStamp.Begin:
                                    entryStampTimeBegin = entryDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                                    entryStampTimeFinish = null;
                                    entryStampNumBegin = 1;
                                    entryStampNumFinish = 0;
                                    break;
                                case WorkStamp.Finish:
                                    entryStampTimeBegin = null;
                                    entryStampTimeFinish = entryDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                                    entryStampNumBegin = 0;
                                    entryStampNumFinish = 1;
                                    break;
                            }
                            var cmd = dm.CreateCommand(sql);
                            DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
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
                            parameters[0].Value = uid;                                          //IDを指定
                            parameters[1].Value = entryDateTime.ToString("yyyyMMdd");           //勤務年月日
                            parameters[2].Value = DataConv.IfNull(entryStampTimeBegin);         //開始打刻
                            parameters[3].Value = DataConv.IfNull(entryStampTimeFinish);        //終了打刻
                            parameters[4].Value = entryStampNumBegin;                           //開始打刻回数
                            parameters[5].Value = entryStampNumFinish;                          //終了打刻回数
                            parameters[6].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //登録・更新年月日
                            //2023-10-30 iwai-tamura upd-str ------
                            parameters[7].Value = AttendanceStatus; //勤務状況区分
                            //2023-10-30 iwai-tamura upd-end ------
                            if (cmd.ExecuteNonQuery() <= 0) {
                                throw new Exception("打刻処理エラー");
                            }
                        }

                        //2023-02-01 iwai-tamura upd-str ------
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
                        parameters2[0].Value = uid;                                             //修正対象データ従業員
                        parameters2[1].Value = uid;                                             //修正者
                        parameters2[2].Value = entryDateTime.ToString("yyyyMMdd");              //修正対象勤務年月日

                        //2023-11-30 iwai-tamura upd-str ------
                        //2023-10-30 iwai-tamura upd-str ------
                        switch (wStamp) {
                            case WorkStamp.Begin:
                                parameters2[3].Value = "01";                                    //処理区分(本人登録) 出勤処理
                                break;
                            case WorkStamp.Finish:
                                parameters2[3].Value = "02";                                    //処理区分(本人登録) 退勤処理
                                break;
                            case WorkStamp.Status:
                                parameters2[3].Value = "10";                                    //処理区分(本人変更) ステータスの変更時
                                break;
                            case WorkStamp.StatusBigin:
                                parameters2[3].Value = "11";                                    //処理区分(本人変更) 出勤処理時のステータス変更
                                break;
                        }
                        //switch (wStamp) {
                        //    case WorkStamp.Begin:
                        //    case WorkStamp.StatusBigin:
                        //        parameters2[3].Value = "01";                                    //処理区分(本人登録) 出勤処理・出勤処理時のステータス変更
                        //        break;
                        //    case WorkStamp.Finish:
                        //        parameters2[3].Value = "02";                                    //処理区分(本人登録) 退勤処理
                        //        break;
                        //    case WorkStamp.Status:
                        //        parameters2[3].Value = "10";                                    //処理区分(本人変更) ステータスの変更時
                        //        break;
                        //}
                        ////parameters2[3].Value = "01";                                            //修正区分(本人登録データ)
                        //2023-10-30 iwai-tamura upd-end ------
                        //2023-11-30 iwai-tamura upd-end ------

                        parameters2[4].Value = "";                                              //備考
                        parameters2[5].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");    //登録・更新年月日
                        if (cmd2.ExecuteNonQuery() <= 0) {
                            throw new Exception("打刻処理エラー");
                        }
                        //2023-02-01 iwai-tamura upd-end ------
                    }
                    scope.Complete();
                }
                
                return true;
            }
            catch (Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return false;
            }
            finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// 出勤処理
        /// </summary>
        /// <param name="TopViewModels"></param>
        /// <param name="ユーザーID"></param>
        /// <returns>トップ画面用モデル</returns>

        public bool WorkBegin(TopViewModels model, string uid)
        {
            //開始
            nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
            try
            {
                //出勤打刻データ登録
                //2023-10-30 iwai-tamura upd-str ------
                return AttendanceRegisterData(model, uid, WorkStamp.Begin, ((int)AttendanceStatus.Attend).ToString("00"));
                //return AttendanceRegisterData(model, uid, WorkStamp.Begin);
                //2023-10-30 iwai-tamura upd-end ------

            }
            catch (Exception ex)
            {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return false;
            }
            finally
            {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// 退勤処理
        /// </summary>
        /// <param name="TopViewModels"></param>
        /// <returns>トップ画面用モデル</returns>

        public bool WorkFinish(TopViewModels model, string uid)
        {
            //開始
            nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

            try
            {
                //出勤打刻データ登録
                //2023-10-30 iwai-tamura upd-str ------
                return AttendanceRegisterData(model, uid, WorkStamp.Finish, ((int)AttendanceStatus.Workout).ToString("00"));
                //return AttendanceRegisterData(model, uid, WorkStamp.Finish);
                //2023-10-30 iwai-tamura upd-end ------
            }
            catch (Exception ex)
            {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return false;
            }
            finally
            {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }

        }


        //2023-10-30 iwai-tamura upd-str ------
        /// <summary>
        /// 出勤状況変更処理(ステータス)
        /// </summary>
        /// <param name="TopViewModels"></param>
        /// <returns>トップ画面用モデル</returns>

        public bool ChangeStatus(TopViewModels model, string uid,string AttendanceStatus,bool bolBigin) {
            //開始
            nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

            try
            {
                //出勤打刻データ登録
                if (bolBigin){
                    return AttendanceRegisterData(model, uid, WorkStamp.StatusBigin, AttendanceStatus);
                } else{
                    return AttendanceRegisterData(model, uid, WorkStamp.Status, AttendanceStatus);

                }


            }
            catch (Exception ex)
            {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return false;
            }
            finally
            {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }

        }
        //2023-10-30 iwai-tamura upd-end ------


        /// <summary>
        /// ログイン状態か判定
        /// </summary>
        /// <returns>判定結果</returns>
        public bool IsLogin()
        {
            return HttpContext.Current.Session["LoginUser"] == null ? false : true;
        }

    }
}