using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Akce pro zobrazení domovské stránky
        public IActionResult Index()
        {
            return View(); // Vrátí Index.cshtml
        }

        // Akce pro zpracování formuláøe
        [HttpPost]
        public IActionResult OdeslatData(string name, string email, string message, IFormFile modelFile)
        {
            // E-mail pro tebe (pøíjemce poptávky)
            var emailMessageToYou = new MimeMessage();
            _logger.LogInformation("OdeslatData byla zavolána");

            emailMessageToYou.From.Add(new MailboxAddress("Tiskni3D", "tiskni3dd@seznam.cz"));
            emailMessageToYou.To.Add(new MailboxAddress("Tiskni3D", "tiskni3dd@seznam.cz"));
            emailMessageToYou.Subject = "Nová poptávka na 3D tisk";

            var bodyBuilderToYou = new BodyBuilder
            {
                TextBody = $"Jméno: {name}\nEmail zákazníka: {email}\nZpráva: {message}"
            };

            if (modelFile != null && modelFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    modelFile.CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();
                    bodyBuilderToYou.Attachments.Add(modelFile.FileName, fileBytes, ContentType.Parse(modelFile.ContentType));
                }
            }

            emailMessageToYou.Body = bodyBuilderToYou.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.seznam.cz", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate("tiskni3dd@seznam.cz", "MARt_2009.6.10");
                    client.Send(emailMessageToYou);

                    // E-mail pro zákazníka
                    var emailMessageToCustomer = new MimeMessage();
                    emailMessageToCustomer.From.Add(new MailboxAddress("Tiskni3D", "tiskni3dd@seznam.cz"));
                    emailMessageToCustomer.To.Add(new MailboxAddress(name, email));
                    emailMessageToCustomer.Subject = "Potvrzení o pøijetí objednávky";

                    // HTML tìlo zprávy s obrázkem
                    var bodyBuilderToCustomer = new BodyBuilder
                    {
                        HtmlBody = $@"
                        <html>
                        <body>
                            <p>Dobrý den {name},</p>
                            <p>Dìkujeme za váš zájem o 3D tisk. Vaši objednávku jsme pøijali a co nejrychleji ji zpracujeme. Brzy vás budeme kontaktovat s dalšími informacemi.</p>
                            <p>S pozdravem,<br>Tým Tiskni3D</p>
                            <img src=""cid:logo"" alt=""Logo Tiskni3D"" style=""width:300px;"" />
                        </body>
                        </html>"
                    };

                    // Pøidání obrázku jako inline pøíloha
                    var logoImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Futuristic3D-ezgif.com-crop.jpg");

                    if (System.IO.File.Exists(logoImagePath))
                    {
                        var image = bodyBuilderToCustomer.LinkedResources.Add(logoImagePath);
                        image.ContentId = "logo";  // Tento ContentId odpovídá tomu, který je uvedený v HTML (cid:logo)
                    }

                    // Nastavení tìla zprávy zákazníkovi
                    emailMessageToCustomer.Body = bodyBuilderToCustomer.ToMessageBody();

                    // Odeslání e-mailu zákazníkovi
                    client.Send(emailMessageToCustomer);
                    _logger.LogInformation("E-mail zákazníkovi úspìšnì odeslán na {Email}", email);

                    client.Disconnect(true);
                }

                ViewBag.Message = "Dìkujeme za váš zájem, vaše poptávka k nám brzo dorazí.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba pøi odesílání e-mailu.");
                ViewBag.Message = "Omlouváme se, ale došlo k chybì pøi odesílání vaší poptávky. Zkuste to prosím pozdìji.";
            }

            return View("Index");
        }

        public IActionResult Potvrzeni()
        {
            return View();
        }
    }
}
