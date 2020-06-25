using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace test5.Models
{
    public class Message
    {
        
        public int ID { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime When { get; set; }
        public int UserID { get; set; }

        public virtual AppUser AppUser { get; set; }


    }
}