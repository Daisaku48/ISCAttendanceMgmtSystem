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
    public class AttendanceRecordManagerViewModels {
        /// <summary>
        /// 検索条件
        /// </summary>
        public AttendanceRecordManagerViewModel Search { get; set; }
        /// <summary>
        /// 検索結果
        /// </summary>
        public List<AttendanceRecordManagerViewListModel> SearchResult { get; set; }
        /// <summary>
        /// ダウンロード設定値
        /// </summary>
        public AttendanceRecordDownLoadModel Down { get; set; }

    }
    /// <summary>
    /// 検索条件
    /// </summary>
    public class AttendanceRecordManagerViewModel : SearchModel {
        /// <summary>
        /// 従業員Code
        /// </summary>
        public string EmployeeNo { get; set; }

        //2023-10-30 iwai-tamura upd-str ---
        /// <summary>
        /// 検索従業員リスト用データ：従業員Code,入社年月日,退職年月日
        /// </summary>
        public string EmployeeData { get; set; }
        //2023-10-30 iwai-tamura upd-end ---

        /// <summary>
        /// 年
        /// </summary>
        public string EntryYear { get; set; }

        /// <summary>
        /// 月
        /// </summary>
        public string EntryMonth { get; set; }

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
    /// 検索結果
    /// </summary>
    public class AttendanceRecordManagerViewListModel : SearchListModel {

        /// <summary>
        /// 従業員Code
        /// </summary>
        public string EmployeeNo { get; set; }

        /// <summary>
        /// 氏名
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 所属Code
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 所属名
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 勤務年月日
        /// </summary>
        public string AttendanceDate { get; set; }

        /// <summary>
        /// 表示用日付
        /// </summary>
        public string ViewDay { get; set; }

        /// <summary>
        /// 表示用曜日
        /// </summary>
        public string ViewWeek { get; set; }

        /// <summary>
        /// 勤務形態
        /// </summary>
        public string AttendanceType { get; set; }

        /// <summary>
        /// 開始打刻
        /// </summary>
        public string StampTimeBegin { get; set; }

        /// <summary>
        /// 終了打刻
        /// </summary>
        public string StampTimeFinish { get; set; }

        /// <summary>
        /// 休憩開始打刻
        /// </summary>
        public string RestStampTimeBegin { get; set; }

        /// <summary>
        /// 休憩終了打刻
        /// </summary>
        public string RestStampTimeFinish { get; set; }

        /// <summary>
        /// 合計勤務時間
        /// </summary>
        public string WorkingTimeTotal { get; set; }

        /// <summary>
        /// 時間外勤務時間_1
        /// </summary>
        public string WorkingOffTime1 { get; set; }

        /// <summary>
        /// 時間外勤務時間_2
        /// </summary>
        public string WorkingOffTime2 { get; set; }

    }
    /// <summary>
    /// ダウンロード設定値
    /// </summary>
    public class AttendanceRecordManagerDownLoadModel : DownLoadModel {
    }
}
