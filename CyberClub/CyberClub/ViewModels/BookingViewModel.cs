using CyberClub.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberClub.ViewModels
{
    public class BookingViewModel
    {
        public int UserID { get; set; }

        public List<Zone> Zones { get; set; } = new(); 
        public List<Seat> Seats { get; set; } = new();

        public int SelectedZoneId { get; set; }
        public int SelectedSeatId { get; set; }

        public DateTime SelectedDate { get; set; }
        public TimeSpan SelectedTime { get; set; }

        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public string? SelectedTimeRaw { get; set; }

        public int Duration { get; set; } 

        public bool ShowSeats => Seats != null && Seats.Any();
    }


}
