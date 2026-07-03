using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourPlanner.DAL.Entities
{
    public class TourLog
    {
        [Key]
        public int log_id { get; set; }
        public DateTime logDateTime { get; set; }
        public string? comment { get; set; }
        public int difficulty { get; set; }
        public double totalDistance { get; set; }
        public int totalTime { get; set; }
        public int rating { get; set; }
        public int tour_id { get; set; }
        [ForeignKey(nameof(tour_id))]
        public Tour Tour { get; set; } = null!;
    }
}