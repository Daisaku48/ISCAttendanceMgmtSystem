using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISCAttendanceMgmtSystem.COM.Util.Database {
    public class DbHelper {
        /// <summary>
        /// 指定されたIDbCommandにパラメータを追加します。
        /// </summary>
        /// <param name="command">パラメータを追加する対象のコマンド</param>
        /// <param name="parameterName">パラメータ名</param>
        /// <param name="dbType">パラメータの型</param>
        public static void AddDbParameter(IDbCommand command, string parameterName, DbType dbType) {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.DbType = dbType;
            command.Parameters.Add(param);
        }
    }
}
