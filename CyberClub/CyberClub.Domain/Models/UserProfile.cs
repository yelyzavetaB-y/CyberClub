using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public int PhoneNumber { get; set; }
        public DateTime DOB { get; set; }
        public User? User { get; set; }
        public int UserId { get; set; }

    }

}
