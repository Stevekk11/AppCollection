using AppCollection.Helpers;
using iText.Kernel.Pdf;
using iText.Signatures;

namespace AppCollection.Services;

/// <summary>
/// Provides services for handling PDF signature functionalities, including verifying if a PDF is signed
/// and retrieving signature details from a PDF document.
/// </summary>
public class PdfSignatureService
{
    /// <summary>
    /// Determines whether a PDF document, specified by its file path, contains a digital signature.
    /// </summary>
    /// <param name="filePath">The file path of the PDF document to check.</param>
    /// <returns>Returns true if the PDF document is signed; otherwise, false.</returns>
    public bool IsPdfSigned(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            using var pdfReader = new PdfReader(filePath);
            using var pdfDocument = new PdfDocument(pdfReader);

            var signatureUtil = new SignatureUtil(pdfDocument);
            var signatureNames = signatureUtil.GetSignatureNames();

            if (!signatureNames.Any())
            {
                return false;
            }

            foreach (var name in signatureNames)
            {
                var pkcs7 = signatureUtil.ReadSignatureData(name);
                if (pkcs7 != null)
                {
                    return true;
                }
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
            //logger
        }
    }

    /// <summary>
    /// Retrieves information about the digital signature(s) in a specified PDF document.
    /// </summary>
    /// <param name="filePath">The file path of the PDF document to analyze.</param>
    /// <returns>A <see cref="PdfSignatureInfo"/> object containing details about the signature(s)
    /// in the PDF, including whether it is signed, the signer name, signature date,
    /// and the total number of signatures. If the document is not signed or an error occurs,
    /// returns an object with default values.</returns>
    public PdfSignatureInfo GetPdfSignatureInfo(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return new PdfSignatureInfo { IsSigned = false };
            }
            using var pdfReader = new PdfReader(filePath);
            using var pdfDocument = new PdfDocument(pdfReader);
            var signatureUtil = new SignatureUtil(pdfDocument);
            var signatureNames = signatureUtil.GetSignatureNames();

            if (!signatureNames.Any())
            {
                return new PdfSignatureInfo { IsSigned = false };
            }

            var firstSignatureName = signatureNames.First();
            var pkcs7 = signatureUtil.ReadSignatureData(firstSignatureName);


            if (pkcs7 == null)
            {
                return new PdfSignatureInfo { IsSigned = false };
            }

            return new PdfSignatureInfo
            {
                IsSigned = true,
                SignerName = pkcs7.GetSigningCertificate()?.GetSubjectDN()?.ToString(),
                SignatureDate = pkcs7.GetSignDate(),
                SignatureCount = signatureNames.Count
            };
        }
        catch (Exception e)
        {
            return new PdfSignatureInfo { IsSigned = false };
        }
    }

}