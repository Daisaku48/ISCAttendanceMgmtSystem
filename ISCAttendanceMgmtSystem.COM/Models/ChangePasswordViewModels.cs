// 2022-99-99 iwai-shibuya add str
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISCAttendanceMgmtSystem.COM.Models {
    /// <summary>
    /// パスワード変更
    /// </summary>
    public class ChangePasswordViewModelSd {
        /// <summary>
        /// 現在のパスワード
        /// </summary>
        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "現在のパスワード")]
        public string OldPassword { get; set; }

        /// <summary>
        /// 新規パスワード
        /// </summary>
        //[Required]
        //[StringLength(100, ErrorMessage = "{0} の長さは {2} 文字以上である必要があります。", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name = "新しいパスワード")]
        public string NewPassword { get; set; }

        /// <summary>
        /// 確認入力
        /// </summary>
        //[DataType(DataType.Password)]
        //[Display(Name = "新しいパスワードの確認入力")]
        //[Compare("NewPassword", ErrorMessage = "新しいパスワードと確認のパスワードが一致しません。")]
        public string ConfirmPassword { get; set; }
    }
}
// 2022-99-99 iwai-shibuya add end