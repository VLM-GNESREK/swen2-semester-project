using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourPlanner.DAL.Entities
{
    public class Tour
    {
        [Key] public int tour_id { get; set; }
        public string tour_name { get; set; }
        public string? description { get; set; }
        public string startLocation { get; set; }
        public string targetLocation { get; set; }
        public string transportType { get; set; }

        public decimal distance { get; set; }
        public decimal FromLat { get; set; }
        public decimal FromLng { get; set; }
        public decimal ToLat { get; set; }
        public decimal ToLng { get; set; }
        public decimal estimatedTime { get; set; }
        public string routeImagePath { get; set; }
        public int? popularity { get; set; }
        public int? childFriendly { get; set; }
        public int user_id { get; set; }
        [ForeignKey(nameof(user_id))] public User User { get; set; } = null!;
        public ICollection<TourLog> TourLogs { get; set; } = new List<TourLog>();
    }
}