using System.ComponentModel.DataAnnotations;

namespace DrPioMuseumSurveyAPI.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public string Type { get; set; } = "rating";
        public bool IsRequired { get; set; } = true;
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; } = true;
    }
}