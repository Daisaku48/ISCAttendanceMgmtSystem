using System;
using System.Reflection;
using ISCAttendanceMgmtSystem.COM.Enum;

namespace ISCAttendanceMgmtSystem.COM.Entity.Session {

    /// <summary>
    /// セッション格納用のユーザー情報
    /// </summary>
    public class LoginUser {
        /// <summary>
        /// 社員コード
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 氏名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 管理区分
        /// </summary>
        public string AdminType { get; set; }

        ///
        /// <summary>
        /// 所属番号
        /// </summary>
        public string DepartmentNo { get; set; }
        /// <summary>
        /// 所属名称
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 役職番号
        /// </summary>
        public string PostNo { get; set; }
        /// <summary>
        /// 管理アクセス権有無
        /// </summary>
        public bool IsRoot { get; set; }
        /// <summary>
        /// システム管理者判断
        /// </summary>
        public bool IsRootUser { get; set; }

        /// <summary>
        /// 管理職判断
        /// </summary>
        public bool IsPost { get; set; }

        //2023-02-01 iwai-tamura upd-str ------
        ///
        /// <summary>
        /// 入力可能所属範囲_開始
        /// </summary>
        public string ManageDepartmentStart { get; set; }

        ///
        /// <summary>
        /// 入力可能所属範囲_終了
        /// </summary>
        public string ManageDepartmentEnd { get; set; }

        ///
        /// <summary>
        /// 入力可能所属_1
        /// </summary>
        public string ManageDepartment1 { get; set; }

        ///
        /// <summary>
        /// 入力可能所属_2
        /// </summary>
        public string ManageDepartment2 { get; set; }

        ///
        /// <summary>
        /// 入力可能所属_3
        /// </summary>
        public string ManageDepartment3 { get; set; }

        ///
        /// <summary>
        /// 入力可能所属_4
        /// </summary>
        public string ManageDepartment4 { get; set; }

        ///
        /// <summary>
        /// 入力可能所属_5
        /// </summary>
        public string ManageDepartment5 { get; set; }

        ///
        /// <summary>
        /// 入力可能従業員_1
        /// </summary>
        public string ManageEmployee1 { get; set; }

        ///
        /// <summary>
        /// 入力可能従業員_2
        /// </summary>
        public string ManageEmployee2 { get; set; }

        ///
        /// <summary>
        /// 入力可能従業員_3
        /// </summary>
        public string ManageEmployee3 { get; set; }

        ///
        /// <summary>
        /// 入力可能従業員_4
        /// </summary>
        public string ManageEmployee4 { get; set; }

        ///
        /// <summary>
        /// 入力可能従業員_5
        /// </summary>
        public string ManageEmployee5 { get; set; }

        ///
        /// <summary>
        /// 管理職承認可能Flg
        /// </summary>
        public string ApprovalManagementFlg { get; set; }

        ///
        /// <summary>
        /// 総務職承認可能Flg
        /// </summary>
        public string ApprovalGeneralAffairsFlg { get; set; }
        //2023-02-01 iwai-tamura upd-end ------



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoginUser() {
            //全フィールドに空文字セット リフレクション版
            Type type = this.GetType();
            PropertyInfo[] pInfos = type.GetProperties();
            object value;
            foreach (PropertyInfo pinfo in pInfos) {
                value = null;
                if (pinfo.PropertyType == typeof(string))
                    value = "";
                else if (pinfo.PropertyType == typeof(bool))
                    value = false;

                pinfo.SetValue(this, value, null);
            }
        }
    }
}
