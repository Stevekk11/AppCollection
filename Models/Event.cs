using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCollection.Models;

/// <summary>
/// Represents an event within the application system.
/// </summary>
/// <remarks>
/// The Event class is used to store details of a scheduled event,
/// including its name, start date, end date, and associated user.
/// It includes data annotations for restrictions and database mappings.
/// </remarks>
public class Event
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("LoginId")]
    public int UserId { get; set; }
    [Required]// Foreign key to Logins
    public string Name { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }

    public Login User { get; set; }
}