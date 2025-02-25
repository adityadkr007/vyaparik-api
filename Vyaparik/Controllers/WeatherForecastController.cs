using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Vyaparik.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpPost("send-email", Name = "send-email")]
        public async Task<IActionResult> SendMail(IFormCollection formData)
        {
            if (formData == null || formData.Count == 0)
            {
                return BadRequest("Invalid or empty form data.");
            }

            // AWS SES SMTP Server
            string smtpServer = "email-smtp.eu-central-1.amazonaws.com"; // Change to your AWS region
            int smtpPort = 587; // Use 465 for SSL, 587 for TLS

            // AWS SES SMTP Credentials (Found in IAM)
            string smtpUsername = "AKIAZOZQFNAKUZDIZT6G";
            string smtpPassword = "BBRSjEjuYPMLT2mQK9UOygiJ5IdplUJ0awah/nwUZ5El";

            // Sender and Recipient details
            string fromEmail = "adityadkr1@gmail.com"; // Must be verified in SES
            string toEmail = "justadiyt@gmail.com"; // Can be any email (if SES is out of sandbox)

            try
            {
                using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = true; // Use TLS encryption

                    // Start building the email body with a card layout
                    string emailBody = "<html><body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>" +
                                       "<div style='max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 10px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);'>" +
                                       "<div style='background-color: #d4edda; padding: 15px; border-radius: 10px 10px 0 0; text-align: center;'>" +
                                       "<h2 style='color: #155724; margin: 0;'>Contact Form Submission</h2></div>";

                    // Dynamically iterate through form data and create a section for each field
                    foreach (var kvp in formData)
                    {
                        string label = kvp.Key.Substring(0, 1).ToUpper() + kvp.Key.Substring(1).ToLower(); // Capitalize the first letter
                        string value = kvp.Value.FirstOrDefault() ?? "N/A";

                        // Create form-like structure with inline styles for each field
                        emailBody += $"<div style='background-color: #f8f9fa; padding: 10px; margin: 10px 0; border-radius: 5px; border-left: 4px solid #28a745;'>" +
                                     $"<label style='font-size: 14px; font-weight: bold; color: #333;'>{label}:</label><br>" +
                                     $"<span style='font-size: 14px; color: #555;'>{value}</span>" +
                                     "</div>";
                    }

                    // Footer section with inline styles
                    emailBody += "<div style='text-align: center; font-size: 12px; color: #777; margin-top: 20px;'>" +
                                 "<p>Thank you for your inquiry! We will get back to you soon.</p>" +
                                 "</div></div></body></html>";

                    MailMessage mail = new MailMessage(fromEmail, toEmail)
                    {
                        Subject = "Contact Form Submission",
                        Body = emailBody,
                        IsBodyHtml = true // Mark the email as HTML
                    };

                    await client.SendMailAsync(mail);
                    return Ok("Email sent successfully!");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }




    }
}

    
