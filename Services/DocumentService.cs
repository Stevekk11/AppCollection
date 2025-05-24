using Microsoft.EntityFrameworkCore;
using SlovníHodiny.Models;

namespace SlovníHodiny.Services;

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