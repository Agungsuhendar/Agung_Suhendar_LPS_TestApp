using Agung_Suhendar_LPS_TestApp.Data;
using Agung_Suhendar_LPS_TestApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IO;

namespace Agung_Suhendar_LPS_TestApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly LPSDbContext _context;
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedDocuments");

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] int chunkIndex, [FromForm] int totalChunks, [FromForm] string fileName)
        {
            try
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
            }
            catch (Exception ex)
            {

            }
            return new JsonResult(new { chunkIndex, totalChunks });
        }

        private async Task StoreDocumentInDatabase(string filePath, string fileName)
        {
            try
            {
                byte[] fileContent = await System.IO.File.ReadAllBytesAsync(filePath);


                var document = new Document
                {
                    Name = fileName,
                    ContentFile = fileContent,
                    UploadedDate = DateTime.UtcNow
                };

                _context.Documents.Add(document);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }

        private bool SendNotification(string fileName)
        {
            bool result = false;
            try
            {

                System.Diagnostics.Debug.WriteLine($"Document {fileName} uploaded successfully.");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }
    }
}
