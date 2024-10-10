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

        // Akce pro zobrazen� domovsk� str�nky
        public IActionResult Index()
        {
            return View(); // Vr�t� Index.cshtml
        }

        // Akce pro zpracov�n� formul��e
        [HttpPost]
        public IActionResult OdeslatData(string name, string email, string message, IFormFile modelFile)
        {
            // E-mail pro tebe (p��jemce popt�vky)
            var emailMessageToYou = new MimeMessage();
            _logger.LogInformation("OdeslatData byla zavol�na");

            emailMessageToYou.From.Add(new MailboxAddress("Tiskni3D", "tiskni3dd@seznam.cz"));
            emailMessageToYou.To.Add(new MailboxAddress("Tiskni3D", "tiskni3dd@seznam.cz"));
            emailMessageToYou.Subject = "Nov� popt�vka na 3D tisk";

            var bodyBuilderToYou = new BodyBuilder
            {
                TextBody = $"Jm�no: {name}\nEmail z�kazn�ka: {email}\nZpr�va: {message}"
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

                    // E-mail pro z�kazn�ka
                    var emailMessageToCustomer = new MimeMessage();
                    emailMessageToCustomer.From.Add(new MailboxAddress("Tiskni3D", "tiskni3dd@seznam.cz"));
                    emailMessageToCustomer.To.Add(new MailboxAddress(name, email));
                    emailMessageToCustomer.Subject = "Potvrzen� o p�ijet� objedn�vky";

                    // HTML t�lo zpr�vy s obr�zkem
                    var bodyBuilderToCustomer = new BodyBuilder
                    {
                        HtmlBody = $@"
                        <html>
                        <body>
                            <p>Dobr� den {name},</p>
                            <p>D�kujeme za v� z�jem o 3D tisk. Va�i objedn�vku jsme p�ijali a co nejrychleji ji zpracujeme. Brzy v�s budeme kontaktovat s dal��mi informacemi.</p>
                            <p>S pozdravem,<br>T�m Tiskni3D</p>
                            <img src=""cid:logo"" alt=""Logo Tiskni3D"" style=""width:300px;"" />
                        </body>
                        </html>"
                    };

                    // P�id�n� obr�zku jako inline p��loha
                    var logoImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Futuristic3D-ezgif.com-crop.jpg");

                    if (System.IO.File.Exists(logoImagePath))
                    {
                        var image = bodyBuilderToCustomer.LinkedResources.Add(logoImagePath);
                        image.ContentId = "logo";  // Tento ContentId odpov�d� tomu, kter� je uveden� v HTML (cid:logo)
                    }

                    // Nastaven� t�la zpr�vy z�kazn�kovi
                    emailMessageToCustomer.Body = bodyBuilderToCustomer.ToMessageBody();

                    // Odesl�n� e-mailu z�kazn�kovi
                    client.Send(emailMessageToCustomer);
                    _logger.LogInformation("E-mail z�kazn�kovi �sp�n� odesl�n na {Email}", email);

                    client.Disconnect(true);
                }

                ViewBag.Message = "D�kujeme za v� z�jem, va�e popt�vka k n�m brzo doraz�.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba p�i odes�l�n� e-mailu.");
                ViewBag.Message = "Omlouv�me se, ale do�lo k chyb� p�i odes�l�n� va�� popt�vky. Zkuste to pros�m pozd�ji.";
            }

            return View("Index");
        }

        public IActionResult Potvrzeni()
        {
            return View();
        }
    }
}
