using System.ComponentModel.DataAnnotations;

namespace TourPlanner.BL.DTOs;

public class FactDto
{
    public string Id { get; set; }

    public string Username { get; set; }

    public string Text { get; set; }

    public string Source { get; set; }

    public string SourceUrl { get; set; }

    public string Language { get; set; }
    
    [Required]
    public required string Fact { get; set; }
    
    [Required]
    public required int Length { get; set; }
}