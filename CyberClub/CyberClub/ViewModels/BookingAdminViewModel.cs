namespace CyberClub.ViewModels
{
    public class BookingAdminViewModel
    {
        public int BookingID { get; set; }
        public string UserEmail { get; set; }
        public string ZoneName { get; set; }
        public string SeatNumber { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }
    }

}
