using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCollection.Models;

/// <summary>
/// Represents a record of a search performed by a user within the application.
/// </summary>
/// <remarks>
/// This class is used to store and retrieve information about searches,
/// including the search term, the date and time of the search, and the user who performed it.
/// </remarks>
public class SearchHistory
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Word { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [ForeignKey("LoginId")]
    public int UserId { get; set; }

    public Login User { get; set; }
}