namespace TourPlanner.BL.DTOs;

public class Coords
{
    public string Name { get; set; } = string.Empty;
    public decimal Lat { get; set; } = 0;
    public decimal Lng { get; set; } = 0;
}

public class ToFromCoords
{
    public Coords ToCoord { get; set; } = new Coords();
    public Coords FromCoord { get; set; } = new Coords();
}