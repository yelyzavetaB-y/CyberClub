namespace CyberClub.ViewModels
{
    public class TournamentViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Game { get; set; }

        public DateTime StartDateTime { get; set; }

        public string Status { get; set; }

        public string? ThemeColor { get; set; }

        public string? Description { get; set; }

        public bool IsOnline { get; set; }

        public int? MaxParticipants { get; set; }
    }
}
