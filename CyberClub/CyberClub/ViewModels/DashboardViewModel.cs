namespace CyberClub.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalBookings { get; set; }
        public int UpcomingBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int AverageDuration { get; set; }
        public List<ZoneStat> ZoneBookingStats { get; set; }
    }

    public class ZoneStat
    {
        public string ZoneName { get; set; }
        public int BookingCount { get; set; }
    }

}
