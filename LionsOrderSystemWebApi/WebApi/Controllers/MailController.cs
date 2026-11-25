using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WebApi.Data;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MailController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public MailController()
    {
        _dbContext = new AppDbContext(new DbContextOptions<AppDbContext>());
    }

    [HttpPost("SendOrderMail")]
    public async Task<ActionResult<bool>> SendOrderMail(SendMailRequest sendMail)
    {
        try
        {
            var ordre = _dbContext.Ordrer.FirstOrDefault(o => o.ordreid == sendMail.ordreid);
            var ordreLinjer = _dbContext.OrdreLinjer.Where(o => o.ordreid == sendMail.ordreid).OrderBy(o => o.ordrelinjeid).ToList();
            if (ordre is null || ordreLinjer is null)
            {
                return NotFound($"Fandt ingen ordre/ordrelinjer på ordrenr. {sendMail.ordreid}");
            }

            var bodyHTML = new StringBuilder();

            // HTML Head
            bodyHTML.AppendLine(@"<!Doctype html>");
            bodyHTML.AppendLine(@"<html lang=""da"">");
            bodyHTML.AppendLine(@"<head>");
            bodyHTML.AppendLine(@"<meta charset=""UTF-8"">");
            bodyHTML.AppendLine(@"<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">");
            bodyHTML.AppendLine(@"<title>Ordrebekræftelse - Lions Club Vinderup</title>");
            bodyHTML.AppendLine(@"</head>");

            // HTML Body (begin)
            bodyHTML.AppendLine(@"<body style=""margin: 0; padding: 0; background-color: #f4f5f7; font-family: Arial, Helvetica, sans-serif; color: #333333; -webkit-font-smoothing: antialiased;"">");

            bodyHTML.AppendLine(@"<table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""background-color: #f4f5f7; padding: 30px 15px;"">");
            bodyHTML.AppendLine(@"<tr>");
            bodyHTML.AppendLine(@"<td align=""center"">");

            // Mail header
            bodyHTML.AppendLine(@"<table role=""presentation"" width=""640"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""max-width: 640px; width: 100%; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.06);"">");
            bodyHTML.AppendLine(@"<tr>");
            bodyHTML.AppendLine(@"<td style=""background-color: #00338d; padding: 28px 32px; text-align: left; border-bottom: 4px solid #ebb700;"">");
            bodyHTML.AppendLine(@"<h1 style=""margin: 0; font-size: 22px; color: #ffffff; letter-spacing: 0.5px;"">Lions Club Vinderup</h1>");
            bodyHTML.AppendLine(@"<p style=""margin: 6px 0 0 0; font-size: 16px; color: #ebb700; font-weight: bold;"">");
            bodyHTML.AppendLine($@"Ordrebekræftelse #{ordre.ordreid}");
            bodyHTML.AppendLine(@"</p>");
            bodyHTML.AppendLine(@"</td>");
            bodyHTML.AppendLine(@"</tr>");

            bodyHTML.AppendLine(@"<tr>");
            bodyHTML.AppendLine(@"<td style=""padding: 32px 32px 16px 32px;"">");
            bodyHTML.AppendLine($@"<p style=""margin: 0 0 16px 0; font-size: 16px; line-height: 1.5;"">Kære <strong>{ordre.navn}</strong>,</p>");
            bodyHTML.AppendLine(@"<p style=""margin: 0 0 16px 0; font-size: 16px; line-height: 1.5;"">Tak for din bestilling.</p>");
            bodyHTML.AppendLine(@"<p style=""margin: 0 0 16px 0; font-size: 16px; line-height: 1.5;"">");
            bodyHTML.AppendLine(@"Dit køb kommer også til at gøre gavn for andre. Hos Lions går overskuddet nemlig ubeskåret til velgørende og humanitære formål &ndash; både lokalt, nationalt og internationalt.");
            bodyHTML.AppendLine(@"</p>");
            bodyHTML.AppendLine(@"</td>");
            bodyHTML.AppendLine(@"</tr>");

            // Mail payment method
            bodyHTML.AppendLine(@"<tr>");
            bodyHTML.AppendLine(@"<td style=""padding: 12px 32px;"">");
            bodyHTML.AppendLine(@"<table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""background-color: #f0f4fa; border-left: 4px solid #00338d; border-radius: 4px;"">");
            bodyHTML.AppendLine(@"<tr>");
            bodyHTML.AppendLine(@"<td style=""padding: 18px 20px;"">");
            bodyHTML.AppendLine(@"<h2 style=""margin: 0 0 8px 0; font-size: 16px; color: #00338d;"">Betaling</h2>");
            bodyHTML.AppendLine(@"<p style=""margin: 0; font-size: 16px; line-height: 1.5;"">");
            if (ordre?.betalingsmetode == "mobilepay")
            {
                bodyHTML.AppendLine($@"Indbetal <strong>kr. {ordre.totalpris:#0.00}</strong> med MobilePay på nr. <strong>88345</strong> og skriv <strong>{ordre.ordreid}</strong> i beskedfeltet.");
            }
            if (ordre?.betalingsmetode == "bank")
            {
                bodyHTML.AppendLine($@"Overfør <strong>kr. {ordre.totalpris:#0.00}</strong> til bankkonto:<br>");
                bodyHTML.AppendLine($@"Reg.nr. <strong>9070</strong><br>");
                bodyHTML.AppendLine($@"Kontonr. <strong>2021032452</strong><br>");
                bodyHTML.AppendLine($@"Skriv <strong>{ordre.ordreid}</strong> i tekst-til-modtager feltet.");
            }
            bodyHTML.AppendLine(@"</p>");
            bodyHTML.AppendLine(@"</td>");
            bodyHTML.AppendLine(@"</tr>");
            bodyHTML.AppendLine(@"</table>");
            bodyHTML.AppendLine(@"</td>");
            bodyHTML.AppendLine(@"</tr>");

            // Mail delivery method
            bodyHTML.AppendLine(@"<tr>");
            bodyHTML.AppendLine(@"<td style=""padding: 16px 32px;"">");
            bodyHTML.AppendLine(@"<h2 style=""margin: 0 0 8px 0; font-size: 16px; color: #00338d;"">Levering &amp; Afhentning</h2>");
            bodyHTML.AppendLine(@"<p style=""margin: 0; font-size: 16px; line-height: 1.5;"">");

            var deliveryText = "";
            if (ordre?.leveringsmetode == "henter-selv") deliveryText = "Afhentes - Jeg henter selv hos Lions i Vinderup";
            if (ordre?.leveringsmetode == "stil-ved-fordøren") deliveryText = "Leveres - Stilles ved min fordør";
            if (ordre?.leveringsmetode == "stil-ved-bagdøren") deliveryText = "Leveres - Stilles ved min bagdør";
            if (ordre?.leveringsmetode == "stil-i-indkørsel") deliveryText = "Leveres - Stilles i min indkørsel";
            if (ordre?.leveringsmetode == "stil-i-carport") deliveryText = "Leveres - Stilles i min carport/garage";
            if (ordre?.leveringsmetode == "stil-under-halvtag") deliveryText = "Leveres - Stilles under mit halvtag";
            if (ordre?.leveringsmetode == "stil--på-terrasse") deliveryText = "Leveres - Stilles på min terrasse";
            if (ordre?.leveringsmetode == "stil-i-trillebør") deliveryText = "Leveres - Stilles i min trillebør";
            if (ordre?.leveringsmetode == "stil-i-skur") deliveryText = "Leveres - Stilles i mit skur";
            if (ordre?.leveringsmetode == "læs-bemærkning") deliveryText = "Har skrevet det i bemærkning";
            bodyHTML.AppendLine($@"Du valgte følgende leveringsmetode: {deliveryText} <br><br>");

            if (ordre?.leveringsmetode == "henter-selv")
            {
                bodyHTML.AppendLine(@"Du skal hente din bestilling hos Lions Club Vinderup.<br>");
                bodyHTML.AppendLine(@"Vi kontakter dig og aftaler nærmere om tid og sted.");
            }
            else
            {
                bodyHTML.AppendLine(@"Vi levere periodevis, hvilket typisk betyder at du får din bestilling indenfor 14 dage.");
            }
            bodyHTML.AppendLine(@"</p>");
            bodyHTML.AppendLine(@"</td>");
            bodyHTML.AppendLine(@"</tr>");

            // Mail note
            if (!string.IsNullOrEmpty(ordre?.note))
            {
                bodyHTML.AppendLine(@"<tr>");
                bodyHTML.AppendLine(@"<td style=""padding: 16px 32px;"">");
                bodyHTML.AppendLine(@"<h2 style=""margin: 0 0 8px 0; font-size: 16px; color: #00338d;"">Din bemærkning på ordren</h2>");
                bodyHTML.AppendLine(@"<p style=""margin: 0; font-size: 16px; line-height: 1.5;"">");
                bodyHTML.AppendLine($@"{ordre?.note}");
                bodyHTML.AppendLine(@"</p>");
                bodyHTML.AppendLine(@"</td>");
                bodyHTML.AppendLine(@"</tr>");
            }

            // Mail your order (begin)
            bodyHTML.AppendLine(@"<tr>");
            bodyHTML.AppendLine(@"<td style=""padding: 16px 32px 32px 32px;"">");
            bodyHTML.AppendLine(@"<h2 style=""margin: 0 0 12px 0; font-size: 16px; color: #00338d;"">Din bestilling</h2>");
            bodyHTML.AppendLine(@"<table role=""presentation"" width=""640"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""width: 640px; table-layout: fixed; border-collapse: collapse; font-size: 16px;"">");
            bodyHTML.AppendLine(@"<colgroup>");
            bodyHTML.AppendLine(@"<col style = ""width: 340px;"">");
            bodyHTML.AppendLine(@"<col style = ""width: 150px;"">");
            bodyHTML.AppendLine(@"<col style = ""width: 150px;"">");
            bodyHTML.AppendLine(@"</colgroup>");

            // Mail order table header
            bodyHTML.AppendLine(@"<thead>");
            bodyHTML.AppendLine(@"<tr style=""background-color: #f8f9fa; border-bottom: 2px solid #dee2e6;"">");
            bodyHTML.AppendLine(@"<th scope=""col"" width=""340"" style=""width: 340px; box-sizing: border-box; padding: 8px 6px; text-align: left; color: #333333; font-weight: bold;"">Produkt</th>");
            bodyHTML.AppendLine(@"<th scope=""col"" width=""150"" style=""width: 150px; box-sizing: border-box; padding: 8px 6px; text-align: right; color: #333333; font-weight: bold;"">Styk</th>");
            bodyHTML.AppendLine(@"<th scope=""col"" width=""150"" style=""width: 150px; box-sizing: border-box; padding: 8px 6px; text-align: right; color: #333333; font-weight: bold;"">Beløb</th>");
            bodyHTML.AppendLine(@"</tr>");
            bodyHTML.AppendLine(@"</thead>");

            // Mail order table body
            bodyHTML.AppendLine(@"<tbody>");

            var totalPris = 0;
            var totalAntal = 0;
            foreach (var ol in ordreLinjer)
            {
                bodyHTML.AppendLine(@"<tr style=""border-bottom: 1px solid #eeeeee;"">");
                bodyHTML.AppendLine($@"<td width=""340"" style=""width: 340px; box-sizing: border-box; padding: 8px 6px; text-align: left; color: #333333;"">{ol.produktbeskrivelse}</td>");
                bodyHTML.AppendLine($@"<td width=""150"" style=""width: 150px; box-sizing: border-box; padding: 8px 6px; text-align: right; color: #333333;"">{ol.antal}</td>");
                bodyHTML.AppendLine($@"<td width=""150"" style=""width: 150px; box-sizing: border-box; padding: 8px 6px; text-align: right; color: #333333;"">{ol.totalpris:#0.00} kr.</td>");
                bodyHTML.AppendLine(@"</tr>");
                totalPris += ol.totalpris;
                totalAntal += ol.antal;
            }

            bodyHTML.AppendLine(@"</tbody>");

            // Mail order table footer
            bodyHTML.AppendLine(@"<tfoot>");
            bodyHTML.AppendLine(@"<tr style=""background-color: #f8f9fa; border-top: 2px solid #00338d;"">");
            bodyHTML.AppendLine(@"<td width=""340"" style=""width: 340px; box-sizing: border-box; padding: 8px 6px; text-align: left; font-weight: bold; color: #00338d;"">Total</td>");
            bodyHTML.AppendLine($@"<td width=""150"" style=""width: 150px; box-sizing: border-box; padding: 8px 6px; text-align: right; font-weight: bold; color: #00338d;"">{totalAntal}</td>");
            bodyHTML.AppendLine($@"<td width=""150"" style=""width: 150px; box-sizing: border-box; padding: 8px 6px; text-align: right; font-weight: bold; color: #00338d;"">{totalPris:#0.00} kr.</td>");
            bodyHTML.AppendLine(@"</tr>");
            bodyHTML.AppendLine(@"</tfoot>");

            // Mail your order (end)
            bodyHTML.AppendLine(@"</table>");
            bodyHTML.AppendLine(@"</td>");
            bodyHTML.AppendLine(@"</tr>");

            bodyHTML.AppendLine(@"<tr>");
            bodyHTML.AppendLine(@"<td style=""padding: 16px 32px;"">");
            bodyHTML.AppendLine(@"<p style=""margin: 0; font-size: 16px; line-height: 1.5;"">");
            bodyHTML.AppendLine(@"Der er ikke pålagt moms, da foreningen Lions Club Vinderup er fritaget for moms.");
            bodyHTML.AppendLine(@"</p>");
            bodyHTML.AppendLine(@"</td>");
            bodyHTML.AppendLine(@"</tr>");

            // Mail kind regards and contacts
            bodyHTML.AppendLine(@"<tr>");
            bodyHTML.AppendLine(@"<td style=""background-color: #f8f9fa; padding: 24px 32px; border-top: 1px solid #eeeeee; font-size: 16px; line-height: 1.5;"">");
            bodyHTML.AppendLine(@"<p style=""margin: 0 0 8px 0;"">Med venlig hilsen,<br><strong>Lions Club Vinderup</strong></p>");
            bodyHTML.AppendLine(@"<p style=""margin: 0;"">");
            bodyHTML.AppendLine(@"Mail: <a href=""mailto:mail@lcvinderup.dk"" style=""color: #00338d; text-decoration: none;"">mail@lcvinderup.dk</a> &nbsp;|&nbsp;");
            bodyHTML.AppendLine(@"Mobil: <a href=""tel:+4549490304"" style=""color: #00338d; text-decoration: none;"">49 49 03 04</a>");
            bodyHTML.AppendLine(@"</p>");
            bodyHTML.AppendLine(@"</td>");
            bodyHTML.AppendLine(@"</tr>");

            // HTML Body (end)
            bodyHTML.AppendLine(@"</table>");
            bodyHTML.AppendLine(@"</td>");
            bodyHTML.AppendLine(@"</tr>");
            bodyHTML.AppendLine(@"</table>");
            bodyHTML.AppendLine(@"</body>");

            var message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress("Lions Club Vinderup", "mail@lcvinderup.dk"));
            message.To.Add(new MimeKit.MailboxAddress($"{ordre?.navn}", $"{ordre?.mail}"));
            message.Subject = $"Bestilling - Ordrenr. {sendMail.ordreid}";
            message.Body = new MimeKit.BodyBuilder
            {
                HtmlBody = bodyHTML.ToString()
            }.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                var smtpServer = "websmtp.simply.com";

                #if DEBUG
                    smtpServer = "smtp.simply.com";
                #endif

                await client.ConnectAsync(smtpServer, 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("mail@lcvinderup.dk", "LCV7830#!");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            Log.Write($"Mail sendt til {ordre?.mail} ({ordre?.navn})");
            return true;
        }
        catch (Exception ex)
        {
            Log.Write($"Fejlede under SendOrderMail: {ex}");
            return NotFound("Fejlede under SendOrderMail");
        }
    }
}

public class SendMailRequest
{
    [Key]
    public required int ordreid { get; set; }
}
