using System;
using System.Web.Mvc;
using WishList.Data.DataAccess;
using WishList.Services;
using WishList.Data;
using System.Web.Security;
using System.Net.Mail;
using System.Configuration;
using System.Diagnostics;
using WishList.WebUI.ModelBinders;
using System.Security.Principal;

namespace WishList.WebUI.Controllers
{
	public partial class AccountController : Controller
	{
		public class UserData
		{
			public string Error { get; set; }
			public string Info { get; set; }
			public User User { get; set; }

			public UserData()
			{
				User = new User();
			}

		}

		public class SaveAccountResult
		{
			public bool Success { get; set; }
			public string StatusMessage { get; set; }
		}

		private IWishListRepository _repository;
		private UserService _service;

		public AccountController()
			: this( new SqlWishListRepository() )
		{
		}

		public AccountController( IWishListRepository repository )
		{
			_repository = repository;
		}

		private UserService Service
		{
			get
			{
				if (_service == null)
				{
					_service = new UserService( _repository );
				}
				return _service;
			}
		}

		#region Controller actions

		public virtual ActionResult Index()
		{
			return RedirectToAction( "Login" );
		}

		public virtual ActionResult Login()
		{
			UserData data = new UserData();

			if (TempData["loginmessage"] != null)
				data.Error = TempData["loginmessage"].ToString();

			return View( "Login", data );
		}

		[AcceptVerbs( "GET" )]
		public virtual ActionResult Authenticate()
		{
			return View( "Login" );
		}

		[AcceptVerbs( "POST" )]
		public virtual ActionResult Authenticate( string username, string password, string returnUrl )
		{

			bool success = Service.ValidateUser( username, password );

			if (success)
			{
				PerformFormsAuthentication( username );
				if (returnUrl != null)
				{
					return Redirect( returnUrl );
				}

				return RedirectToAction( "Index", "Home" );
			}

			TempData["loginmessage"] = "Felaktigt användarnamn eller lösenord!";
			return RedirectToAction( "Login" );
		}

		public virtual ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			return RedirectToAction( "Index", "Home" );
		}

		[AcceptVerbs( "GET" )]
		public virtual ActionResult Create()
		{
			return View( new SaveAccountResult() );
		}

		[AcceptVerbs( "POST" )]
		public virtual ActionResult Create( User user, string password, string message )
		{
			SaveAccountResult result = new SaveAccountResult { StatusMessage = "Ditt konto kunde inte skapas" };

			user.SetPassword( password );
			User createdUser = Service.CreateUser( user );
			if (createdUser != null)
			{
				result.Success = true;
				result.StatusMessage = "Ditt konto är skapat men är ännu inte aktiverat. Detta sker inom kort.";
				SendRequestForApproval( createdUser, Service.GetApprovalTicket( user.Name ).Value, message );
			}

			return View( "Create", result );
		}

		[AcceptVerbs( "GET" )]
		public virtual ActionResult Edit( [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentUser )
		{
			User user = Service.GetUser( currentUser.Identity.Name );

			return View( new UserData { User = user } );
		}

		[AcceptVerbs( "POST" )]
		public virtual ActionResult Edit( User editedUser, [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentUser )
		{
			//TODO: Validation checks

			User user = Service.GetUser( currentUser.Identity.Name );

			user.Email = editedUser.Email;
			user.NotifyOnChange = editedUser.NotifyOnChange;

			UserData data = new UserData { User = user };
			try
			{
				data.User = Service.UpdateUser( user );
				data.Info = "Ändringen sparad";
			}
			catch
			{
				data.Error = "Kunde inte spara dina ändringar.";
			}

			return View( data );
		}

		public virtual ActionResult ChangePassword( string password, [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentUser )
		{
			UserData data = new UserData();
			User user = Service.GetUser( currentUser.Identity.Name );
			data.User = user;

			if (!string.IsNullOrEmpty( password ))
			{
				Service.UpdatePassword( user.Name, password );
				data.Info = "Ditt lösenord är uppdaterat!";
			}
			else
			{
				data.Error = "Du kan inte ha ett tomt lösenord!";
			}

			return View( "Edit", data );
		}

		public virtual ActionResult Approve( string username, string ticket )
		{
			Guid guid;
			try
			{
				guid = new Guid( ticket );
			}
			catch
			{
				return View( "ApproveOrDeclineStatus", null, "Wrong ticket" );
			}

			try
			{
				Service.ApproveUser( username, guid );
			}
			catch (InvalidOperationException ex)
			{
				return View( "ApproveOrDeclineStatus", null, ex.Message );
			}

			SendApprovalConfirmation( username );

			return View( "ApproveOrDeclineStatus", null, "User approved" );
		}

		public virtual ActionResult Decline( string username, string ticket )
		{
			throw new NotImplementedException( "Decline är inte implementerat ännu - gör manuellt" );
		}

		#endregion

		#region Helper methods

		protected virtual void PerformFormsAuthentication( string userName )
		{
			FormsAuthentication.SetAuthCookie( userName, false );
		}

		private void SendRequestForApproval( User user, Guid ticket, string message )
		{
			try
			{
				string body = string.Format( "Snälla, godkänn mitt konto!\n\n----\n{0}\n----\n\nGodkänn: {1}/Account/Approve/{2}/{3}\nNeka: {4}/Account/Decline/{5}/{6}",
					message ?? "Lämnade inget meddelande",
					ConfigurationManager.AppSettings["ApplicationUrl"],
					user.Name,
					ticket,
					ConfigurationManager.AppSettings["ApplicationUrl"],
					user.Name,
					ticket );

				SmtpClient client = new SmtpClient();
				client.Send( string.Format( "{0} <{1}>", user.Name, user.Email ),
							ConfigurationManager.AppSettings["AdministratorEmail"],
							"Nytt konto på Wish2",
							body );
			}
			catch
			{
				Debug.WriteLine( "Det gick inte att skicka mailet." );
			}
		}

		private void SendApprovalConfirmation( string username )
		{
			User user = Service.GetUser( username );
			if (user != null)
			{
				string body = string.Format( "Ditt konto har blivit godkänt. Du kan nu logga in på {0}",
											ConfigurationManager.AppSettings["ApplicationUrl"] );
				SmtpClient client = new SmtpClient
										{
											Host = ConfigurationManager.AppSettings["SmtpServer"]
										};
				try
				{
					client.Send( ConfigurationManager.AppSettings["AdministratorEmail"], user.Email,
								"Ditt konto på Önskelistemaskinen har aktiverats", body );
				}
				catch
				{
					Debug.WriteLine( "Det gick inte att skicka mailet." );
				}
			}
		}

		#endregion
	}
}
