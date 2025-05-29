using System.ComponentModel.DataAnnotations;

namespace CyberClub.ViewModels
{
    public class TournamentCreateViewModel
    {
        [Required(ErrorMessage = "Tournament name is required.")]
        [StringLength(100, ErrorMessage = "Name can't exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Game is required.")]
        [StringLength(100, ErrorMessage = "Game name can't exceed 100 characters.")]
        public string Game { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Tournament Date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Time is required.")]
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public TimeSpan Time { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("Upcoming|Ongoing|Completed", ErrorMessage = "Invalid status.")]
        public string Status { get; set; }

        [Range(4, int.MaxValue, ErrorMessage = "Participants must be at least 4.")]
        public int? MaxParticipants { get; set; }

        [StringLength(1000, ErrorMessage = "Description can't exceed 1000 characters.")]
        public string? Description { get; set; }

        [Display(Name = "Theme Color")]
        [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "Color must be a valid hex code (e.g., #ff0000).")]
        public string? ThemeColor { get; set; }

    }

}
