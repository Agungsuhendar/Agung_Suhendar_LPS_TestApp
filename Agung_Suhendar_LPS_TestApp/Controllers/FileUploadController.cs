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
                ////4. Sent Email and Update OTP
                MailSending xMailSending = new MailSending();
                MailProperties xMail = new MailProperties();
                //xMail.SMTPProperties = xSMTP;
                //xMail.From = xSMTP.AccountMailAddress;
                //xMail.To = xEmail;
                //xMail.CC = xLoginOTPModel.EmailCC;
                //xMail.BCC = xLoginOTPModel.EmailBCC;
                //xMail.IsHTML = true;
                //xMail.SubjectEmail = xLoginOTPModel.OTPEmailSubject == null ? "[No Reply] Email OTP Iflow (via Application)" : xLoginOTPModel.OTPEmailSubject + "(via Application)";
                //xMail.BodyEmail = xLoginOTPModel.OTPEmailBody == null ? "" : xLoginOTPModel.OTPEmailBody;
                //xMail.BodyEmail = xMail.BodyEmail.Replace("[Email]", xLoginOTPModel.Employee.Email);
                //xMail.BodyEmail = xMail.BodyEmail.Replace("[FullName]", xLoginOTPModel.Employee.FullName);
                //xMail.BodyEmail = xMail.BodyEmail.Replace("[NumberOTP]", xLoginOTPModel.NumberOTP.ToString());
                //double xExpiredSecondTime = Double.Parse(xLoginOTPModel.ExpiredSecondTimeOTP.ToString());
                //System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("id-ID");
                //DateTime CurrentTime = DateTime.UtcNow.AddHours(7);
                //xLoginOTPModel.ExpiredDateTimeOTP = CurrentTime.AddSeconds(xExpiredSecondTime);
                //string NextTimeExpiredString = xLoginOTPModel.ExpiredDateTimeOTP.ToString("dddd, dd MMMM yyyy (Pukul: HH:mm:ss) ", cultureinfo);
                //xMail.BodyEmail = xMail.BodyEmail.Replace("[NextTimeExpired]", NextTimeExpiredString);
                //xMail.BodyEmail = xMail.BodyEmail.Replace("[ExpiredSecondTimeOTP]", xLoginOTPModel.ExpiredSecondTimeOTP.ToString());

                ////Process Sending via App
                //if (xLoginOTPModel.UsingMailApp.HasValue && xLoginOTPModel.UsingMailApp.Value == true)
                //{
                //    using (var dbContextTransaction = context.Database.BeginTransaction())
                //    {
                //        try
                //        {
                //            xMail = xMailSending.Send(xMail);
                //            if (xMail.IsError.HasValue && xMail.IsError.Value == true)
                //            {
                //                dbContextTransaction.Rollback();
                //                ProcessResult.Error(result, xMail.Messages);
                //                result.Data = contentData;
                //                return result;
                //            }
                //            else
                //            {
                //                contentData.OTPEncode = xLoginOTPModel.EncodeOTP;
                //                contentData.OTPExpiryTime = xLoginOTPModel.ExpiredDateTimeOTP;
                //                context.Entry(contentData).State = EntityState.Modified;
                //                context.SaveChanges();
                //                dbContextTransaction.Commit();
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            dbContextTransaction.Rollback();
                //            ProcessResult.Error(result, ex);
                //            result.Data = contentData;
                //            return result;
                //        }
                //    }
                //}
                // xMailSending.

                //// 5. update send otp via email using exec sp(otp, otpencode, toEmail)

                //if (xLoginOTPModel.UsingMailApp.HasValue == false || (xLoginOTPModel.UsingMailApp.HasValue && xLoginOTPModel.UsingMailApp.Value == false))
                //{
                //    // SqlConnection conn = new SqlConnection(context.Database.GetConnectionString());
                //    using (SqlConnection xConn = new SqlConnection(context.Database.GetConnectionString()))
                //    {
                //        xConn.Open();
                //        using (SqlCommand xCmd = new SqlCommand())
                //        {
                //            SqlParameter IsSuccess = new SqlParameter()
                //            {
                //                ParameterName = "@IsSuccess",
                //                Value = "-",
                //                SqlDbType = System.Data.SqlDbType.VarChar,
                //                Direction = ParameterDirection.Output
                //            };
                //            xCmd.Connection = xConn;
                //            xCmd.CommandText = "SentEmailOTP";
                //            xCmd.CommandType = CommandType.StoredProcedure;
                //            xCmd.Parameters.Add(new("@OTPNumber", xLoginOTPModel.NumberOTP));
                //            xCmd.Parameters.Add(new SqlParameter("@OTPNumberEncode", xLoginOTPModel.EncodeOTP));
                //            xCmd.Parameters.Add(new SqlParameter("@EmailTo", contentData.Email));
                //            xCmd.Parameters.Add(IsSuccess);

                //            using (var reader = xCmd.ExecuteReader()) //error occurs here
                //            {
                //                while (reader.Read())
                //                {
                //                    if (IsSuccess.SqlValue != null)
                //                    {
                //                        xLoginOTPModel.ResultSentOTP = IsSuccess.SqlValue.ToString();
                //                    }
                //                    else
                //                    {
                //                        xLoginOTPModel.ResultSentOTP = reader.GetString(0);
                //                    }
                //                }
                //                reader.Close();
                //            }
                //            xCmd.Dispose();
                //        }
                //        xConn.Close();
                //    }

                //    if (xLoginOTPModel.ResultSentOTP.ToLower() != "success")
                //    {
                //        ProcessResult.Error(result, "Error exec store procedure SentEmailOTP! ");
                //        result.Data = contentData;
                //        return result;
                //    }
                //}
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
