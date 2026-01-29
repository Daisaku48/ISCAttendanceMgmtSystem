using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISCAttendanceMgmtSystem.COM.Models {
    /// <summary>
    /// 検索項目ベースクラス
    /// </summary>
    abstract public class SearchModel {
        /// <summary>
        /// 年度
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// 所属FROM
        /// </summary>
        public string DepartmentFrom { get; set; }
        /// <summary>
        /// 所属To
        /// </summary>
        public string DepartmentTo { get; set; }
        /// <summary>
        /// 社員番号FROM
        /// </summary>
        public string EmployeeNoFrom { get; set; }
        /// <summary>
        /// 社員番号To
        /// </summary>
        public string EmployeeNoTo { get; set; }
        /// <summary>
        /// 社員番号個別指定フラグ
        /// </summary>
        public bool DesignatedFlag { get; set; }
        /// <summary>
        /// 氏名
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 氏名カナ
        /// </summary>
        [RegularExpression(@"[ァ-ヶ]+", ErrorMessage = "全角カタカナのみ入力できます。")]
        public string EmployeeNameKana { get; set; }
    }

    /// <summary>
    /// 検索結果ベースクラス
    /// </summary>
    abstract public class SearchListModel {
        /// <summary>
        /// 選択状態
        /// </summary>
        public bool Selected { get; set; }
        /// <summary>
        /// 管理番号
        /// </summary>
        public string ManageNo { get; set; }
        /// <summary>
        /// 年度
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// 社員番号
        /// </summary>
        public string EmployeeNumber { get; set; }
        /// <summary>
        /// 氏名
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 氏名カナ
        /// </summary>
        public string EmployeeNameKana { get; set; }
        /// <summary>
        /// 所属
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 表示リンク
        /// </summary>
        public string ViewLink { get; set; }
        /// <summary>
        /// テーブルの種類
        /// </summary>
        public string TableType { get; set; }
    }
}
