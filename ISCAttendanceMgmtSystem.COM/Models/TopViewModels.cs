using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ISCAttendanceMgmtSystem.COM.Models;


namespace ISCAttendanceMgmtSystem.COM.Models {
    /// <summary>
    /// TOP用モデル
    /// </summary>
    public class TopViewModels
    {
        /// <summary>
        /// 検索条件
        /// </summary>
        public TopViewModel TopView { get; set; }
        /// <summary>
        /// 検索結果
        /// </summary>
        public List<TodayAttendanceRecordViewListModel> AttendanceRecordViewList { get; set; }
    }

    /// <summary>
    /// TOP用メインモデル
    /// </summary>
    public class TopViewModel{

        /// <summary>
        /// 標準日時 日時
        /// </summary>
        public string StandardDateTime { get; set; }

        /// <summary>
        /// 標準日時 日付
        /// </summary>
        public string StandardDate { get; set; }

        /// <summary>
        /// 標準日時 時間
        /// </summary>
        public string StandardTime { get; set; }

        /// <summary>
        /// 表示用 始業時間
        /// </summary>
        public string ViewStartTime { get; set; }

        /// <summary>
        /// 表示用 終業時間
        /// </summary>
        public string ViewEndTime { get; set; }

        //2023/10/30 iwai-tamura upd-str ------
        /// <summary>
        /// 表示用 ステータス
        /// </summary>
        public string ViewStatus { get; set; }
        //2023/10/30 iwai-tamura upd-end ------

    }

    /// <summary>
    /// 当日の出勤状況用モデル
    /// </summary>
    public class TodayAttendanceRecordViewListModel {
        /// <summary>
        /// 従業員番号
        /// </summary>
        public string EmployeeNo { get; set; }

        /// <summary>
        /// 氏名
        /// </summary>
        public string EmployeeName { get; set; }


        /// <summary>
        /// 所属Code
        /// </summary>
        public string DepartmentNo { get; set; }

        /// <summary>
        /// 所属名
        /// </summary>
        public string DepartmentName { get; set; }
        

        /// <summary>
        /// ステータス
        /// </summary>
        public string AttendanceStatus { get; set; }

        //2023/10/30 iwai-tamura upd-str ------
        /// <summary>
        /// ステータス名
        /// </summary>
        public string AttendanceStatusName { get; set; }

        /// <summary>
        /// ステータス文字色
        /// </summary>
        public string AttendanceStatusColor { get; set; }
        //2023/10/30 iwai-tamura upd-end ------

        /// <summary>
        /// 始業時間
        /// </summary>
        public string BiginTime { get; set; }

        /// <summary>
        /// 終業時間
        /// </summary>
        public string FinishTime { get; set; }

        /// <summary>
        /// 予定始業時間
        /// </summary>
        public string BiginTimePlan { get; set; }

        /// <summary>
        /// 予定終業時間
        /// </summary>
        public string FinishTimePlan { get; set; }


        //2023/02/08 iwai-tamura upd-str ------
        /// <summary>
        /// 修正時間
        /// </summary>
        public string UpdateTime { get; set; }

        /// <summary>
        /// 修正者氏名
        /// </summary>
        public string UpdateEmployeeName { get; set; }

        /// <summary>
        /// 事由
        /// </summary>
        public string UpdateComment { get; set; }
        //2023/02/08 iwai-tamura upd-end ------


    }
}
