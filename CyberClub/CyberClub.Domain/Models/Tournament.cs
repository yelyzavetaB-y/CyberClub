using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Game { get; set; }
        public DateTime StartDateTime { get; set; }
        public string Status { get; set; } 
        public string? Description { get; set; }
        public int? MaxParticipants { get; set; }
        public string? ThemeColor { get; set; }
        public int CurrentParticipantCount { get; set; }

    }

}
