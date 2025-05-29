using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Models.DTOs
{
    public class ZoneSeatMap
    {
        public int ZoneID { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public List<SeatStatus> Seats { get; set; } = new();
    }
}
