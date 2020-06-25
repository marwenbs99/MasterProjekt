
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test5.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            Message = new HashSet<Message>();

        }
        public virtual ICollection<Message> Message { get; set; }
    }
}