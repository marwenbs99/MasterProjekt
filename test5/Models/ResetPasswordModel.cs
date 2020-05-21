using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace test5.Models
{
    public class ResetPasswordModel
    {
        [Display(Name = "New Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "New Password required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "please type the same password ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string ResetCode { get; set; }
    }
}