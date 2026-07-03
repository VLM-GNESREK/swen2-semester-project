using System.ComponentModel.DataAnnotations;

namespace TourPlanner.DAL.Entities
{
    public class User
    {
        [Key]
        public int user_id { get; set; }
        public string username { get; set; }
        public string pw_hash { get; set; }
        public ICollection<Tour> Tours { get; set; } = new List<Tour>();
    }
}