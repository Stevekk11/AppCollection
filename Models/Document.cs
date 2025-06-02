using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCollection.Models;

/// <summary>
/// Represents a document entity that is associated with a user and contains metadata about a stored file.
/// </summary>
/// <remarks>
/// This class is primarily used for managing file information, including its storage path, metadata, and upload timestamp.
/// It leverages Entity Framework Core annotations for database schema creation.
/// </remarks>
public class Document
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int LoginId { get; set; }
    [ForeignKey("LoginId")]
    public Login Login { get; set; }
    [Required]
    public string FileName { get; set; }
    [Required]
    public string ContentType { get; set; }
    [Required]
    public long FileSize { get; set; }
    [Required]
    public string StoragePath { get; set; }
    public DateTime UploadedAt { get; set; }
}