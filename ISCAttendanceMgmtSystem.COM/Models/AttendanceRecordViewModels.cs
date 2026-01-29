using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ISCAttendanceMgmtSystem.COM.Models;

namespace ISCAttendanceMgmtSystem.COM.Models {
    /// <summary>
    /// 勤務実績照会モデル
    /// </summary>
    public class AttendanceRecordViewModels {
        /// <summary>
        /// 検索条件
        /// </summary>
        public AttendanceRecordViewModel Search { get; set; }
        /// <summary>
        /// 検索結果
        /// </summary>
        public List<AttendanceRecordViewListModel> SearchResult { get; set; }
        /// <summary>
        /// ダウンロード設定値
        /// </summary>
        public AttendanceRecordDownLoadModel Down { get; set; }

    }
    /// <summary>
    /// 検索条件
    /// </summary>
    public class AttendanceRecordViewModel : SearchModel {
        /// <summary>
        /// 従業員Code
        /// </summary>
        public string EmployeeNo { get; set; }

        /// <summary>
        /// 年
        /// </summary>
        public string EntryYear { get; set; }

        /// <summary>
        /// 月
        /// </summary>
        public string EntryMonth { get; set; }

    }
    /// <summary>
    /// 検索結果
    /// </summary>
    public class AttendanceRecordViewListModel : SearchListModel {

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




        /// <summary>
        /// 自己申告書パターン
        /// </summary>
        public string SelfDecType { get; set; }

        /// <summary>
        /// 自己申告書Dパターン
        /// </summary>
        public string SelfDecDType { get; set; }

        /// <summary>
        /// 自己申告書パターン
        /// </summary>
        public string CarrierSheetType { get; set; }

        /// <summary>
        /// 職掌番号
        /// </summary>
        public string DutyNo { get; set; }
        /// <summary>
        /// 職掌名
        /// </summary>
        public string Duty { get; set; }

        /// <summary>
        /// 資格番号
        /// </summary>
        public string CompetencyNo { get; set; }
        /// <summary>
        /// 資格名
        /// </summary>
        public string Competency { get; set; }

        /// <summary>
        /// 自己申告書ボタン表示
        /// </summary>
        public string SelfDecAtoCButtonView { get; set; }
        /// <summary>
        /// 自己申告書Dボタン表示
        /// </summary>
        public string SelfDecDButtonView { get; set; }
        /// <summary>
        /// キャリアシートボタン表示
        /// </summary>
        public string CareerButtonView { get; set; }


        
        
        /// <summary>
        /// 目標設定承認
        /// </summary>
        public string ObjectivesAceept { get; set; }
        /// <summary>
        /// 達成度承認
        /// </summary>
        public string AattainmentAccept { get; set; }
        /// <summary>
        /// 達成度計
        /// </summary>
        public string AchvTotal { get; set; }
        /// <summary>
        /// プロセス計
        /// </summary>
        public string ProcessTotal { get; set; }
    }
    /// <summary>
    /// ダウンロード設定値
    /// </summary>
    public class AttendanceRecordDownLoadModel : DownLoadModel {
    }
}
