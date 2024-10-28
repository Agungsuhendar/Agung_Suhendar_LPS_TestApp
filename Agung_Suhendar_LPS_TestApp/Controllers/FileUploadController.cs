using Agung_Suhendar_LPS_TestApp.Data;
using Agung_Suhendar_LPS_TestApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agung_Suhendar_LPS_TestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly LPSDbContext _context;
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedDocuments");

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] int chunkIndex, [FromForm] int totalChunks, [FromForm] string fileName)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var filePath = Path.Combine(_storagePath, file.FileName);
            Directory.CreateDirectory(_storagePath);

            using (var stream = new FileStream(filePath, chunkIndex == 0 ? FileMode.Create : FileMode.Append))
            {
                await file.CopyToAsync(stream);
            }

            if (chunkIndex == totalChunks - 1)
            {
                await StoreDocumentInDatabase(filePath, file.FileName);
                SendNotification(file.FileName);
            }

            return new JsonResult(new { chunkIndex, totalChunks });
        }

        private async Task StoreDocumentInDatabase(string filePath, string fileName)
        {
            byte[] fileContent = await System.IO.File.ReadAllBytesAsync(filePath);

            var document = new Document
            {
                Name = fileName,
                Content = fileContent,
                UploadedDate = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
        }

        private void SendNotification(string fileName)
        {
            // Logic to send notification (e.g., email, SMS, etc.)
            System.Diagnostics.Debug.WriteLine($"Document {fileName} uploaded successfully.");
        }
    }
}
