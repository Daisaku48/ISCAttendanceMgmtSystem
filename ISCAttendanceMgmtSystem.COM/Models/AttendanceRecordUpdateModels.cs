using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ISCAttendanceMgmtSystem.COM.Models;

namespace ISCAttendanceMgmtSystem.COM.Models {
    /// <summary>
    /// 勤務実績照会モデル
    /// </summary>
    public class AttendanceRecordUpdateModels {

        /// <summary>
        /// ヘッダー情報
        /// </summary>
        public AttendanceRecordUpdateModel Head { get; set; }

        /// <summary>
        /// 検索条件
        /// </summary>
        public AttendanceRecordUpdateSearchModel Search { get; set; }

        /// <summary>
        /// 登録内容
        /// </summary>
        public AttendanceRecordUpdateInputModel Input { get; set; }

        /// <summary>
        /// 検索結果
        /// </summary>
        public List<AttendanceRecordUpdateListModel> SearchResult { get; set; }
        /// <summary>
        /// ダウンロード設定値
        /// </summary>
        public AttendanceRecordDownLoadModel Down { get; set; }

    }

    /// <summary>
    /// ヘッダー情報
    /// </summary>
    public class AttendanceRecordUpdateModel {
        /// <summary>
        /// 表示区分
        /// </summary>
        public string ViewType { get; set; }

        /// <summary>
        /// 従業員Code：修正者
        /// </summary>
        public string EmployeeNo { get; set; }

    }



    /// <summary>
    /// 検索条件
    /// </summary>
    public class AttendanceRecordUpdateSearchModel : SearchModel {
        /// <summary>
        /// 検索日付
        /// </summary>
        public DateTime Date { get; set; }

        //2023-10-30 iwai-tamura upd-str ---
        /// <summary>
        /// 検索従業員リスト用データ：従業員Code,入社年月日,退職年月日
        /// </summary>
        public string EmployeeData { get; set; }
        //2023-10-30 iwai-tamura upd-end ---

        /// <summary>
        /// 検索従業員Code：修正対象者
        /// </summary>
        public string EmployeeNo { get; set; }

        /// <summary>
        /// 検索従業員氏名
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 検索従業員ドロップダウン用アイテム
        /// </summary>
        public IList<SelectListItem> EmployeeItemList { get; set; }
    }

    /// <summary>
    /// 登録内容
    /// </summary>
    public class AttendanceRecordUpdateInputModel{
        //2023/10/30 iwai-tamura upd-str ------
        /// <summary>
        /// 処理区分
        /// </summary>
        public string UpdateType { get; set; }
        //2023/10/30 iwai-tamura upd-end ------

        /// <summary>
        /// 出勤時間：時
        /// </summary>
        public string InputBeginHour { get; set; }

        /// <summary>
        /// 出勤時間：分
        /// </summary>
        public string InputBeginMinutes { get; set; }

        /// <summary>
        /// 退勤時間：時
        /// </summary>
        public string InputFinishHour { get; set; }

        /// <summary>
        /// 退勤時間：分
        /// </summary>
        public string InputFinishMinutes { get; set; }

        //2023/10/30 iwai-tamura upd-str ------
        /// <summary>
        /// ステータス
        /// </summary>
        public string AttendanceStatus { get; set; }
        //2023/10/30 iwai-tamura upd-end ------

        /// <summary>
        /// 備考
        /// </summary>
        public string InputComment { get; set; }
    }

    /// <summary>
    /// 検索結果
    /// </summary>
    public class AttendanceRecordUpdateListModel : SearchListModel {
        /// <summary>
        /// 採番
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 処理日時
        /// </summary>
        public string UpdateDateTimeView { get; set; }

        /// <summary>
        /// 処理者従業員番号
        /// </summary>
        public string UpdateEmployeeNo { get; set; }

        /// <summary>
        /// 処理者従業員氏名
        /// </summary>
        public string UpdateEmployeeName { get; set; }

        /// <summary>
        /// 処理区分
        /// </summary>
        public string UpdateType { get; set; }

        /// <summary>
        /// 処理区分名
        /// </summary>
        public string UpdateTypeName { get; set; }

        //2023/10/30 iwai-tamura upd-str ------
        /// <summary>
        /// 勤務状態区分名
        /// </summary>
        public string AttendanceStatusName { get; set; }
        //2023/10/30 iwai-tamura upd-end ------

        /// <summary>
        /// 処理備考
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 開始打刻
        /// </summary>
        public string StampTimeBegin { get; set; }

        /// <summary>
        /// 終了打刻
        /// </summary>
        public string StampTimeFinish { get; set; }
    }
}
