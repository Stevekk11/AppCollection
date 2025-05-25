using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AppCollection.Models;
using AppCollection.Services;
using System.Drawing;

namespace AppCollection.Controllers;

/// <summary>
/// Controller class responsible for managing user documents.
/// Provides functionality for uploading, downloading, deleting documents,
/// and retrieving image details or image gallery views.
/// </summary>
[Authorize]
public class DocumentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly DocumentService _documentService;
    private readonly PdfSignatureService _pdfSignatureService;

    public DocumentController(ApplicationDbContext context, IConfiguration configuration,
        DocumentService documentService, PdfSignatureService pdfSignatureService)
    {
        _context = context;
        _documentService = documentService;
        _pdfSignatureService = pdfSignatureService;
    }

    /// <summary>
    /// Retrieves the currently authenticated user's unique identifier based on their username and user type.
    /// </summary>
    /// <returns>An integer representing the unique identifier of the currently authenticated user.</returns>
    /// <exception cref="Exception">Thrown if the current user's information cannot be found in the database.</exception>
    private int GetCurrentUserId()
    {
        var username = User.Identity.Name;
        var usertype = Convert.ToByte(User.FindFirst("Usertype").Value);
        var user = _context.Logins.FirstOrDefault(x => x.Username == username && x.Usertype == usertype);
        return user?.Id_Login ?? throw new Exception("User not found");
    }

    /// <summary>
    /// Displays the user's document management dashboard, listing all documents associated
    /// with the currently authenticated user.
    /// </summary>
    /// <returns>A view displaying the user's documents and associated data.</returns>
    /// <exception cref="Exception">Thrown if the authenticated user's information cannot be retrieved.</exception>
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var docs = await _documentService.GetUserDocumentsAsync(userId);
        var documentsWithSignature = docs.Select(doc => new DocumentWithSignature
        {
            Document = doc,
            IsPdfSigned = doc.ContentType == "application/pdf" && _pdfSignatureService.IsPdfSigned(doc.StoragePath)
        }).ToList();

        return View(new DocumentViewModel { DocumentsWithSignature = documentsWithSignature });
    }

    /// <summary>
    /// Retrieves detailed information about the digital signatures present in a PDF document.
    /// </summary>
    /// <param name="id">The unique identifier of the PDF document.</param>
    /// <returns>A JSON result containing information about the PDF signatures, including whether the document is signed, the signer's name, the signature date, and the total number of signatures.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the specified document cannot be found or does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the document is not of type 'application/pdf'.</exception>
    [HttpGet]
    public async Task<IActionResult> GetPdfSignatureInfo(int id)
    {
        var userId = GetCurrentUserId();
        var doc = await _documentService.GetDocumentAsync(id, userId);

        if (doc == null || doc.ContentType != "application/pdf")
            return NotFound();

        var signatureInfo = _pdfSignatureService.GetPdfSignatureInfo(doc.StoragePath);

        return Json(new {
            isSigned = signatureInfo.IsSigned,
            signerName = signatureInfo.SignerName,
            signatureDate = signatureInfo.SignatureDate?.ToString("dd.MM.yyyy HH:mm"),
            signatureCount = signatureInfo.SignatureCount
        });
    }


    /// <summary>
    /// Handles the file upload process for the currently authenticated user. The uploaded file is stored and associated with the user's account.
    /// </summary>
    /// <param name="file">The file being uploaded by the user.</param>
    /// <returns>An <see cref="IActionResult"/> that redirects to the index page after the file is successfully uploaded or returns an error if the upload fails.</returns>
    /// <exception cref="Exception">Thrown if no file is selected for upload or if an error occurs during file processing.</exception>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["UploadError"] = "No file selected.";
            return RedirectToAction(nameof(Index));
        }

        var userId = GetCurrentUserId();
        await _documentService.AddDocumentAsync(userId, file);

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Handles the download process for a requested document by its unique identifier.
    /// Validates user permissions and ensures the file exists before serving it.
    /// </summary>
    /// <param name="id">The unique identifier of the document to be downloaded.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the requested file for download
    /// if successful, or an appropriate HTTP status code if an error occurs.
    /// </returns>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist on the storage system.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown if the current user is not authorized to access the document.</exception>
    /// <exception cref="Exception">Thrown for any unexpected errors occurring during the download process.</exception>
    public async Task<IActionResult> Download(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var doc = await _documentService.GetDocumentAsync(id, userId);
            if (doc == null)
                return NotFound();

            var mimeType = doc.ContentType ?? "application/octet-stream";
            var absolutePath = Path.GetFullPath(doc.StoragePath);

            if (!System.IO.File.Exists(absolutePath))
                return NotFound();

            return PhysicalFile(absolutePath, mimeType, doc.FileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "Unexpected error while downloading document {DocumentId}", id);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// Retrieves and displays a filtered list of image documents uploaded by the current user.
    /// Only documents with supported image content types (e.g., jpeg, png, gif, etc.) are included,
    /// and they are sorted in descending order based on their upload date.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> rendering the user's image gallery within a
    /// <see cref="DocumentViewModel"/>.</returns>
    /// <exception cref="Exception">Thrown if the current user's information cannot be retrieved.</exception>
    public async Task<IActionResult> Gallery()
    {
        var userId = GetCurrentUserId();
        var imageTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/bmp" };

        var images = await _context.Documents
            .Where(d => d.LoginId == userId && imageTypes.Contains(d.ContentType.ToLower()))
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();

        return View(new DocumentViewModel { Documents = images });
    }

    public async Task<IActionResult> Music()
    {
        var userId = GetCurrentUserId();
        var audioTypes = new[] { "audio/mpeg", "audio/mp3", "audio/wav", "audio/x-wav", "audio/x-ms-wma", "audio/x-ms-wax", "audio/ogg" };

        var audioFiles = await _context.Documents.Where(d => d.LoginId == userId && audioTypes.Contains(d.ContentType.ToLower()))
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
        return View(new DocumentViewModel { Documents = audioFiles });
    }

    /// <summary>
    /// Retrieves detailed information about a specific image document for the current user.
    /// If the specified document does not exist or is not an image, a 404 Not Found result is returned.
    /// </summary>
    /// <param name="id">The unique identifier of the document.</param>
    /// <returns>An <see cref="IActionResult"/> containing the image details in JSON format,
    /// or a NotFound result if the document is unavailable or not an image.</returns>
    /// <exception cref="Exception">Thrown if the user cannot be identified or document retrieval fails.</exception>
    [HttpGet]
    public async Task<IActionResult> GetImageDetails(int id)
    {
        var userId = GetCurrentUserId();
        var doc = await _documentService.GetDocumentAsync(id, userId);

        if (doc == null || !doc.ContentType.StartsWith("image/"))
            return NotFound();

        var absolutePath = Path.GetFullPath(doc.StoragePath);
        using var image = Image.FromFile(absolutePath);

        return Json(new
        {
            id = doc.Id,
            fileName = doc.FileName,
            fileSize = FormatFileSize(doc.FileSize),
            uploadedAt = doc.UploadedAt.ToString("dd.MM.yyyy HH:mm"),
            contentType = doc.ContentType,
            downloadUrl = Url.Action("Download", new { id = doc.Id }),
            width = image.Width,
            height = image.Height
        });
    }

    [HttpGet]
    public async Task<IActionResult> Preview(int id)
    {
        var userId = GetCurrentUserId();
        var doc = await _documentService.GetDocumentAsync(id, userId);
        if (doc == null)
            return NotFound();

        var mimeType = doc.ContentType ?? "application/octet-stream";
        var absolutePath = Path.GetFullPath(doc.StoragePath);

        if (!System.IO.File.Exists(absolutePath))
            return NotFound();

        // Inline disposition for browser preview
        return PhysicalFile(absolutePath, mimeType, enableRangeProcessing: true);
    }


    /// <summary>
    /// Formats the file size to be human readable.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    /// <summary>
    /// Deletes a specific document from the user's account.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        await _documentService.DeleteDocumentAsync(id, userId);
        return RedirectToAction(nameof(Index));
    }
}