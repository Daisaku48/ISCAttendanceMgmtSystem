// 2022-99-99 iwai-miki add str
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace ISCAttendanceMgmtSystem.COM.Models
{

    /// <summary>
    /// ログイン用
    /// </summary>
    public class LoginViewModelSd
    {
        /// <summary>
        /// ログインID
        /// </summary>
        [Required(ErrorMessage = "{0}は必須です。")]
        [Display(Name = "ID")]
        public string Id { get; set; }

        /// <summary>
        /// パスワード
        /// </summary>
        [Required(ErrorMessage = "{0}は必須です。")]
        [DataType(DataType.Password)]
        [Display(Name = "パスワード")]
        public string Password { get; set; }
    }
}
// 2022-99-99 iwai-miki add end