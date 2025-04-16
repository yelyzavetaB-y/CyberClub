using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Models
{
    public class SeatWithStatus : Seat
    {
        public bool IsAvailableForBooking { get; set; }
    }
}
