using System.ComponentModel.DataAnnotations;

namespace SlovníHodiny.Models;

public class Login
{
    [Key]
    public int Id_Login { get; set; }

    [Required] [StringLength(50)] public string Username { get; set; }

    [Required] [StringLength(50)] public string Password { get; set; } // This will store the hashed password

    [Required] public byte Usertype { get; set; } // 1 - Admin, 2 - Normal user
}