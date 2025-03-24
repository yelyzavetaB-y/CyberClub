using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int SeatId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public DateTime ReservedStartTime { get; set; }
        public int Duration { get; set; }



    }

}
