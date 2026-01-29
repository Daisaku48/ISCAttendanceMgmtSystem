using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ISCAttendanceMgmtSystem.COM.Util.Database;
using ISCAttendanceMgmtSystem.COM.Entity.Session;
using ISCAttendanceMgmtSystem.COM.Models;
using ISCAttendanceMgmtSystem.COM.Util.Convert;
using ISCAttendanceMgmtSystem.COM.Util.Encrypt;
using ISCAttendanceMgmtSystem.COM.Enum;

namespace ISCAttendanceMgmtSystem.BL.Login
{

    /// <summary>
    /// ログイン用ビジネスロジック
    /// </summary>
    public class LoginBL
    {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoginBL()
        {
        }

        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Login(string uid, string password, LoginViewModelSd vm)
        {
            try
            {
                //開始
                var sql = "select"
                        + "  TMLogin.従業員Code"
                        + " ,TM従業員.氏名 "
                        + " ,TM従業員.名前 "
                        + " ,TM管理.ConstraintFlg "

                        //2023-02-01 iwai-tamura upd-str ------
                        + " ,TM管理.入力可能所属範囲_開始 "
                        + " ,TM管理.入力可能所属範囲_終了 "
                        + " ,TM管理.入力可能所属個別1 "
                        + " ,TM管理.入力可能所属個別2 "
                        + " ,TM管理.入力可能所属個別3 "
                        + " ,TM管理.入力可能所属個別4 "
                        + " ,TM管理.入力可能所属個別5 "
                        + " ,TM管理.入力可能従業員個別1 "
                        + " ,TM管理.入力可能従業員個別2 "
                        + " ,TM管理.入力可能従業員個別3 "
                        + " ,TM管理.入力可能従業員個別4 "
                        + " ,TM管理.入力可能従業員個別5 "
                        + " ,TM管理.管理職承認可能Flg "
                        + " ,TM管理.総務職承認可能Flg "
                        //2023-02-01 iwai-tamura upd-end ------

                        + "  from TMS010LoginPassword as TMLogin "
                        + "     left join TM010従業員情報Master as TM従業員 on TMLogin.従業員Code = TM従業員.従業員Code"
                        + "     left join TMS500UserConstraintMaster as TM管理 on TMLogin.従業員Code = TM管理.従業員Code"
                        + " where TMLogin.従業員Code = @EmployeeNo and TMLogin.Password = @Password";

                using (DbManager dm = new DbManager())
                using (IDbCommand cmd = dm.CreateCommand(sql))
                using (DataSet ds = new DataSet())
                {
                    DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
                    DbHelper.AddDbParameter(cmd, "@Password", DbType.String);
                    ((IDbDataParameter)cmd.Parameters[0]).Value = uid;
                    ((IDbDataParameter)cmd.Parameters[1]).Value = DataConv.IfNull(EncryptUtil.EncryptString(password));
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);

                    // データセットに設定する
                    da.Fill(ds);

                    StringBuilder sb = new StringBuilder();
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        LoginUser luser = new LoginUser
                        {
                            UserCode = KanaEx.ToHankaku(uid.ToUpper()),     //全角を半角、小文字を大文字に変換
                            UserName = row["氏名"].ToString(),
                            AdminType = row["ConstraintFlg"].ToString(),
                            //2023-02-01 iwai-tamura upd-str ------
                            ManageDepartmentStart = row["入力可能所属範囲_開始"].ToString(),
                            ManageDepartmentEnd = row["入力可能所属範囲_終了"].ToString(),
                            ManageDepartment1 = row["入力可能所属個別1"].ToString(),
                            ManageDepartment2 = row["入力可能所属個別2"].ToString(),
                            ManageDepartment3 = row["入力可能所属個別3"].ToString(),
                            ManageDepartment4 = row["入力可能所属個別4"].ToString(),
                            ManageDepartment5 = row["入力可能所属個別5"].ToString(),
                            ManageEmployee1 = row["入力可能従業員個別1"].ToString(),
                            ManageEmployee2 = row["入力可能従業員個別2"].ToString(),
                            ManageEmployee3 = row["入力可能従業員個別3"].ToString(),
                            ManageEmployee4 = row["入力可能従業員個別4"].ToString(),
                            ManageEmployee5 = row["入力可能従業員個別5"].ToString(),
                            ApprovalManagementFlg = row["管理職承認可能Flg"].ToString(),
                            ApprovalGeneralAffairsFlg = row["総務職承認可能Flg"].ToString(),
                            //2023-02-01 iwai-tamura upd-end ------

                        };
                        //ログイン状態
                        HttpContext.Current.Session["IsLogin"] = true;
                        HttpContext.Current.Session["LoginUser"] = luser;
                        // パスワード有効期限
                        HttpContext.Current.Session["IsPassExpiration"] = false;    //ok
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                // エラー
                throw;
            }
            finally
            {
                //終了
            }
        }

        /// <summary>
        /// ログイン状態か判定
        /// </summary>
        /// <returns>判定結果</returns>
        public bool IsLogin()
        {
            return HttpContext.Current.Session["LoginUser"] == null ? false : true;
        }

        // パスワード有効期限追加
        /// <summary>
        /// パスワードの有効期限が過ぎているか判定
        /// </summary>
        /// <returns>判定結果</returns>
        public bool IsPassExpiration()
        {
            return (bool)HttpContext.Current.Session["IsPassExpiration"] == true ? true : false;
        }
    }
}
