namespace TourPlanner.BL.DTOs;

public class TourLogDto
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public DateTime Date { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public double TotalDistance { get; set; }
    public double TotalTime { get; set; }
    public int Rating  { get; set; }
    
}