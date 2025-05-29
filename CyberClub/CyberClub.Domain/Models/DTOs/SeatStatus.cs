using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Models.DTOs
{
    public class SeatStatus
    {
        public int SeatID { get; set; }
        public string SeatNumber { get; set; }
        public bool IsAvailable { get; set; }
    }

}
