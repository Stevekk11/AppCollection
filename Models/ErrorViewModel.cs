namespace AppCollection.Models;

/// <summary>
/// Represents a model to handle error information in the application.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}