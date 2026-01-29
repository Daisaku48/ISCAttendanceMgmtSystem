using System;
using System.Collections;
using System.Security.Cryptography;
using System.Data;
using ISCAttendanceMgmtSystem.COM.Enum;
using System.Globalization;

namespace ISCAttendanceMgmtSystem.COM.Util.Convert {
    /// <summary>
    /// ============================================================================== 
    /// Iwai Project                                  CopyRight(C)　SBC 
    /// ------------------------------------------------------------------------------ 
    /// サイト　　:　職能システム 
    /// 機能名　　:　データ変換
    /// ファイル名:　DataConv.cs
    /// 処理区分　:　共通ユーティリティ
    /// 作成日　　:　2015/02/26 
    /// 作成者　　:　SBC Katoh
    /// バージョン:　1.0 
    /// 機能概要　:  データ変換を行う
    ///  
    /// ============================================================================== 
    /// </summary>
    public static class DataConv {
        /// <summary>
        /// Int変換
        /// </summary>
        /// <param name="value">変換したいデータ</param>
        /// <returns>数値</returns>
        /// <remarks>
        /// 文字列を数値変換します、変換失敗時はNULL
        /// </remarks>
        public static int? IntParse(string value) {
            return IntParse(value, null);
        }

        /// <summary>
        /// Int変換
        /// </summary>
        /// <param name="value">変換したいデータ</param>
        /// <param name="nullConv">ヌル時数値</param>
        /// <returns>数値</returns>
        /// <remarks>
        /// 文字列を数値変換します、変換失敗時およびNULL時はヌル時数値を返す
        /// </remarks>
        public static int? IntParse(string value, int? nullConv) {
            int result;
            int? rtValue = (nullConv == null) ? null : nullConv;
            if(int.TryParse(value, out result)) rtValue = result;
            return rtValue;
        }

        /// <summary>
        /// long変換
        /// </summary>
        /// <param name="value">変換したいデータ</param>
        /// <returns>数値</returns>
        /// <remarks>
        /// 文字列を数値変換します、変換失敗時はNULL
        /// </remarks>
        public static long? LongParse(string value) {
            return LongParse(value, null);
        }

        /// <summary>
        /// long変換
        /// </summary>
        /// <param name="value">変換したいデータ</param>
        /// <param name="nullConv">ヌル時数値</param>
        /// <returns>数値</returns>
        /// <remarks>
        /// 文字列を数値変換します、変換失敗時およびNULL時はヌル時数値を返す
        /// </remarks>
        public static long? LongParse(string value, long? nullConv) {
            long result;
            long? rtValue = (nullConv == null) ? null : nullConv;
            if(long.TryParse(value, out result)) rtValue = result;
            return rtValue;
        }

        /// <summary>
        /// DateTime変換
        /// </summary>
        /// <param name="value">変換したいデータ</param>
        /// <returns>日時</returns>
        /// <remarks>
        /// 文字列を日時変換します、変換失敗時はNULL
        /// </remarks>
        public static DateTime? DateTimeParse(string value) {
            return DateTimeParse(value, null);
        }

        /// <summary>
        /// DateTime変換
        /// </summary>
        /// <param name="value">変換したいデータ</param>
        /// <param name="nullConv">ヌル時値</param>
        /// <returns>日時</returns>
        /// <remarks>
        /// 文字列を日時変換します、変換失敗時およびNULL時はヌル時値を返す
        /// </remarks>
        public static DateTime? DateTimeParse(string value, DateTime? nullConv) {
            DateTime result;
            DateTime? rtValue = (nullConv == null) ? null : nullConv;
            if(DateTime.TryParse(value, out result)) rtValue = result;
            return rtValue;
        }

        /// <summary>
        /// 年月日分解
        /// </summary>
        /// <param name="value">日付の文字列</param>
        /// <param name="dEnum">日付用Enum</param>
        /// <returns>分解した結果</returns>
        public static string Date2String(string value, DateEnum dEnum) {
            DateTime? dt = DateTimeParse(value, null);
            if(dt == null) return "";

            var retval = "";
            switch(dEnum) {
                case DateEnum.YEAR:
                    retval = dt.Value.Year.ToString();
                    break;
                case DateEnum.MONTH:
                    retval = dt.Value.Month.ToString();
                    break;
                case DateEnum.DAY:
                    retval = dt.Value.Day.ToString();
                    break;
                case DateEnum.YMD:
                    retval = string.Format("{0}/{1}/{2}", dt.Value.Year.ToString(), dt.Value.Month.ToString(), dt.Value.Day.ToString());
                    break;
                case DateEnum.ALL:
                default:
                    retval = dt.ToString();
                    break;
            }
            return retval;
        }

        /// <summary>
        /// 和暦変換
        /// </summary>
        /// <param name="dt">日付</param>
        /// <param name="format">フォーマット</param>
        /// <returns>和暦</returns>
        public static string Date2Jcal(DateTime dt, string format) {
            CultureInfo culture = new CultureInfo("ja-JP", true);
            culture.DateTimeFormat.Calendar = new JapaneseCalendar();
            return dt.ToString(format, culture);
        }


        /// <summary>
        /// 改行を削除
        /// </summary>
        /// <param name="val">対象文字</param>
        /// <returns>変換後</returns>
        public static String EraseCrLf(string val) {
            //Nullで空文字
            return DataConv.EraseCrLf(val, null);
        }

        /// <summary>
        /// 改行を削除して置換
        /// </summary>
        /// <param name="val">対象文字</param>
        /// <param name="repVal">変換用文字</param>
        /// <returns>変換後</returns>
        public static String EraseCrLf(string val, string repVal) {
            return val.Replace("\r", "").Replace("\n", repVal == null ? "" : repVal);
        }

        /// <summary>
        /// タブクリア
        /// </summary>
        /// <param name="val">対象文字</param>
        /// <returns>変換後</returns>
        public static String EraseTab(string val) {
            //Nullで空文字
            return DataConv.EraseTab(val, null);
        }

        /// <summary>
        /// タブクリアして置換
        /// </summary>
        /// <param name="val">対象文字</param>
        /// <param name="repVal">変換用文字</param>
        /// <returns>変換後</returns>
        public static String EraseTab(string val, string repVal) {
            return val.Replace("\t", repVal == null ? "" : repVal);
        }

        /// <summary>
        /// 登録エンティティ取り出し
        /// </summary>
        /// <param name="entityList">登録</param>
        /// <returns>登録エンティティ</returns>
        public static T GetEntity<T>(IList eList) {
            //取り出し
            foreach(T iEntry in eList) { return iEntry; }
            //空を返す
            return default(T);
        }

        /// <summary>
        /// 日付に変換できるか判定し、変換できない場合は日付の最大値を返す
        /// </summary>
        /// <param name="val">変換元</param>
        /// <returns>結果</returns>
        public static string DateTimeTostring(string val) {
            DateTime dt;
            return !DateTime.TryParse(val, out dt) ? DateTime.MaxValue.ToString() : val;
        }

        /// <summary>
        /// 日付に変換できるか判定し、変換出来た場合は整形してかえすわ。
        /// </summary>
        /// <param name="val">変換元</param>
        /// <returns>結果</returns>
        public static string DateTimeTostring(DateTime? val, string format) {
            return string.IsNullOrEmpty(val.ToString()) ? "" : ((DateTime)val).ToString(format);
        }

        /// <summary>
        /// DateTime型変換
        /// </summary>
        /// <param name="value">変換元</param>
        /// <remarks>値が空白の場合エラーで落ちるので追加</remarks>
        /// <returns>結果</returns>
        public static string DateTimeToLongTimeString(string value) {
            DateTime dt;
            //空の場合
            if(value.Trim() == "") { return ""; }
            //変換できない場合
            if(!DateTime.TryParse(value, out dt)) { return ""; }
            //変換できた                
            return dt.ToString("HH:mm:ss");
        }

        /// <summary>
        /// DateTime型変換
        /// </summary>
        /// <param name="value">変換元</param>
        /// <remarks>値が空白の場合エラーで落ちるので追加</remarks>
        /// <returns>結果</returns>
        public static string DateTimeToStringEx(string value) {
            string fmt = "{0} {1}";

            if(value == null) return "";

            if(value == "") return value;

            //日付
            string[] date = value.Split(' ');
            string datetimemax = DateTime.MaxValue.ToString();
            DateTime? dval = DataConv.DateTimeParse(date[0]);
            date[0] = dval != null ? ((DateTime)dval).ToString("yyyy/MM/dd") : DateTime.MaxValue.ToString("yyyy/MM/dd");
            dval = DataConv.DateTimeParse(string.Format(fmt, DateTime.MaxValue.ToString("yyyy/MM/dd"), date[1]));
            date[1] = dval != null ? ((DateTime)dval).ToString("HH:mm:ss") : DateTime.MaxValue.ToString("HH:mm:ss");
            return string.Format(fmt, date[0], date[1]);
        }

        /// <summary>
        /// 暗号化
        /// </summary>
        /// <param name="value">暗号化したい文字列</param>
        /// <returns>暗号化した文字</returns>
        public static string CreateSha256Crypt(string value) {
            try {
                SHA256 shaM = new SHA256Managed();
                byte[] data = System.Text.Encoding.UTF8.GetBytes(value.Trim());
                byte[] bs = shaM.ComputeHash(data);
                //byte型配列を16進数の文字列に変換
                System.Text.StringBuilder result = new System.Text.StringBuilder();
                foreach(byte b in bs) {
                    result.Append(b.ToString("x2"));
                }
                return result.ToString().Trim();
            } catch(Exception) {   //エラー時はNULLを返します。
                return "";
            }
        }

        /// <summary>
        /// URLコンバイン
        /// </summary>
        /// <param name="baseUrl">ベース</param>
        /// <param name="relativeUrl">くっつけるの</param>
        /// <returns>くっつけた結果なの</returns>
        public static string CombineUrl(string baseUrl, string relativeUrl) {
            //ベースが空？
            if(string.IsNullOrEmpty(baseUrl))
                return relativeUrl;
            //ベースが一文字かつ「/」のみ
            if(baseUrl.Trim().Length == 1 && baseUrl.Trim().Equals("/"))
                return relativeUrl;

            return baseUrl + relativeUrl;
        }

        /// <summary>
        /// expr1がnullでもEmpty文字列でもない場合はexpr1を返し、それ以外の場合はDBNull.Valueを返します。
        /// </summary>
        /// <param name="expr1">値１</param>
        /// <returns>expr1がnullでもEmpty文字列でもない場合はexpr1、それ以外はDBNull.Value。</returns>
        public static object IfNull(string expr1) {
            return IfNull(expr1, DBNull.Value);
        }

        /// <summary>
        /// expr1がnullでもEmpty文字列でもない場合はexpr1を返し、それ以外の場合はexpr2を返します。
        /// </summary>
        /// <param name="expr1">値１</param>
        /// <param name="expr2">値２</param>
        /// <returns>expr1がnullでもEmpty文字列でもない場合はexpr1、それ以外はexpr2。</returns>
        public static object IfNull(string expr1, object expr2) {
            object result = expr1;
            if(string.IsNullOrEmpty(expr1)) {
                result = expr2;
            }
            return result;
        }

        /// <summary>
        /// decimal変換
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nullConv"></param>
        /// <returns></returns>
        public static decimal? Str2Deci(string value, decimal? nullConv) {
            decimal result;
            decimal? rtValue = (nullConv == null) ? null : nullConv;
            if(decimal.TryParse(value, out result)) rtValue = result;
            return rtValue;
        }
        /// <summary>
        /// decimal変換
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal? Str2Deci(string value) {
            return Str2Deci(value, null);
        }

        /// <summary>
        /// int変換
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nullConv"></param>
        /// <returns></returns>
        public static int? Str2Int(string value, int? nullConv) {
            int result;
            int? rtValue = (nullConv == null) ? null : nullConv;
            if(int.TryParse(value, out result)) rtValue = result;
            return rtValue;
        }
        /// <summary>
        /// int変換
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? Str2Int(string value) {
            return Str2Int(value, null);
        }
    }
}
