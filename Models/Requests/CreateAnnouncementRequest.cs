using System.ComponentModel.DataAnnotations;

namespace ConsumerApi.Models.Requests;

public class CreateAnnouncementRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset StartsAt { get; set; }

    [Required]
    public DateTimeOffset EndsAt { get; set; }

    public bool IsActive { get; set; } = true;
}
