using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISCAttendanceMgmtSystem.BL.Common {
    /// <summary>
    /// 全体共通ビジネスロジック
    /// </summary>
    public class CommonBL {
        /// <summary>
        /// パターンマッチング用の文字列作成
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CreateLikePattern(string value) {
            StringBuilder sb = new StringBuilder(value);
            //文字列後方から0のみ書き換え、0以外で書き換え中止
            for(var i = sb.Length - 1; i > 0; i--) {
                if(sb[i] != '0') { break; }
                sb[i] = '_';
            }
            return sb.ToString();
        }
    }
}
