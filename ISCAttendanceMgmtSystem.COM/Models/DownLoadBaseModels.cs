using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ISCAttendanceMgmtSystem.COM.Models {
    /// <summary>
    /// ダウンロードベース
    /// </summary>
    abstract public class DownLoadModel {
        public DownLoadModel() {
            //全フィールドに空文字セット リフレクション版
            Type type = this.GetType();
            PropertyInfo[] pInfos = type.GetProperties();
            object value;
            foreach(PropertyInfo pinfo in pInfos) {
                value = null;
                if(pinfo.PropertyType == typeof(string))
                    value = "";
                else if(pinfo.PropertyType == typeof(bool))
                    value = false;

                pinfo.SetValue(this, value, null);
            }
        }
        /// <summary>
        /// ダウンロードファイルパス
        /// </summary>
        public string DownloadPath { get; set; }
        /// <summary>
        /// ダウンロードフラグ
        /// </summary>
        public bool DownloadFlag { get; set; }
    }
}
