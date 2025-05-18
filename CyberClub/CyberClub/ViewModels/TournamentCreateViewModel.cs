using System.ComponentModel.DataAnnotations;

namespace CyberClub.ViewModels
{
    public class TournamentCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Game { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan Time { get; set; }

        [Required]
        public string Status { get; set; }  

        public string? Description { get; set; }
    }

}
