namespace TourPlanner.BL.DTOs
{
    public class TourLogDTO
    {
        public int ID { get; set; }
        public int TourID { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Difficulty { get; set; }
        public double TotalDistance { get; set; }
        public int TotalTime { get; set; }
        public int Rating  { get; set; }
    }
}