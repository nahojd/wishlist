using System;
using System.Linq;
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
using System.Collections.Generic;

namespace WishList.WebUI.Controllers
{
	public class AccountController : Controller
	{
		public class UserData
		{
			public string Error { get; set; }
			public string Info { get; set; }
			public User User { get; set; }
			public IDictionary<string, bool> Friends { get; set; }
		}

		public class SaveAccountResult
		{
			public bool Success { get; set; }
			public string StatusMessage { get; set; }
		}

		private IUserService _service;

		public AccountController()
			: this( new UserService() )
		{
		}

		public AccountController( IUserService service )
		{
			_service = service;
		}

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

		[HttpGet]
		public virtual ActionResult Authenticate()
		{
			return View( "Login" );
		}

		[HttpPost]
		public virtual ActionResult Authenticate( string username, string password, string returnUrl )
		{

			bool success = _service.ValidateUser( username, password );

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

		public virtual ActionResult Create()
		{
			return View( new SaveAccountResult() );
		}

		[HttpPost]
		public virtual ActionResult Create( User user, string password, string message )
		{
			SaveAccountResult result = new SaveAccountResult { StatusMessage = "Ditt konto kunde inte skapas" };

			user.SetPassword( password );
			User createdUser = _service.CreateUser( user );

			if (createdUser == null)
				return View( "Create", result );

			result.Success = true;
			result.StatusMessage = "Ditt konto är skapat men är ännu inte aktiverat. Detta sker inom kort.";
			SendRequestForApproval( createdUser, _service.GetApprovalTicket( user.Name ).Value, message );

			return View( "Create", result );
		}

		[HttpGet]
		[Authorize]
		public virtual ActionResult Edit( [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentUser )
		{
			User user = _service.GetUser( currentUser.Identity.Name );

			return View( new UserData
			{
				User = user,
				Friends = GetFriends( user )
			} );
		}

		[HttpPost]
		[Authorize]
		public virtual ActionResult Edit( User editedUser, [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentUser )
		{
			//TODO: Validation checks

			User user = _service.GetUser( currentUser.Identity.Name );

			user.Email = editedUser.Email;
			user.NotifyOnChange = editedUser.NotifyOnChange;

			var model = new UserData { User = user, Friends = GetFriends( user ) };
			try
			{
				model.User = _service.UpdateUser( user );
				model.Info = "Ändringen sparad";
			}
			catch
			{
				model.Error = "Kunde inte spara dina ändringar.";
			}

			return View( model );
		}

		private IDictionary<string, bool> GetFriends( User user )
		{
			var friends = _service.GetFriends( user );
			var allUsers = _service.GetUsers();
			var friendDictionary = new SortedDictionary<string, bool>();
			foreach (var u in allUsers)
			{
				if (u.Name != user.Name)
					friendDictionary.Add( u.Name, friends.Any( x => x.Name == u.Name ) );
			}
			return friendDictionary;
		}

		[Authorize]
		[HttpPost]
		public ActionResult AddFriend( [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentUser, string username )
		{
			_service.AddFriend( currentUser.Identity.Name, username );

			return Json( new { status = "ok" } );
		}

		[Authorize]
		[HttpPost]
		public ActionResult RemoveFriend( [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentUser, string username )
		{
			_service.RemoveFriend( currentUser.Identity.Name, username );

			return Json( new { status = "ok" } );
		}

		[Authorize]
		public virtual ActionResult ChangePassword( string password, [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentUser )
		{
			var user = _service.GetUser( currentUser.Identity.Name );
			var model = new UserData
			{
				User = user,
				Friends = GetFriends( user )
			};

			if (!string.IsNullOrEmpty( password ))
			{
				_service.UpdatePassword( user.Name, password );
				model.Info = "Ditt lösenord är uppdaterat!";
			}
			else
			{
				model.Error = "Du kan inte ha ett tomt lösenord!";
			}

			return View( "Edit", model );
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
				_service.ApproveUser( username, guid );
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

				using (var client = new SmtpClient())
				{
					client.Send( string.Format( "{0} <{1}>", user.Name, user.Email ), ConfigurationManager.AppSettings["AdministratorEmail"], "Nytt konto på Wish2", body );
				}
			}
			catch
			{
				Debug.WriteLine( "Det gick inte att skicka mailet." );
			}
		}

		private void SendApprovalConfirmation( string username )
		{
			User user = _service.GetUser( username );
			if (user != null)
			{
				string body = string.Format( "Ditt konto har blivit godkänt. Du kan nu logga in på {0}",
											ConfigurationManager.AppSettings["ApplicationUrl"] );
				using (var client = new SmtpClient())
				{
					try
					{
						client.Send( ConfigurationManager.AppSettings["AdministratorEmail"], user.Email, "Ditt konto på Önskelistemaskinen har aktiverats", body );
					}
					catch
					{
						Debug.WriteLine( "Det gick inte att skicka mailet." );
					}
				}
			}
		}

		#endregion
	}
}
