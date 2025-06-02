using NUnit.Framework;
using AppCollection.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace NUnitTests;

[TestFixture]
public class DocumentTest
{
    private ApplicationDbContext _context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "DocumentsTestDb")
            .Options;
        _context = new ApplicationDbContext(options);

        var user = new Login { Username = "docsuser", Password = "docs", Usertype = 1 };
        _context.Logins.Add(user);
        _context.SaveChanges();
    }

    [Test]
    public void AddDocument_CanBeRetrieved()
    {
        var user = _context.Logins.First();
        var doc = new Document
        {
            LoginId = user.Id_Login,
            FileName = "example.pdf",
            ContentType = "application/pdf",
            FileSize = 12345,
            StoragePath = "/files/example.pdf",
            UploadedAt = DateTime.UtcNow
        };

        _context.Documents.Add(doc);
        _context.SaveChanges();

        var stored = _context.Documents.FirstOrDefault(d => d.FileName == "example.pdf");
        Assert.That(stored, Is.Not.Null);
        Assert.That(stored.FileName, Is.EqualTo("example.pdf"));
        Assert.That(user.Id_Login, Is.EqualTo(stored.LoginId));
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}