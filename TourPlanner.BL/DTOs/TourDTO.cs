namespace TourPlanner.BL.DTOs;

public class TourDTO
{
    public int ID { get; set; }
    public string Name { get; set; }= string.Empty;
    public string Description { get; set; }= string.Empty;
    public string From { get; set; }= string.Empty;
    public string To { get; set; }= string.Empty;
    public string TransportType { get; set; } = string.Empty;
    public double Distance { get; set; }
    public int EstimatedTime { get; set; }
    public string ImageRouteInformation { get; set; }= string.Empty;
    public int Popularity { get; set; }
    public bool IsChildFriendly { get; set; }
}