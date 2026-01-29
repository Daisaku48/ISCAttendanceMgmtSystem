using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ISCAttendanceMgmtSystem.BL.Common;
using ISCAttendanceMgmtSystem.COM.Entity.Session;
using ISCAttendanceMgmtSystem.COM.Models;
using ISCAttendanceMgmtSystem.COM.Util.Config;
using ISCAttendanceMgmtSystem.COM.Util.Convert;
using ISCAttendanceMgmtSystem.COM.Util.Database;
using ISCAttendanceMgmtSystem.COM.Util.Zip;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;


namespace ISCAttendanceMgmtSystem.BL.AttendanceRecordManagerView {
    /// <summary>
    /// 管理者用勤務実績確認
    /// </summary>
    public class AttendanceRecordManagerViewBL {
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
        public AttendanceRecordManagerViewBL(string fullPath)
        {
            Configuration config = WebConfig.GetConfigFile();
            ////帳票作成ディレクトリを取得
            //TempDir = config.AppSettings.Settings["DOWNLOAD_TEMP_DIR_O"].Value;

            ////末尾がpathの区切り文字かしらべ、違っていたら追加する。
            //if(!(TempDir.EndsWith(Path.DirectorySeparatorChar.ToString()))) {
            //    TempDir = TempDir + Path.DirectorySeparatorChar;
            //}

            TempDir = fullPath;

            //帳票作成ディレクトリ内保存日数を取得(int変換に失敗した場合は3とする。)
            if (!int.TryParse(config.AppSettings.Settings["RETENTIO_PERIOD"].Value, out R_P)) { R_P = 3; };
        }


        /// <summary>
        /// 従業員一覧取得
        /// </summary>
        public IList<SelectListItem> AttendanceRecordManagerViewEmployeeList(LoginUser lu) {

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
                //2023-03-01 iwai-tamura upd-str ---
                case "3":
                //2023-03-01 iwai-tamura upd-end ---
                case "4":
                case "5":
                case "9":
                    //担当管理者、管理職、総務職
                    sqlWhere = " WHERE 1=1 "
                            + "     AND ( "
                            + "         所属Code BETWEEN @ManageDepartmentStart AND @ManageDepartmentEnd"
                            + "         OR 所属Code in(@ManageDepartment1,@ManageDepartment2,@ManageDepartment3,@ManageDepartment4,@ManageDepartment5)"
                            + "         OR 従業員Code in(@ManageEmployee1,@ManageEmployee2,@ManageEmployee3,@ManageEmployee4,@ManageEmployee5)"
                            //2023-03-01 iwai-tamura upd-str ---
                            + "         OR 従業員Code = @Myself"
                            //2023-03-01 iwai-tamura upd-end ---
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
                //2023-10-30 iwai-tamura upd-str ------
                itemList.Add(new SelectListItem { Value = "ALL,,", Text = "全件(DLのみ)" });
                //itemList.Add(new SelectListItem { Value = "ALL", Text = "全件(DLのみ)" });
                //2023-10-30 iwai-tamura upd-end ------
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
        /// <param name="model">勤務実績検索モデル</param>
        /// <returns>検索モデル</returns>
        public AttendanceRecordManagerViewModels Search(AttendanceRecordManagerViewModels model, LoginUser lu) {
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


                //該当月の日付を取得
                DateTime CalendarDate = DateTime.Parse(model.Search.EntryYear + "/" + model.Search.EntryMonth + "/" + "1");

                //該当月の日数を取得
                int CalendarDays = DateTime.DaysInMonth(int.Parse(model.Search.EntryYear) , int.Parse(model.Search.EntryMonth));

                //該当月の月末を取得
                DateTime CalendarEndDate = DateTime.Parse(model.Search.EntryYear + "/" + model.Search.EntryMonth + "/" + CalendarDays.ToString());

                var sql = " WITH DateTable(BasicDate) AS ( "
                        + "   SELECT"
                        + "     CONVERT(DATETIME, '{0}') "
                        + "   UNION ALL "
                        + "   SELECT"
                        + "     DATEADD(d, 1, BasicDate) "
                        + "   FROM"
                        + "     DateTable "
                        + "   WHERE"
                        + "     BasicDate < CONVERT(DATETIME, '{1}')"
                        + " ) "
                        + " SELECT"
                        + " 	BasicDate as 暦日付,T勤怠.*"
                        + " FROM"
                        + "   DateTable as DT left join TA100勤怠実績Data T勤怠 on CONVERT(nvarchar,DT.BasicDate,112) = T勤怠.勤務年月日 and T勤怠.従業員Code = '{2}'";


                sql = string.Format(sql, CalendarDate.ToString("yyyy/MM/dd"), CalendarEndDate.ToString("yyyy/MM/dd"), model.Search.EmployeeNo);
                using (DbManager dm = new DbManager())
                using(IDbCommand cmd = dm.CreateCommand(sql))
                using(DataSet ds = new DataSet()) {
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    // データセットに設定する
                    da.Fill(ds);

                    List<AttendanceRecordManagerViewListModel> resultList = new List<AttendanceRecordManagerViewListModel>();
                    foreach (DataRow row in ds.Tables[0].Rows) {

                        //打刻データを挿入
                        AttendanceRecordManagerViewListModel result = new AttendanceRecordManagerViewListModel {
                            AttendanceDate = CalendarDate.ToString(),
                            ViewDay = CalendarDate.ToString("dd"),
                            ViewWeek = CalendarDate.ToString("(ddd)"),
                            EmployeeNo = row["従業員Code"].ToString(),
                            StampTimeBegin = dbDateToViewDate(row["開始打刻"].ToString()),
                            StampTimeFinish = dbDateToViewDate(row["終了打刻"].ToString())
                        };
                        resultList.Add(result);
                        CalendarDate = CalendarDate.AddDays(1);    //日付を加算
                    }

                    model.SearchResult = resultList;
                }
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
        /// Excel出力
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string ExcelOutput(AttendanceRecordManagerViewModels model,LoginUser lu) {

            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //現在日時を取得
                DateTime NowDate = DateTime.Now;
                string strUserCode = ((ISCAttendanceMgmtSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).UserCode.ToString();
                string strDepartmentNo = ((ISCAttendanceMgmtSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).DepartmentNo.ToString();

                //帳票作成フォルダを用意
                string strWorkFolder = "";
                strWorkFolder = TempDir + NowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar +
                    strUserCode + NowDate.ToString("yyyyMMddHHmmss") + Path.DirectorySeparatorChar;
                System.IO.Directory.CreateDirectory(strWorkFolder);

                //作成フォルダ内ファイル一覧を取得
                foreach (string file in System.IO.Directory.GetDirectories(TempDir, "*")) {
                    DateTime oldDirDate = new DateTime();
                    oldDirDate = System.IO.Directory.GetCreationTime(file); //作成日時を取得

                    //削除基準日より古いフォルダを削除
                    if (oldDirDate < NowDate.AddDays(-(R_P))) {
                        DeleteDirectory(file);
                    }
                }

                //zip作成フォルダとzipファイル名を用意を用意
                string strZipFolder = TempDir + NowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar;
                string strZipName = strUserCode + NowDate.ToString("yyyyMMddHHmmss") + ".zip";
                //return用path文字列を用意
                string zipReturnPath = NowDate.ToString("yyyyMMdd") + Path.DirectorySeparatorChar + strZipName;


                //excelファイルの作成
                //ファイル名作成
                //[対象年月]_[対象タイトル]_[対象データ]_[出力日(yyyyMMdd)-出力時間(HHmmss)]_[従業員ID].xlsx
                string strTargetName = "";
                if (model.Search.EmployeeNo == "ALL") {
                    strTargetName = "対象データ全件";
                } else {
                    strTargetName = Regex.Replace(model.Search.EmployeeName, @"\s+", ""); 
                }

                string fileName = model.Search.EntryYear + model.Search.EntryMonth.PadLeft(2,'0') + "_" + "勤務実績" + "_" + strTargetName + "_" + NowDate.ToString("yyyyMMdd-HHmmss") + "_" +strUserCode + ".xlsx";

                //データ取得
                DataTable dt = new DataTable();
                dt = GetOutputData(model, lu);

                // EPPlus使用版
                var outputFile = new FileInfo(strWorkFolder + fileName);
                if (outputFile.Exists) {
                    outputFile.Delete();
                }
                using (var excel = new ExcelPackage(outputFile)) {
                    // 初回シート追加
                    var sheet = excel.Workbook.Worksheets.Add((string)dt.Rows[0]["氏名"]);
                    string targetUser = (string)dt.Rows[0]["従業員Code"];

                    int intRow = 1; //シート内行数

                    //初回項目出力
                    for (int i = 0; i < dt.Columns.Count; i++){
                        sheet.Cells[intRow, i+1].Value = dt.Columns[i].ColumnName;
                    }
                    intRow++;

                    //勤務実績データ詳細出力
                    for (int i = 0; i < dt.Rows.Count; i++) {
                        //従業員毎にシートを作成
                        if (targetUser != (string)dt.Rows[i]["従業員Code"]) {
                            sheet = excel.Workbook.Worksheets.Add((string)dt.Rows[i]["氏名"]);
                            targetUser = (string)dt.Rows[i]["従業員Code"];

                            intRow = 1;  //シート内行数初期化

                            //項目出力
                            for (int j = 0; j < dt.Columns.Count; j++) {
                                sheet.Cells[intRow, j+1].Value = dt.Columns[j].ColumnName;
                            }
                            intRow++;
                        }
                        for (int j = 0; j < dt.Columns.Count; j++) {
                            sheet.Cells[intRow, j+1].Value = dt.Rows[i][j].ToString();
                        }
                        intRow++;
                    }

                    // 保存
                    excel.Save();
                }

                //作成したファイルに読み取り専用プロパティを設定
                //ネット経由でダウンロードされたファイルを保護されたビューで開くために必要
                FileAttributes fas = File.GetAttributes(strWorkFolder + fileName);
                fas = fas | FileAttributes.ReadOnly;
                File.SetAttributes(strWorkFolder + fileName, fas);

                //課題--pdf複数出力で、どれか１つにエラーがあってもダウンロードできてしまう。
                //圧縮
                string strZipFullPath = "";
                var compress = new Compress();
                strZipFullPath = compress.CreateZipFile(strZipName, strZipFolder, strWorkFolder);

                //zipファイルパスをセット
                return zipReturnPath;


            }
            catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            }
            finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 出力データ取得
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DataTable GetOutputData(AttendanceRecordManagerViewModels model,LoginUser lu) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string year = model.Search.EntryYear;                     // 出力年
                string month = model.Search.EntryMonth;                     // 出力年

                //該当月の日付を取得
                DateTime startDate = DateTime.Parse(model.Search.EntryYear + "/" + model.Search.EntryMonth + "/" + "1");

                //該当月の日数を取得
                int days = DateTime.DaysInMonth(int.Parse(model.Search.EntryYear), int.Parse(model.Search.EntryMonth));

                //該当月の月末を取得
                DateTime endDate = DateTime.Parse(model.Search.EntryYear + "/" + model.Search.EntryMonth + "/" + days.ToString());

                // 対象月カレンダーを取得し勤怠データと合成
                string sql = "";                                //実行するクエリを保持
                sql = " WITH DateTable(BasicDate) AS ( "
                    + " SELECT "
                    + "   CONVERT(DATETIME, '{0}')  "
                    + " UNION ALL  "
                    + " SELECT "
                    + "   DATEADD(d, 1, BasicDate)  "
                    + " FROM "
                    + "   DateTable  "
                    + " WHERE "
                    + "   BasicDate < CONVERT(DATETIME, '{1}') "
                    + "  ) "
                    + "  "
                    + "   SELECT "
                    + " 	Main.従業員Code "
                    + " 	,Main.氏名 "
                    + " 	,Main.所属Code "
                    + " 	,TM所属.所属名 "
                    + " 	,Main.役職Code "
                    + " 	,TM役職.役職名 "
                    + "  	,CONVERT(VARCHAR,BasicDate,111) as 年月日 "
                    + "  	,DATENAME(WEEKDAY, BasicDate) as 曜日 "
                    + "  	,ISNULL(FORMAT(T勤怠.開始打刻,'HH:mm:ss'),'') as 出勤時間 "
                    + "  	,ISNULL(FORMAT(T勤怠.終了打刻,'HH:mm:ss'),'') as 退勤時間 "
                    + "  	,IIF((T勤怠.開始打刻 Is null) Or (T勤怠.開始打刻 Is null),'','01:00:00') as 休憩時間 "

                    //2023-04-13 iwai-tamura upd-str ------
                    + "  	,TA履歴.処理年月日 as 修正実施時刻"
                    + "  	,TM処理者基本.氏名 as 修正者 "
                    + "  	,TA履歴.処理Comment as 事由 "
                    //2023-04-13 iwai-tamura upd-end ------

                    //2023-10-30 iwai-tamura upd-str ------
                    + " 	,ISNULL(FORMAT(TA本人出勤.開始打刻,'HH:mm:ss'),'') as 打刻実績出勤時間"
                    + " 	,ISNULL(FORMAT(TA本人退勤.終了打刻,'HH:mm:ss'),'') as 打刻実績退勤時間"
                    //2023-10-30 iwai-tamura upd-end ------

                    + "  FROM "
                    //2023-10-30 iwai-tamura upd-str ------
                    + " 	(SELECT 従業員Code,氏名,所属Code,役職Code,入社年月日,退職年月日 FROM TM010従業員情報Master WHERE {2}) AS Main "
                    //+ " 	(SELECT 従業員Code,氏名,所属Code,役職Code FROM TM010従業員情報Master WHERE {2}) AS Main "
                    //2023-10-30 iwai-tamura upd-end ------
                    + " 	Left Join DateTable as DT On 1=1 "
                    + " 	Left Join TA100勤怠実績Data T勤怠 On CONVERT(nvarchar,DT.BasicDate,112) = T勤怠.勤務年月日 AND Main.従業員Code=T勤怠.従業員Code "

                    //2023-04-13 iwai-tamura upd-str ------
                    + "     LEFT JOIN "
                    + " 	    ("
                    + " 	    	SELECT"
                    + " 	    		RANK() OVER(PARTITION BY 従業員Code,勤務年月日 ORDER BY 処理年月日 DESC) AS 順位"
                    + " 	    		,処理年月日"
                    + " 	    		,処理区分"
                    + " 	    		,処理者従業員Code"
                    + " 	    		,処理Comment"
                    + " 	    		,勤務年月日"
                    + " 	    		,従業員Code"
                    + " 	    	FROM TA110勤怠実績履歴Data"
                    //2023-11-30 iwai-tamura upd-str ------
                    //2023-10-30 iwai-tamura upd-str ------
                    + " 	    	WHERE NOT (処理区分 LIKE '0%' OR 処理区分 LIKE '1%')"
                    //+ " 	    	WHERE not 処理区分 like '0%'"
                    ////+ " 	    	WHERE 処理区分 <> '01'"
                    //2023-10-30 iwai-tamura upd-end ------
                    //2023-11-30 iwai-tamura upd-end ------
                    + " 	    ) TA履歴"
                    + " 	    ON T勤怠.従業員Code = TA履歴.従業員Code"
                    + " 		    AND T勤怠.勤務年月日 = TA履歴.勤務年月日"
                    + " 		    AND TA履歴.順位 = 1"
                    + "     LEFT JOIN TM010従業員情報Master TM処理者基本 "
                    + "         ON TM処理者基本.従業員Code = TA履歴.処理者従業員Code"
                    //2023-04-13 iwai-tamura upd-end ------

                    //2023-10-30 iwai-tamura upd-str ------
                    + "     LEFT JOIN "
                    + " 	    ("
                    + " 	    	SELECT"
                    + " 	    		RANK() OVER(PARTITION BY 従業員Code,勤務年月日 ORDER BY 処理年月日 DESC) AS 順位"
                    + " 	    		,処理年月日"
                    + " 	    		,処理区分"
                    + " 	    		,処理者従業員Code"
                    + " 	    		,処理Comment"
                    + " 	    		,勤務年月日"
                    + " 	    		,従業員Code"
                    + " 	    		,開始打刻"
                    + " 	    		,終了打刻"
                    + " 	    	FROM TA110勤怠実績履歴Data"
                    + " 	    	WHERE 処理区分 = '01'"
                    + " 	    ) TA本人出勤"
                    + " 	    ON T勤怠.従業員Code = TA本人出勤.従業員Code"
                    + " 		    AND T勤怠.勤務年月日 = TA本人出勤.勤務年月日"
                    + " 		    AND TA本人出勤.順位 = 1"
                    + "     LEFT JOIN "
                    + " 	    ("
                    + " 	    	SELECT"
                    + " 	    		RANK() OVER(PARTITION BY 従業員Code,勤務年月日 ORDER BY 処理年月日 DESC) AS 順位"
                    + " 	    		,処理年月日"
                    + " 	    		,処理区分"
                    + " 	    		,処理者従業員Code"
                    + " 	    		,処理Comment"
                    + " 	    		,勤務年月日"
                    + " 	    		,従業員Code"
                    + " 	    		,開始打刻"
                    + " 	    		,終了打刻"
                    + " 	    	FROM TA110勤怠実績履歴Data"
                    + " 	    	WHERE 処理区分 = '02'"
                    + " 	    ) TA本人退勤"
                    + " 	    ON T勤怠.従業員Code = TA本人退勤.従業員Code"
                    + " 		    AND T勤怠.勤務年月日 = TA本人退勤.勤務年月日"
                    + " 		    AND TA本人退勤.順位 = 1"
                    //2023-10-30 iwai-tamura upd-end ------



                    + " 	Left Join TM022所属Master TM所属 On Main.所属Code = TM所属.所属Code "
                    + " 	Left Join TM027役職Master TM役職 On Main.役職Code = TM役職.役職Code ";


                string sqlWhere = "";
                string sqlOrderby = "";
                switch (lu.AdminType) {
                    //2023-03-01 iwai-tamura upd-str ---
                    case "3":
                    //2023-03-01 iwai-tamura upd-end ---
                    case "4":
                    case "5":
                    case "9":
                        //担当管理者、管理職、総務職
                        sqlWhere = " WHERE 1=1 "
                                + "     AND ( "
                                + "         Main.所属Code BETWEEN @ManageDepartmentStart AND @ManageDepartmentEnd"
                                + "         OR Main.所属Code in(@ManageDepartment1,@ManageDepartment2,@ManageDepartment3,@ManageDepartment4,@ManageDepartment5)"
                                + "         OR Main.従業員Code in(@ManageEmployee1,@ManageEmployee2,@ManageEmployee3,@ManageEmployee4,@ManageEmployee5)"
                                //2023-03-01 iwai-tamura upd-str ---
                                + "         OR Main.従業員Code = @Myself"
                                //2023-03-01 iwai-tamura upd-end ---
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
                //2023-10-30 iwai-tamura upd-str ------
                sqlWhere += "  AND CASE Main.入社年月日 WHEN '' THEN '000000' ELSE LEFT(Main.入社年月日,6) END <= " + startDate.ToString("yyyyMM");
                sqlWhere += "  AND CASE Main.退職年月日 WHEN '' THEN '999999' ELSE LEFT(Main.退職年月日,6) END >= " + endDate.ToString("yyyyMM");
                //2023-10-30 iwai-tamura upd-end ------

                //2023-04-13 iwai-tamura upd-str ------
                sqlOrderby = "  ORDER BY Main.所属Code,Main.役職Code,Main.従業員Code,CONVERT(nvarchar,DT.BasicDate,112) ";
                //sqlOrderby = "  ORDER BY Main.所属Code,Main.役職Code,Main.従業員Code   ";
                //2023-04-13 iwai-tamura upd-end ------


                string sqlTarget = "";
                if (model.Search.EmployeeNo == "ALL") {
                    sqlTarget = "1=1";
                } else {
                    sqlTarget = "従業員Code = " + model.Search.EmployeeNo;
                }
                sql = string.Format(sql, startDate.ToString("yyyy/MM/dd"), endDate.ToString("yyyy/MM/dd"), sqlTarget) + sqlWhere + sqlOrderby;

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
                    da.Fill(ds);        // データセットに設定する
                    return ds.Tables[0];
                }
            }
            catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            }
            finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 読み取り専用属性を付けられたファイルを含むディレクトリの削除
        /// </summary>
        /// <param name="dir"></param>
        private void DeleteDirectory(string dir) {
            //DirectoryInfoオブジェクトの作成
            DirectoryInfo di = new DirectoryInfo(dir);

            //フォルダ以下のすべてのファイル、フォルダの属性を削除
            RemoveReadonlyAttribute(di);

            //フォルダを根こそぎ削除
            di.Delete(true);
        }

        /// <summary>
        /// 読み取り専用属性の解除
        /// </summary>
        /// <param name="dirInfo"></param>
        private void RemoveReadonlyAttribute(DirectoryInfo dirInfo) {
            //基のフォルダの属性を変更
            if ((dirInfo.Attributes & FileAttributes.ReadOnly) ==
                FileAttributes.ReadOnly)
                dirInfo.Attributes = FileAttributes.Normal;
            //フォルダ内のすべてのファイルの属性を変更
            foreach (FileInfo fi in dirInfo.GetFiles()) {
                if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                    fi.Attributes = FileAttributes.Normal;
                }
            }
            //サブフォルダの属性を回帰的に変更
            foreach (DirectoryInfo di in dirInfo.GetDirectories()) {
                RemoveReadonlyAttribute(di);
            }
        }
    }
}
