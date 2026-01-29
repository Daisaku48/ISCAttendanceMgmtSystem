using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISCAttendanceMgmtSystem.COM.Enum {
    /// <summary>
    /// 日付分解用
    /// </summary>
    public enum DateEnum {
        /// <summary>
        /// そのまま
        /// </summary>
        ALL = 0,
        /// <summary>
        /// 年のみ
        /// </summary>
        YEAR = 1,
        /// <summary>
        /// 月のみ
        /// </summary>
        MONTH = 2,
        /// <summary>
        /// 日のみ
        /// </summary>
        DAY = 3,
        /// <summary>
        /// 年月日
        /// </summary>
        YMD = 4
    }

    /// <summary>
    /// 打刻区分 Enum
    /// </summary>
    public enum WorkStamp {
        /// <summary>
        /// 無
        /// </summary>
        None = 0,
        /// <summary>
        /// 出勤
        /// </summary>
        Begin = 1,
        /// <summary>
        /// 退勤
        /// </summary>
        Finish = 2,

        //2023-10-30 iwai-tamura upd-str ------
        /// <summary>
        /// 勤務状況変更
        /// </summary>
        Status = 5,
        /// <summary>
        /// 勤務状況変更 出勤処理時
        /// </summary>
        StatusBigin = 6
        //2023-10-30 iwai-tamura upd-end ------
    }


    //2023-10-30 iwai-tamura upd-str ------
    /// <summary>
    /// 勤務状況区分 Enum
    /// 実際は文字列2桁
    /// </summary>
    public enum AttendanceStatus
    {
        /// <summary>
        /// 無
        /// </summary>
        None = 0,
        /// <summary>
        /// 出社
        /// </summary>
        Attend = 01,
        /// <summary>
        /// テレワーク
        /// </summary>
        telework = 02,
        /// <summary>
        /// 外出
        /// </summary>
        outing = 11,

        /// <summary>
        /// 退勤
        /// </summary>
        Workout = 81,
        /// <summary>
        /// 休み
        /// </summary>
        Off = 91
    }
    //2023-10-30 iwai-tamura upd-end ------



    /// <summary>
    /// 異動 Enum
    /// </summary>
    public enum Move {
        /// <summary>
        /// 無
        /// </summary>
        None = 0,
        /// <summary>
        /// 転入
        /// </summary>
        In = 1,
        /// <summary>
        /// 転出
        /// </summary>
        Out = 2
    }
    /// <summary>
    /// 区分 Enum
    /// </summary>
    public enum Area {
        /// <summary>
        /// 無
        /// </summary>
        None = 0,
        /// <summary>
        /// 経営
        /// </summary>
        Executive = 1,
        /// <summary>
        /// 業務
        /// </summary>
        Work = 2
    }
    /// <summary>
    /// 部下の有無
    /// </summary>
    public enum Subordinate {
        /// <summary>
        /// 無
        /// </summary>
        None = 0,
        /// <summary>
        /// 有
        /// </summary>
        Any = 1
    }

    /// <summary>
    /// 健康状態
    /// </summary>
    public enum Health {
        /// <summary>
        /// 頑丈
        /// </summary>
        Strong = 0,
        /// <summary>
        /// 普通
        /// </summary>
        Nomal = 1,
        /// <summary>
        /// 少し虚弱
        /// </summary>
        Weak = 2,
        /// <summary>
        /// 虚弱
        /// </summary>
        Frail = 3
    }

    /// <summary>
    /// 管理メッセージID
    /// </summary>
    public enum ManageMessageId {
        /// <summary>
        /// パスワード変更成功
        /// </summary>
        ChangePasswordSuccess,
        /// <summary>
        /// エラー
        /// </summary>
        Error
    }

}
