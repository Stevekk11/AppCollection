using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCollection.Models;

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