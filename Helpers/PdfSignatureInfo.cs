namespace AppCollection.Helpers;

/// <summary>
/// Represents information about a digital signature within a PDF document.
/// </summary>
public class PdfSignatureInfo
{
    public bool IsSigned { get; set; }
    public string? SignerName { get; set; }
    public DateTime? SignatureDate { get; set; }
    public int SignatureCount { get; set; }
}