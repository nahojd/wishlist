using MailKit.Net.Smtp;
using MimeKit;
using WishList.Api.Model;

namespace WishList.Api.Services;

public interface IMessageService
{
	Task NotifyWishChanged(Wish wish);
	Task NotifyWishDeleted(Wish wish);
	Task SendForgotPasswordMessage(string name, string email, string token);
	Task SendNewRegistrationMessage(string v1, string v2, string? message);
}

public class MessageService(IConfiguration config, ILogger<MessageService> logger) : IMessageService
{
	public async Task NotifyWishChanged(Wish wish)
	{
		if (wish?.TjingadBy is null || wish.Owner is null)
			return;

		var subject = $"Önskningen {wish.Name} har ändrats";
		var textBody = @$"Hej, {wish.TjingadBy.Name}!

Detta är ett automatiskt meddelande från Önskelistemaskinen. {wish.Owner.Name} har ändrat önskningen {wish.Name} i sin önskelista (som du har tjingat).
Klicka på nedanstående länk för att kontrollera att inget allvarligt ändrats.

http://wish.driessen.se";

		await SendEmail(wish.TjingadBy.Name!, wish.TjingadBy.Email!, subject, textBody);
	}

	public async Task NotifyWishDeleted(Wish wish)
	{
		if (wish?.TjingadBy is null || wish.Owner is null)
			return;

		var subject = $"Önskningen {wish.Name} togs bort";
		var textBody = @$"Hej, {wish.TjingadBy.Name}!

Detta är ett automatiskt meddelande från Önskelistemaskinen. {wish.Owner.Name} har tagit bort önskningen {wish.Name} i sin önskelista (som du har tjingat).
Du kanske får hitta en annan önskning på önskelistan.

http://wish.driessen.se";

		await SendEmail(wish.TjingadBy.Name!, wish.TjingadBy.Email!, subject, textBody);
	}

	public async Task SendForgotPasswordMessage(string name, string email, string token)
	{
		var resetUrl = $"{config["WebUrl"]}/resetpassword?token={token}";
		var textBody = @$"Hej!

En begäran om att återställa ditt lösenord på Önskelistemaskinen har gjorts.

För att återställa lösenordet, gå till {resetUrl}

Om du inte begärt en återställning av lösenordet, bortse från detta mail.";

		// Console.WriteLine(textBody);

		await SendEmail(name, email, "Begäran om att återställa lösenord", textBody);
	}

	public async Task SendNewRegistrationMessage(string name, string email, string? message)
	{
		var subject = "[WishList] Ny användarregistrering";
		var textBody = @$"
Namn: {name}
Epost: {email}
Meddelande:
{message ?? "Lämnade inget meddelande"}";


		await SendAdminEmail(subject, textBody);
	}

	private async Task SendAdminEmail(string subject, string textBody)
	{
		var name = config["Admin:Name"];
		var email = config["Admin:Email"];

		if (name is not null && email is not null)
			await SendEmail(name, email, subject, textBody);
	}

	//TODO: Detta kanske borde skötas out-of-process, t.ex. med Hangfire
	private async Task SendEmail(string toName, string toEmail, string subject, string textBody)
	{
		if (!string.IsNullOrWhiteSpace(config["Mail:Host"]))
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("Önskelistemaskinen", "no-reply@wish.driessen.se"));
			message.To.Add(new MailboxAddress(toName, toEmail));
			message.Subject = subject;
			message.Body = new BodyBuilder { TextBody = textBody }.ToMessageBody();

			using var client = new SmtpClient();

			try
			{
				await client.ConnectAsync(config["Mail:Host"], config.GetValue("Mail:Port", 25), MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
				await client.SendAsync(message);
				await client.DisconnectAsync(true);
				logger.LogInformation("Skickade epost {subject} till {email}", subject, toEmail);
				if (logger.IsEnabled(LogLevel.Debug))
					logger.LogDebug("Email body:\n{body}", textBody);
			}
			catch (Exception ex)
			{
				//TODO: Om felet är SmtpCommandException med StatusCode MailboxBusy borde vi försöka igenom 5 minuter.
				logger.LogError(ex, "Det gick inte att skicka mail till {email}", toEmail);
			}
		}
		else
		{
			logger.LogInformation("Skickade INTE epost {subject} till {email}\n{body}", subject, toEmail, textBody);
		}
	}
}