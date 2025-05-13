namespace CyberClub.ViewModels
{
    public class ZoneWithSeatsViewModel
    {
        public int ZoneID { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string Color { get; set; }
        public List<SeatWithStatusViewModel> Seats { get; set; } = new();
    }

}
