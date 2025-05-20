using System.ComponentModel.DataAnnotations;

namespace SlovníHodiny.Models;

/// <summary>
/// Represents user login information.
/// </summary>
/// <remarks>
/// This class is used for storing login credentials and associated metadata
/// such as the user type. It is part of the Models namespace and
/// leverages Entity Framework Core annotations for database schema creation.
/// </remarks>
public class Login
{
    [Key]
    public int Id_Login { get; set; }

    [Required] [StringLength(50)] public string Username { get; set; }

    [Required] [StringLength(50)] public string Password { get; set; } // This will store the hashed password

    [Required] public byte Usertype { get; set; } // 1 - Admin, 2 - Normal user
}