using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISCAttendanceMgmtSystem.COM.Util.File {
    /// <summary>
    /// ファイル操作用
    /// </summary>
    public static class FileUtil {

        /// <summary>
        /// コンフィグから帳票出力フォルダ取得
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string GetTempDir(Configuration config,string keys) {
            try {
                string tempDir = config.AppSettings.Settings[keys].Value;
                //末尾がpathの区切り文字かしらべ、違っていたら追加する。
                if(!(tempDir.EndsWith(Path.DirectorySeparatorChar.ToString()))) {
                    tempDir += Path.DirectorySeparatorChar;
                }
                return tempDir;
            } catch(Exception ex) {
                // エラー
                throw;
            }
        }

        /// <summary>
        /// コンフィグからファイル取得
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string GetTempFile(Configuration config,string keys) {
            try {
                string tempFile = config.AppSettings.Settings[keys].Value;
                return tempFile;
            } catch(Exception ex) {
                // エラー
                throw;
            }
        }
        
    }
}
