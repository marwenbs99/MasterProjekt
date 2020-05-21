using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace test5.Models
{
    public class UserLogin
    {
        [Display(Name ="Email")]
        [Required (AllowEmptyStrings =false, ErrorMessage ="Email required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        public string password { get; set; }
        [Display(Name = "Remember me")]
        public bool rememberMe { get; set; }

        
    }
}