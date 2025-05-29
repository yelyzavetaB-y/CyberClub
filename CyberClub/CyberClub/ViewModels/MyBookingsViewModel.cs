namespace CyberClub.ViewModels
{
    public class MyBookingsViewModel
    {
        public List<UserBookingInfo> Bookings { get; set; } = new();
        public List<TournamentViewModel> Tournaments { get; set; }
    }

    public class UserBookingInfo
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public string ZoneName { get; set; }
        public string SeatNumber { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }
    }
}
