﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WishList.Services;

namespace WishList.Tests
{
	class TestMailService : IMailService
	{
		public void SendMail( System.Net.Mail.MailMessage mail )
		{
			//Do nothing
		}
	}
}
