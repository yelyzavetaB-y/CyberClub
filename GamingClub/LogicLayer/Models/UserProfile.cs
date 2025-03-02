using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public int PhoneNumber { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
    }

}
