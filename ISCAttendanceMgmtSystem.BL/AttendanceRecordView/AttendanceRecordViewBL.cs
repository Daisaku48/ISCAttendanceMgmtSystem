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
using ISCAttendanceMgmtSystem.COM.Models;
using ISCAttendanceMgmtSystem.COM.Util.Config;
using ISCAttendanceMgmtSystem.COM.Util.Convert;
using ISCAttendanceMgmtSystem.COM.Util.Database;
using ISCAttendanceMgmtSystem.COM.Util.Zip;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Style;


namespace ISCAttendanceMgmtSystem.BL.AttendanceRecordView {
    /// <summary>
    /// 勤務実績確認
    /// </summary>
    public class AttendanceRecordViewBL {
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
        public AttendanceRecordViewBL(string fullPath)
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
        /// 検索
        /// </summary>
        /// <param name="model">自己申告書検索モデル</param>
        /// <returns>検索モデル</returns>
        public AttendanceRecordViewModels Search(AttendanceRecordViewModels model, LoginUser lu) {
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

                    List<AttendanceRecordViewListModel> resultList = new List<AttendanceRecordViewListModel>();
                    foreach (DataRow row in ds.Tables[0].Rows) {

                        //打刻データを挿入
                        AttendanceRecordViewListModel result = new AttendanceRecordViewListModel {
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
        public string ExcelOutput(AttendanceRecordViewModels model) {

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
                //[対象年度]_[対象タイトル]_[対象支社]_[出力日時].xlsx
                string strTitleName = "";
                strTitleName = "勤務実績データ";

                //2023-02-28 iwai-tamura upd-str ------
                string fileName = model.Search.EntryYear + "年" + model.Search.EntryMonth.PadLeft(2, '0') + "月_" + strTitleName + "_" + NowDate.ToString("yyyyMMddHHmmss") + ".xlsx";
                //string fileName = model.Search.EntryYear + "年" + model.Search.EntryMonth + "月_" + strTitleName + "_" + NowDate.ToString("yyyyMMddHHmmss") + ".xlsx";
                //2023-02-28 iwai-tamura upd-end ------

                //データ取得
                DataTable dt = new DataTable();
                dt = GetOutputData(model);

                // EPPlus使用版
                var outputFile = new FileInfo(strWorkFolder + fileName);
                if (outputFile.Exists) {
                    outputFile.Delete();
                }
                using (var excel = new ExcelPackage(outputFile)) {
                    // シート追加
                    var sheet = excel.Workbook.Worksheets.Add("Sheet1");

                    int x = 1;
                    int y = 1;
                    //勤務表用固定項目
                    //1行目
                    var cell = sheet.Cells[1, 3];
                    // セルに値設定
                    cell.Value = "勤務表転記用データ";
                    // そのままだとフォントが英語圏のフォントなので調整
                    cell.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                    cell = sheet.Cells[1, 3, 1, 8];
                    cell.Merge = true;   //セルを結合
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);  //罫線

                    //2行目
                    //年月日
                    cell = sheet.Cells[2, 1];
                    // セルに値設定
                    cell.Value = "年月日";
                    // そのままだとフォントが英語圏のフォントなので調整
                    cell.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);  //罫線

                    //曜
                    cell = sheet.Cells[2, 2];
                    // セルに値設定
                    cell.Value = "曜日";
                    // そのままだとフォントが英語圏のフォントなので調整
                    cell.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);  //罫線

                    //出勤時間
                    cell = sheet.Cells[2, 3];
                    // セルに値設定
                    cell.Value = "出勤時間";
                    // そのままだとフォントが英語圏のフォントなので調整
                    cell.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                    cell = sheet.Cells[2, 3, 2, 4];
                    cell.Merge = true;   //セルを結合
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);  //罫線

                    //退勤時間
                    cell = sheet.Cells[2, 5];
                    // セルに値設定
                    cell.Value = "退勤時間";
                    // そのままだとフォントが英語圏のフォントなので調整
                    cell.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                    cell = sheet.Cells[2, 5, 2, 6];
                    cell.Merge = true;   //セルを結合
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);  //罫線

                    //休憩時間
                    cell = sheet.Cells[2, 7];
                    // セルに値設定
                    cell.Value = "休憩時間";
                    // そのままだとフォントが英語圏のフォントなので調整
                    cell.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                    cell = sheet.Cells[2, 7, 2, 8];
                    cell.Merge = true;   //セルを結合
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;   //中央寄せ
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);  //罫線

                    //項目行背景色
                    sheet.Cells[1, 1, 2, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[1, 1, 2, 8].Style.Fill.BackgroundColor.SetColor(Color.GreenYellow);



                    //次の行頭へ
                    x = 3;
                    y = 1;

                    //データを挿入
                    foreach (DataRow dr in dt.Rows) {
                        foreach (object trg in dr.ItemArray) {
                            cell = sheet.Cells[x, y];
                            if (trg != null && trg.ToString().Length != 0) {
                                cell.Value = trg.ToString();
                            }
                            cell.Style.Font.SetFromFont(new Font("MS Gothic", 10, FontStyle.Regular));
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);  //罫線
                            y++;
                        }

                        //2023-01-05 iwai-tamura add-str ------
                        //出勤時間
                        if (dr["出勤時間"] != null && dr["出勤時間"].ToString().Length != 0) {
                            cell = sheet.Cells[x, 3];
                            cell.Value = dr["出勤時間"];
                            cell.Style.Numberformat.Format = "H:mm";
                        }
                        //2023-01-05 iwai-tamura add-end ------
                        cell = sheet.Cells[x, 3, x, 4];
                        cell.Merge = true;   //出勤時間のセルを結合
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);  //罫線

                        //2023-01-05 iwai-tamura add-str ------
                        //退勤時間
                        if (dr["退勤時間"] != null && dr["退勤時間"].ToString().Length != 0) {
                            cell = sheet.Cells[x, 5];
                            cell.Value = dr["退勤時間"];
                            cell.Style.Numberformat.Format = "H:mm";
                        }
                        //2023-01-05 iwai-tamura add-end ------
                        cell = sheet.Cells[x, 5, x, 6];
                        cell.Merge = true;   //退勤時間のセルを結合
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);  //罫線
                        
                        cell = sheet.Cells[x, 7, x, 8];
                        cell.Merge = true;   //休憩時間のセルを結合
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);  //罫線
                        x++;
                        y = 1;
                    }

                    // セル幅を自動調整
                    sheet.Cells.AutoFitColumns(8);

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
        public DataTable GetOutputData(AttendanceRecordViewModels model) {
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
                    + " 	CONVERT(VARCHAR,BasicDate,111) as 年月日"
                    + " 	,DATENAME(WEEKDAY, BasicDate) as 曜日"
                    + " 	,T勤怠.開始打刻 as 出勤時間"
                    + " 	,'' as 出勤時間結合用"
                    + " 	,T勤怠.終了打刻 as 退勤時間"
                    + " 	,'' as 退勤時間結合用"
                    + " 	,IIF((T勤怠.開始打刻 Is null) Or (T勤怠.開始打刻 Is null),'','1:00') as 休憩時間"
                    + " 	,'' as 休憩時間結合用"
                    + " FROM"
                    + "   DateTable as DT left join TA100勤怠実績Data T勤怠 on CONVERT(nvarchar,DT.BasicDate,112) = T勤怠.勤務年月日 and T勤怠.従業員Code = '{2}'";

                sql = string.Format(sql, startDate.ToString("yyyy/MM/dd"), endDate.ToString("yyyy/MM/dd"), model.Search.EmployeeNo);

                using (DbManager dm = new DbManager())
                using (IDbCommand cmd = dm.CreateCommand(sql))
                using (DataSet ds = new DataSet()) {
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
