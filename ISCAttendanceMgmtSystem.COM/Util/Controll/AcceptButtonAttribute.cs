using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ISCAttendanceMgmtSystem.COM.Util.Controll {
    /// <summary>
    /// ボタン振り分け用属性
    /// </summary>
    public class AcceptButtonAttribute : ActionMethodSelectorAttribute {
        /// <summary>
        /// ボタン名
        /// </summary>
        public string ButtonName { get; set; }

        /// <summary>
        /// リクエスト有効判定
        /// </summary>
        /// <param name="controllerContext">コンテキスト</param>
        /// <param name="methodInfo">メソッド属性</param>
        /// <returns>ボタン名</returns>
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo) {
            var req = controllerContext.RequestContext.HttpContext.Request;
            return !string.IsNullOrEmpty(req.Form[this.ButtonName]);
        }
    }
}
