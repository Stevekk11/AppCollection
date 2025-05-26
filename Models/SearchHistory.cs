using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCollection.Models;

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