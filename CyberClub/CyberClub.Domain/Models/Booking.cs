using CyberClub.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public Status Status { get; set; }
        public int UserId { get; set; }
        public int SeatId { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }

        public Seat? Seat { get; set; }
        public Zone? Zone { get; set; }
    }
}
