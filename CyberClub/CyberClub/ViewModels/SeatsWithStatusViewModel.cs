namespace CyberClub.ViewModels
{
    public class SeatWithStatusViewModel
    {
        public int SeatID { get; set; }
        public string SeatNumber { get; set; }
        public int ZoneID { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsAvailableForBooking { get; set; }
    }

}
