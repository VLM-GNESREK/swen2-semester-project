namespace TourPlanner.BL.DTOs;

public class OpenRoute
{
    public ToFromCoords ToFromCoords { get; set; } = new ToFromCoords();
    public string TransportType { get; set; } = string.Empty;
    public decimal Duration { get; set; }
    public decimal Distance { get; set; }
    public List<Coords> Steps { get; set; } = new List<Coords>();
}