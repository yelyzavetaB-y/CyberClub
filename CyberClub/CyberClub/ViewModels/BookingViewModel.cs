using CyberClub.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberClub.ViewModels
{
    public class BookingViewModel
    {
        [Required(ErrorMessage ="UserID is required")]
        public int UserID { get; set; }

        public List<Zone> Zones { get; set; } = new(); 
        public List<Seat> Seats { get; set; } = new();

        public int SelectedZoneId { get; set; }

        [Required(ErrorMessage ="Seat must be selected")]
        public int SelectedSeatId { get; set; }


        [Required(ErrorMessage = "Date must be selected")]
        public DateTime SelectedDate { get; set; }


        [Required(ErrorMessage = "Time must be selected")]
        public TimeSpan SelectedTime { get; set; }

        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public string? SelectedTimeRaw { get; set; }

        [Range(60, 180, ErrorMessage ="Duration must be between 1 and 3 hours")]
        public int Duration { get; set; }
        public List<ZoneWithSeatsViewModel> ZonesWithSeats { get; set; } = new();

    }


}
