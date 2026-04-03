namespace TourPlanner.BL.DTOs;

public class TourDto
{
    public int Id { get; set; }
    public string Name { get; set; }= string.Empty;
    public string Description { get; set; }= string.Empty;
    public string From { get; set; }= string.Empty;
    public string To { get; set; }= string.Empty;
    public string TransportType { get; set; } = string.Empty;
    public double Distance { get; set; }
    public double EstimatedTime { get; set; }
}