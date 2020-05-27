using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace test5.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class Users
    {
        public string ConfirmPassword { get; set; }
        
    }
    public class UserMetadata
    {
        [Display(Name= "First Name")]
        [Required(AllowEmptyStrings =false, ErrorMessage ="First Name required")]
        public String FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name required")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = " Email required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateOfBirth { get; set; }


        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [MinLength(6,ErrorMessage = "Minimum 6 characters required")]
        public string Password { get; set; }



        [Display(Name ="Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Confirm password and do not match")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        public string ConfirmPassword { get; set; }



        
    }
}