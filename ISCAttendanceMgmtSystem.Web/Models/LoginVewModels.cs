using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace SkillDiscriminantSystem.Web.Models {

    public class LoginViewModelTest {
        [Required]
        [Display(Name = "ID")]
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "パスワード")]
        public string Password { get; set; }
    }
}