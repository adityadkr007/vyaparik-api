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
        [HttpGet("send-email",Name ="send-email")]
        public async Task<IActionResult> SendMail()
        {// AWS SES SMTP Server
            string smtpServer = "email-smtp.eu-central-1.amazonaws.com"; // Change to your AWS region
            int smtpPort = 587; // Use 465 for SSL, 587 for TLS

            // AWS SES SMTP Credentials (Found in IAM)
            string smtpUsername = "AKIAZOZQFNAKUZDIZT6G\r\n";
            string smtpPassword = "BBRSjEjuYPMLT2mQK9UOygiJ5IdplUJ0awah/nwUZ5El\r\n";

            // Sender and Recipient details
            string fromEmail = "adityadkr1@gmail.com"; // Must be verified in SES
            string toEmail = "justadiyt@gmail.com"; // Can be any email (if SES is out of sandbox)

            try
            {
                using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = true; // Use TLS encryption

                    MailMessage mail = new MailMessage(fromEmail, toEmail)
                    {
                        Subject = "Test Email from AWS SES via SMTP",
                        Body = "Hello! This is a test email sent using AWS SES SMTP in .NET.",
                        IsBodyHtml = false
                    };

                    await client.SendMailAsync(mail);
                   return Ok("Email sent successfully!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending email: {ex.Message}");
            }

        }
    }
}

    
