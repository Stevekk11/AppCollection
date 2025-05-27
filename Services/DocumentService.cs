using Microsoft.EntityFrameworkCore;
using AppCollection.Models;

namespace AppCollection.Services;

/// <summary>
/// Provides services for managing documents, including adding, retrieving, and deleting user-specific documents.
/// </summary>
public class DocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly string _storageRoot;

    public DocumentService(ApplicationDbContext context, string storageRoot)
    {
        _context = context;
        _storageRoot = storageRoot;
        Directory.CreateDirectory(_storageRoot);
    }

    /// <summary>
    /// Asynchronously adds a new document to the database and stores the file in the specified storage location.
    /// </summary>
    /// <param name="loginId">The unique identifier of the user who is uploading the document.</param>
    /// <param name="file">The uploaded file to be stored, including file metadata such as name, content type, and size.</param>
    /// <returns>A task representing the asynchronous operation, with a Document object that contains the details of the uploaded document.</returns>
    public async Task<Document> AddDocumentAsync(int loginId, IFormFile file)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var storagePath = Path.Combine(_storageRoot, fileName);

        using (var stream = new FileStream(storagePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var document = new Document
        {
            LoginId = loginId,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            StoragePath = storagePath,
            UploadedAt = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        return document;
    }

    /// <summary>
    /// Asynchronously retrieves a list of documents associated with the specified user.
    /// </summary>
    /// <param name="loginId">The unique identifier of the user whose documents are being retrieved.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of Document objects.</returns>
    public async Task<List<Document>> GetUserDocumentsAsync(int loginId)
    {
        return await _context.Documents.Where(d => d.LoginId == loginId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }

    public async Task<Document> GetDocumentAsync(int documentId, int loginId)
    {
        return await _context.Documents.FirstOrDefaultAsync(d => d.Id == documentId && d.LoginId == loginId) ??
               throw new InvalidOperationException();
    }

    /// <summary>
    /// Asynchronously deletes a document from the database and its corresponding file from the storage location.
    /// </summary>
    /// <param name="documentId">The unique identifier of the document to be deleted.</param>
    /// <param name="loginId">The unique identifier of the user attempting to delete the document to ensure proper ownership validation.</param>
    /// <returns>A task representing the asynchronous operation, returning a boolean value indicating whether the deletion was successful or not.</returns>
    public async Task<bool> DeleteDocumentAsync(int documentId, int loginId)
    {
        var document = await GetDocumentAsync(documentId, loginId);
        if (document == null) return false;

        if (File.Exists(document.StoragePath))
        {
            File.Delete(document.StoragePath);
        }
        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();
        return true;
    }
}