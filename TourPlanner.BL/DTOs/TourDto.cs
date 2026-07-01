using System.Drawing;
using TourPlanner.BL.Services;

namespace TourPlanner.BL.DTOs;

public class TourDto
{
    public int Id { get; set; }
    public string Name { get; set; }= string.Empty;
    public string Description { get; set; }= string.Empty;
    public string From { get; set; }= string.Empty;
    public string To { get; set; }= string.Empty;
    public string RouteInformation { get; set; }= string.Empty;
    public OpenRoute OpenRoute { get; set; } = new OpenRoute();
}