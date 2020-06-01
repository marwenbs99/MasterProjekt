using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace test5.Models
{
    public class UserProfile
    {
        [Display(Name = "Old password")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Old password required")]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        public string OldPassword { get; set; }

        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        public string password { get; set; }
        


        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "type the same password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        public string ConfirmPassword { get; set; }



        
        public HttpPostedFileWrapper Picture { get; set; }

        public string ImageUrl { get; set; }

    }
}