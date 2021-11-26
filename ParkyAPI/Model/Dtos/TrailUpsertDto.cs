using System.ComponentModel.DataAnnotations;
using static ParkyAPI.Model.Trail;

namespace ParkyAPI.Model.Dtos
{
    public class TrailUpsertDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Distance { get; set; }

        [Required]
        public double Elevation { get; set; }

        public DifficultyType Difficulty { get; set; }

        [Required]
        public int NationalParkId { get; set; }

    }
}
