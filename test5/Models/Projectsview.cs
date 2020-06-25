using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test5.Models
{
    public class Projectsview
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String ManagerName { get; set; }
        public Boolean IfManager { get; set; }
        public String Statut { get; set; }
    }
}