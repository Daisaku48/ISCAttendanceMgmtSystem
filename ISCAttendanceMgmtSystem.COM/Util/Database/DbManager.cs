using System;
using System.Data.SqlClient;
//using MySql.Data.MySqlClient;

using System.Configuration;
using System.Data;
using ISCAttendanceMgmtSystem.COM.Util.Config;
using ISCAttendanceMgmtSystem.COM.Util.Convert;

namespace ISCAttendanceMgmtSystem.COM.Util.Database {
    /// <summary>
    /// ============================================================================== 
    /// Iwai Project                                  CopyRight(C)　SBC 
    /// ------------------------------------------------------------------------------ 
    /// サイト　　:　職能システム 
    /// 機能名　　:　DB管理クラス
    /// ファイル名:　DbManager.cs
    /// 処理区分　:　共通ユーティリティ
    /// 作成日　　:　2015/02/21 
    /// 作成者　　:　SBC Y.Katoh
    /// バージョン:　1.0 
    /// 機能概要　:  ＤＢ接続 および トランザクションを管理する。
    ///              プリプロセッサ
    ///              MYSQL・・・MySql接続
    ///              以外 ・・・SQL Server接続
    ///  
    /// ============================================================================== 
    /// </summary>
    public class DbManager : IDisposable {
        #region "メンバ変数"
        /// <summary>
        /// SQLコネクション
        /// </summary>
        private IDbConnection _dBConnect = null;

        #endregion

        #region "コンストラクタ・デストラクタ"
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionKey">接続文字列取得キー</param>
        public DbManager() {
            this.ConnectDB();
        }

        /// <summary>
        /// 処置メソッド
        /// </summary>
        public void Dispose() {
            if (_dBConnect == null) return;
            if (_dBConnect.State != System.Data.ConnectionState.Closed) {
                try {
                    _dBConnect.Close();
                    _dBConnect.Dispose();
                } catch (Exception) {
                    //throw;
                }
            }
            _dBConnect = null;
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~DbManager() {
            this.Dispose();
        }
        #endregion

        #region "公開プロパティ"
        /// <summary>
        /// DBコネクションプロパティ
        /// </summary>
        public IDbConnection DBConnect {
            get { return _dBConnect; }
        }
        #endregion

        #region "プライベートメソッド"
        /// <summary>
        /// DB接続
        /// </summary>
        private void ConnectDB() {
            // SQLServer接続
            Configuration config = WebConfig.GetConfigFile();
            string dbType = config.AppSettings.Settings["DB_SERVER_TYPE"].Value;

            //if ("MySQL".Equals(dbType)) {
            //    _dBConnect = new MySqlConnection(config.ConnectionStrings.ConnectionStrings[dbType].ConnectionString);
            //} else {
            //    _dBConnect = new SqlConnection(config.ConnectionStrings.ConnectionStrings[dbType].ConnectionString);
            //}
            _dBConnect = new SqlConnection(config.ConnectionStrings.ConnectionStrings[dbType].ConnectionString);

            _dBConnect.Open();
        }

        /// <summary>
        /// SQLコマンド生成
        /// </summary>
        /// <param name="sql">SQL文</param>
        /// <returns>SQLコマンドインターフェース</returns>
        public IDbCommand CreateCommand(string sql) {
            //IDbCommand command = null;
            ////if (_dBConnect is MySqlConnection) {
            ////    command = new MySqlCommand(sql, (MySqlConnection)_dBConnect);
            ////} else {
            ////    command = new SqlCommand(sql, (SqlConnection)_dBConnect);
            ////}
            //command = new SqlCommand(sql, (SqlConnection)_dBConnect);
            //return command;
            return CreateCommand(sql, "");
        }

        /// <summary>
        /// SQLコマンド生成
        /// </summary>
        /// <param name="sql">SQL文</param>
        /// <param name="timeout">コマンドタイムアウト</param>
        /// <returns>SQLコマンドインターフェース</returns>
        public IDbCommand CreateCommand(string sql,string timeout) {
            IDbCommand command = null;
            command = new SqlCommand(sql, (SqlConnection)_dBConnect);
            if (!string.IsNullOrEmpty(timeout)) command.CommandTimeout = (int)DataConv.IntParse(timeout,30);
            return command;
        }

        /// <summary>
        /// SQLデータアダプター生成
        /// </summary>
        /// <param name="command">SQL文</param>
        /// <returns>データアダプターインターフェース</returns>
        public IDataAdapter CreateSqlDataAdapter(IDbCommand command) {
            IDataAdapter dataAdapter = null;
            //if (_dBConnect is MySqlConnection) {
            //    dataAdapter = new MySqlDataAdapter((MySqlCommand)command);
            //} else {
            //    dataAdapter = new SqlDataAdapter((SqlCommand)command);
            //}
            dataAdapter = new SqlDataAdapter((SqlCommand)command);
            return dataAdapter;
        }
        #endregion
    }
}
