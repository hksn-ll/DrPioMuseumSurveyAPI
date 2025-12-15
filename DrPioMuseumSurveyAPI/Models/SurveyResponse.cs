using System.ComponentModel.DataAnnotations;

namespace DrPioMuseumSurveyAPI.Models
{
    public class SurveyResponse
    {
        [Key]
        public int Id { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? StreetAddress { get; set; }
        public string? District { get; set; } 
        public string? Barangay { get; set; }
        public string? OtherLocation { get; set; }
        public string? AgeRange { get; set; }
        public string? Gender { get; set; }
        public string RatingsRaw { get; set; } = string.Empty;
        public string? LikedMost { get; set; }
        public string? Improvements { get; set; }
        public string? Recommend { get; set; }
        public string? Comments { get; set; }
    }
}