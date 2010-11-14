using System;
namespace WishList.Services
{
	public interface IMailService
	{
		void SendMail( System.Net.Mail.MailMessage mail );
	}
}
