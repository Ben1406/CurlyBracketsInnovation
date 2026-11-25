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

            StringBuilder bodyText = new StringBuilder();

            // HTML Head
            bodyText.AppendLine(@"<!Doctype html>");
            bodyText.AppendLine(@"<html lang=""da"">");
            bodyText.AppendLine(@"<head>");
            bodyText.AppendLine(@"<meta charset=""UTF-8"">");
            bodyText.AppendLine(@"<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">");
            bodyText.AppendLine(@"<title>Ordrebekræftelse - Lions Club Vinderup</title>");
            bodyText.AppendLine(@"</head>");
            bodyText.AppendLine(@"<body>");
            bodyText.AppendLine(@"<div>");

            // HTML Body (begin)
            bodyText.AppendLine(@"<body style=""margin: 0; padding: 0; background-color: #f4f5f7; font-family: Arial, Helvetica, sans-serif; color: #333333; -webkit-font-smoothing: antialiased;"">");

            bodyText.AppendLine(@"<table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""background-color: #f4f5f7; padding: 30px 15px;"">");
            bodyText.AppendLine(@"<tr>");
            bodyText.AppendLine(@"<td align=""center"">");

            // Mail header
            bodyText.AppendLine(@"<table role=""presentation"" width=""720"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""max-width: 720px; width: 100%; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.06);"">");
            bodyText.AppendLine(@"<tr>");
            bodyText.AppendLine(@"<td style=""background-color: #00338d; padding: 28px 32px; text-align: left; border-bottom: 4px solid #ebb700;"">");
            bodyText.AppendLine(@"<h1 style=""margin: 0; font-size: 22px; color: #ffffff; letter-spacing: 0.5px;"">Lions Club Vinderup</h1>");
            bodyText.AppendLine($@"<p style=""margin: 6px 0 0 0; font-size: 16px; color: #ebb700; font-weight: bold;"">Ordrebekræftelse #{ordre.ordreid}</p>");
            bodyText.AppendLine(@"</td>");
            bodyText.AppendLine(@"</tr>");

            bodyText.AppendLine(@"<tr>");
            bodyText.AppendLine(@"<td style=""padding: 32px 32px 16px 32px;"">");
            bodyText.AppendLine($@"<p style=""margin: 0 0 16px 0; font-size: 16px; line-height: 1.5;"">Kære <strong>{ordre.navn}</strong>,</p>");
            bodyText.AppendLine(@"<p style=""margin: 0 0 16px 0; font-size: 16px; line-height: 1.5;"">Tak for din bestilling.</p>");
            bodyText.AppendLine(@"<p style=""margin: 0 0 16px 0; font-size: 16px; line-height: 1.5;"">");
            bodyText.AppendLine(@"Dit køb kommer også til at gøre gavn for andre. Hos Lions går overskuddet nemlig ubeskåret til velgørende og humanitære formål &ndash; både lokalt, nationalt og internationalt.");
            bodyText.AppendLine(@"</p>");
            bodyText.AppendLine(@"</td>");
            bodyText.AppendLine(@"</tr>");

            // Mail payment method
            bodyText.AppendLine(@"<tr>");
            bodyText.AppendLine(@"<td style=""padding: 12px 32px;"">");
            bodyText.AppendLine(@"<table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""background-color: #f0f4fa; border-left: 4px solid #00338d; border-radius: 4px;"">");
            bodyText.AppendLine(@"<tr>");
            bodyText.AppendLine(@"<td style=""padding: 18px 20px;"">");
            bodyText.AppendLine(@"<h2 style=""margin: 0 0 8px 0; font-size: 16px; color: #00338d;"">Betaling</h2>");
            bodyText.AppendLine(@"<p style=""margin: 0; font-size: 16px; line-height: 1.5;"">");
            if (ordre?.betalingsmetode == "mobilepay")
            {
                bodyText.AppendLine($@"Indbetal <strong>kr. {ordre.totalpris:#0.00}</strong> med MobilePay på nr. <strong>88345</strong> og skriv <strong>{ordre.ordreid}</strong> i beskedfeltet.");
            }
            if (ordre?.betalingsmetode == "bank")
            {
                bodyText.AppendLine($@"Overfør <strong>kr. {ordre.totalpris:#0.00}</strong> til bankkonto: <br/ >");
                bodyText.AppendLine($@"Reg.nr. <strong>9070</strong> <br/ >");
                bodyText.AppendLine($@"Kontonr. <strong>2021032452</strong> <br/ >");
                bodyText.AppendLine($@"Skriv <strong>{ordre.ordreid}</strong> i tekst-til-modtager feltet.");
            }
            bodyText.AppendLine(@"</p>");
            bodyText.AppendLine(@"</td>");
            bodyText.AppendLine(@"</tr>");
            bodyText.AppendLine(@"</table>");
            bodyText.AppendLine(@"</td>");
            bodyText.AppendLine(@"</tr>");

            // Mail delivery method
            bodyText.AppendLine(@"<tr>");
            bodyText.AppendLine(@"<td style=""padding: 16px 32px;"">");
            bodyText.AppendLine(@"<h2 style=""margin: 0 0 8px 0; font-size: 16px; color: #00338d;"">Levering &amp; Afhentning</h2>");
            bodyText.AppendLine(@"<p style=""margin: 0; font-size: 16px; line-height: 1.5;"">");
            if (ordre?.leveringsmetode == "henter-selv")
            {
                bodyText.AppendLine(@"Du kan hente din bestilling hos Lions Club Vinderup. <br/ >");
                bodyText.AppendLine(@"Vi kontakter dig og aftaler nærmere om tid og sted.");
            }
            else
            {
                bodyText.AppendLine(@"Vi levere periodevis, hvilket typisk betyder at du får din bestilling indenfor 14 dage.");
            }
            bodyText.AppendLine(@"</p>");
            bodyText.AppendLine(@"</td>");
            bodyText.AppendLine(@"</tr>");

            // Mail your order (begin)
            bodyText.AppendLine(@"<tr>");
            bodyText.AppendLine(@"<td style=""padding: 16px 32px 32px 32px;"">");
            bodyText.AppendLine(@"<h2 style=""margin: 0 0 12px 0; font-size: 16px; color: #00338d;"">Din bestilling</h2>");
            bodyText.AppendLine(@"<table role=""presentation"" width=""720"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""width: 720px; table-layout: fixed; border-collapse: collapse; font-size: 16px;"">");
            bodyText.AppendLine(@"<colgroup>");
            bodyText.AppendLine(@"<col style = ""width: 400px;"">");
            bodyText.AppendLine(@"<col style = ""width: 160px;"">");
            bodyText.AppendLine(@"<col style = ""width: 160px;"">");
            bodyText.AppendLine(@"</colgroup>");

            // Mail order table header
            bodyText.AppendLine(@"<thead>");
            bodyText.AppendLine(@"<tr style=""background-color: #f8f9fa; border-bottom: 2px solid #dee2e6;"">");
            bodyText.AppendLine(@"<th scope=""col"" width=""400"" style=""width: 400px; box-sizing: border-box; padding: 8px 6px; text-align: left; color: #333333; font-weight: bold;"">Produkt</th>");
            bodyText.AppendLine(@"<th scope=""col"" width=""160"" style=""width: 160px; box-sizing: border-box; padding: 8px 6px; text-align: right; color: #333333; font-weight: bold;"">Styk</th>");
            bodyText.AppendLine(@"<th scope=""col"" width=""160"" style=""width: 160px; box-sizing: border-box; padding: 8px 6px; text-align: right; color: #333333; font-weight: bold;"">Beløb</th>");
            bodyText.AppendLine(@"</tr>");
            bodyText.AppendLine(@"</thead>");

            // Mail order table body
            bodyText.AppendLine(@"<tbody>");

            var totalPris = 0;
            var totalAntal = 0;
            foreach (var ol in ordreLinjer)
            {
                bodyText.AppendLine(@"<tr style=""border-bottom: 1px solid #eeeeee;"">");
                bodyText.AppendLine($@"<td width=""400"" style=""width: 400px; box-sizing: border-box; padding: 8px 6px; text-align: left; color: #333333;"">{ol.produktbeskrivelse}</td>");
                bodyText.AppendLine($@"<td width=""160"" style=""width: 160px; box-sizing: border-box; padding: 8px 6px; text-align: right; color: #333333;"">{ol.antal}</td>");
                bodyText.AppendLine($@"<td width=""160"" style=""width: 160px; box-sizing: border-box; padding: 8px 6px; text-align: right; color: #333333;"">{ol.totalpris:#0.00} kr.</td>");
                bodyText.AppendLine(@"</tr>");
                totalPris += ol.totalpris;
                totalAntal += ol.antal;
            }

            bodyText.AppendLine(@"</tbody>");

            // Mail order table footer
            bodyText.AppendLine(@"<tfoot>");
            bodyText.AppendLine(@"<tr style=""background-color: #f8f9fa; border-top: 2px solid #00338d;"">");
            bodyText.AppendLine(@"<td width=""400"" style=""width: 400px; box-sizing: border-box; padding: 8px 6px; text-align: left; font-weight: bold; color: #00338d;"">Total</td>");
            bodyText.AppendLine($@"<td width=""160"" style=""width: 160px; box-sizing: border-box; padding: 8px 6px; text-align: right; font-weight: bold; color: #00338d;"">{totalAntal}</td>");
            bodyText.AppendLine($@"<td width=""160"" style=""width: 160px; box-sizing: border-box; padding: 8px 6px; text-align: right; font-weight: bold; color: #00338d;"">{totalPris:#0.00} kr.</td>");
            bodyText.AppendLine(@"</tr>");
            bodyText.AppendLine(@"</tfoot>");

            // Mail your order (end)
            bodyText.AppendLine(@"</table>");
            bodyText.AppendLine(@"</td>");
            bodyText.AppendLine(@"</tr>");

            // Mail kind regards and contacts
            bodyText.AppendLine(@"<tr>");
            bodyText.AppendLine(@"<td style=""background -color: #f8f9fa; padding: 24px 32px; border-top: 1px solid #eeeeee; font-size: 16px; line-height: 1.5;"">");
            bodyText.AppendLine(@"<p style=""margin: 0 0 8px 0;"">Med venlig hilsen,<br><strong>Lions Club Vinderup</strong></p>");
            bodyText.AppendLine(@"<p style=""margin: 0;"">");
            bodyText.AppendLine(@"Mail: <a href=""mailto:mail@lcvinderup.dk"" style=""color: #00338d; text-decoration: none;"">mail@lcvinderup.dk</a> &nbsp;|&nbsp;");
            bodyText.AppendLine(@"Mobil: <a href=""tel:+4549490304"" style=""color: #00338d; text-decoration: none;"">49 49 03 04</a>");
            bodyText.AppendLine(@"</p>");
            bodyText.AppendLine(@"</td>");
            bodyText.AppendLine(@"</tr>");

            // HTML Body (end)
            bodyText.AppendLine(@"</table>");
            bodyText.AppendLine(@"</td>");
            bodyText.AppendLine(@"</tr>");
            bodyText.AppendLine(@"</table>");

            var message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress("Lions Club Vinderup", "mail@lcvinderup.dk"));
            message.To.Add(new MimeKit.MailboxAddress($"{ordre?.navn}", $"{ordre?.mail}"));
            message.Subject = $"Bestilling - Ordrenr. {sendMail.ordreid}";
            message.Body = new MimeKit.BodyBuilder
            {
                HtmlBody = bodyText.ToString()
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

            Log.Write($"Mail sendt til ...");
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
