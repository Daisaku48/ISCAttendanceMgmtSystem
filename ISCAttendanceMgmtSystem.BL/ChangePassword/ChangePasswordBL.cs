using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ISCAttendanceMgmtSystem.COM.Entity.Session;
using ISCAttendanceMgmtSystem.COM.Models;
using ISCAttendanceMgmtSystem.COM.Util.Convert;
using ISCAttendanceMgmtSystem.COM.Util.Database;
using ISCAttendanceMgmtSystem.COM.Util.Encrypt;

namespace ISCAttendanceMgmtSystem.BL.ChangePassword {
    /// <summary>
    /// パスワード変更ビジネスロジック
    /// </summary>
    public class ChangePasswordBL {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// パスワード変更
        /// </summary>
        /// <param name="model">変更用モデル</param>
        /// <param name="lu">ログインユーザー</param>
        /// <returns>結果</returns>
        public bool ChangePassword(ChangePasswordViewModelSd model, LoginUser lu) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                using (var scope = new TransactionScope()) {
                    using(DbManager dm = new DbManager()) {
                        //詳細
                        var sql = "update TMS010LoginPassword"
                            + " set "
                            + " Password = @NewPassword"
                            + " ,PS更新年月日 = @NowDate"
                            + " ,最終更新者Code = @ChangerId"
                            + " ,更新年月日 = @UpdateDate"
                        + " ,更新回数 = 更新回数 + 1"
                        + " where 従業員Code = @EmployeeNo"
                        + "   and Password = @OldPassword";
                        //SQL文の型を指定
                        var cmd = dm.CreateCommand(sql);
                        DbHelper.AddDbParameter(cmd, "@NewPassword", DbType.String);//新Password
                        DbHelper.AddDbParameter(cmd, "@NowDate", DbType.String);//更新年月日(8桁)
                        DbHelper.AddDbParameter(cmd, "@ChangerId", DbType.String);//最終更新者
                        DbHelper.AddDbParameter(cmd, "@UpdateDate", DbType.DateTime);//更新年月日(Date型)
                        DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);//従業員番号
                        DbHelper.AddDbParameter(cmd, "@OldPassword", DbType.String);//旧Password
                        //パラメータ設定
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = DataConv.IfNull(EncryptUtil.EncryptString(model.NewPassword));
                        parameters[1].Value = DateTime.Today.ToString("yyyyMMdd");
                        parameters[2].Value = DataConv.IfNull(lu.UserCode);
                        parameters[3].Value = DateTime.Now;
                        parameters[4].Value = DataConv.IfNull(lu.UserCode);
                        parameters[5].Value = DataConv.IfNull(EncryptUtil.EncryptString(model.OldPassword));

                        if(cmd.ExecuteNonQuery() <= 0) {
                            throw new Exception("パスワード更新エラー");
                        }
                    }
                    scope.Complete();
                }
                return true;
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return false;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
    }
}
