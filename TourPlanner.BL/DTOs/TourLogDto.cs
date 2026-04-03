namespace TourPlanner.BL.DTOs;

public class TourLogDto
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public DateTime Date { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public double TotalDistance { get; set; }
    public double TotalTime { get; set; }
    public int Rating  { get; set; }
    
}