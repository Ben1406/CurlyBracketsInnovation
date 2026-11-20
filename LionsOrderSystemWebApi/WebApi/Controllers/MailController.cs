using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Runtime.InteropServices;
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
    public async Task<ActionResult<bool>> SendOrderMail(int ordreid)
    {
        try
        {
            var ordre = _dbContext.Ordrer.FirstOrDefault(o => o.ordreid == ordreid);
            var ordreLinjer = _dbContext.OrdreLinjer.Where(o => o.ordreid == ordreid).OrderBy(o => o.ordrelinjeid).ToList();
            if (ordre is null || ordreLinjer is null)
            {
                return NotFound($"Fandt ingen ordre/ordrelinjer på ordrenr. {ordreid}");
            }

            string styleBorder = $@"border-style: outset; border-width: 1px; border-color: black";
            string styleNormal = $@"style=""font-size: 16px; text-align: left; line-height: 18px""";
            string styleClose = $@"style=""font-size: 16px; text-align: left; line-height: 2px""";
            string stylePriRow = $@"style=""font-size: 16px; text-align: left; font-weight: normal; width: 350px""";
            string stylePriBorderRow = $@"style=""font-size: 16px; text-align: left; font-weight: normal; width: 350px; {styleBorder}""";
            string styleSecRow = $@"style=""font-size: 16px; text-align: right; font-weight: normal; width: 150px""";
            string styleSecBorderRow = $@"style=""font-size: 16px; text-align: right; font-weight: normal; width: 150px; {styleBorder}""";

            StringBuilder bodyText = new StringBuilder();

            bodyText.AppendLine(@"<!Doctype html>");
            bodyText.AppendLine(@"<html lang=""en"">");
            bodyText.AppendLine(@"<head>");
            bodyText.AppendLine(@"<meta charset=""UTF-8"">");
            bodyText.AppendLine(@"</head>");
            bodyText.AppendLine(@"<body>");
            bodyText.AppendLine(@"<div>");

            bodyText.AppendLine($"<p {styleNormal}>Kære {ordre?.navn}</p>");
            bodyText.AppendLine($"<p {styleNormal}>Tak for din bestilling.</p>");
            bodyText.AppendLine($"<p {styleClose}>Dit køb kommer også til at gøre gavn for andre.</p>");
            bodyText.AppendLine($"<p {styleClose}>For hos Lions går overskuddet nemlig ubeskåret til velgørende og humanitære formål.</p>");
            bodyText.AppendLine($"<p {styleClose}>Både lokalt, nationalt og internationalt.</p>");

            bodyText.AppendLine($"<p {styleNormal}><u>Betaling</u></p>");
            if (ordre?.betalingsmetode == "mobilepay") bodyText.AppendLine($"<p {styleClose}>Indbetal <b>kr. {ordre.totalpris}</b> med MobilePay på nr. 88345 og skriv ' {ordre.ordreid} ' i besked-feltet.</p>");
            if (ordre?.betalingsmetode == "bank")
            {
                bodyText.AppendLine($"<p {styleClose}>Overfør <b>kr. {ordre.totalpris:#0.00}</b> til bankkonto:</span>");
                bodyText.AppendLine($"<p {styleClose}>Reg.nr. 9070</span>");
                bodyText.AppendLine($"<p {styleClose}>Kontonr. 2021032452</span>");
                bodyText.AppendLine($"<p {styleClose}>Skriv ' {ordre.ordreid} ' i tekst-til-modtager-feltet.</span>");
            };

            bodyText.AppendLine($"<p {styleNormal}><u>Levering</u></p>");
            if (ordre?.leveringsmetode == "henter-selv")
            {
                bodyText.AppendLine($"<p {styleClose}>Du kan hente din bestilling hos Lions Club Vinderup.</p>");
                bodyText.AppendLine($"<p {styleClose}>Vi kontakter dig og aftaler tid og sted.</p>");
            }
            else
            {
                bodyText.AppendLine($"<p {styleClose}>Vi levere periodevis, hvilket typisk betyder at du får din bestilling indenfor 14 dage.<p>");
            }

            bodyText.AppendLine($"<p {styleNormal}><u>Bestilling</u></p>");

            bodyText.AppendLine($@"<table style=""width: 600px; {styleBorder}"">");
            bodyText.AppendLine("<thead>");
            bodyText.AppendLine("<tr>");
            bodyText.AppendLine($@"<th scope""col"" {stylePriBorderRow}>Produkt</th>");
            bodyText.AppendLine($@"<th scope""col"" {styleSecBorderRow}>Styk</th>");
            bodyText.AppendLine($@"<th scope""col"" {styleSecBorderRow}>Beløb</th>");
            bodyText.AppendLine("</tr>");
            bodyText.AppendLine("</thead>");
            bodyText.AppendLine("<tbody>");

            var totalPris = 0;
            var totalAntal = 0;

            foreach (var ol in ordreLinjer)
            {
                bodyText.AppendLine("<tr>");
                bodyText.AppendLine($@"<th scope""row"" {stylePriRow}>{ol.produktbeskrivelse}</th>");
                bodyText.AppendLine($@"<td scope""row"" {styleSecRow}>{ol.antal}</th>");
                bodyText.AppendLine($@"<td scope""row"" {styleSecRow}>{ol.totalpris:#0.00} kr.</th>");
                bodyText.AppendLine("</tr>");
                totalPris += ol.totalpris;
                totalAntal += ol.antal;
            }

            bodyText.AppendLine("</tbody>");
            bodyText.AppendLine("<tfoot>");
            bodyText.AppendLine("<tr>");
            bodyText.AppendLine($@"<th scope""row"" {stylePriBorderRow}>Total</th>");
            bodyText.AppendLine($@"<td scope""row"" {styleSecBorderRow}>{totalAntal}</th>");
            bodyText.AppendLine($@"<td scope""row"" {styleSecBorderRow}>{totalPris:#0.00} kr.</th>");
            bodyText.AppendLine("</tr>");
            bodyText.AppendLine("</tfoot>");
            bodyText.AppendLine("</table>");

            bodyText.AppendLine($"<p {styleNormal}>Med venlig hilsen</p>");
            bodyText.AppendLine($"<p {styleClose}>Lions Club Vinderup</p>");
            bodyText.AppendLine($"<p {styleClose}>Mail: mail@lcvinderup.dk</p>");
            bodyText.AppendLine($"<p {styleClose}>Mobil: 49490304</p>");

            bodyText.AppendLine("<div>");
            bodyText.AppendLine("</body>");
            bodyText.AppendLine("</html>");

            var message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress("Lions Club Vinderup", "mail@lcvinderup.dk"));
            message.To.Add(new MimeKit.MailboxAddress($"{ordre?.navn}", $"{ordre?.mail}"));
            message.Subject = $"Bestilling - Ordrenr. {ordreid}";
            message.Body = new MimeKit.BodyBuilder
            {
                HtmlBody = bodyText.ToString()
            }.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.simply.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
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
