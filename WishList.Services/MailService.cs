using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace WishList.Services
{
	public class MailService : IMailService
	{
		public void SendMail( MailMessage mail )
		{
			using (var client = new SmtpClient())
			{
				client.Send( mail );
			}
		}
	}
}
