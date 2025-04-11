using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Models
{
    public class Seat
    {
        public int SeatID { get; set; }
        public int ZoneID { get; set; }
        public string SeatNumber { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? ReservedStartTime { get; set; }
    }
}
