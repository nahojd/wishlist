using System;
using System.Security.Principal;
using System.Web.Mvc;
using WishList.Data;
using WishList.Services;
using WishList.WebUI.ModelBinders;

namespace WishList.WebUI.Controllers
{
	public class WishController : Controller
	{
		private readonly IWishService _wishService;
		private readonly IUserService _userService;

		public WishController( IWishService wishService, IUserService userService )
		{
			_wishService = wishService;
			_userService = userService;
		}

		[AcceptVerbs( "GET" )]
		[Authorize]
		public virtual ActionResult Edit( int wishId, [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentUser )
		{
			Wish wish = _wishService.GetWish( wishId );
			if (wish == null)
			{
                throw new ArgumentException(String.Format("No wish with id '{0}'", wishId));
			}

			if (!wish.Owner.Name.Equals( currentUser.Identity.Name, StringComparison.InvariantCultureIgnoreCase ))
			{
				throw new InvalidOperationException( "Cannot edit another user's wish" );
			}

			return View( wish );
		}

		[AcceptVerbs( "POST" )]
		[Authorize]
		public virtual ActionResult Edit( int wishId, Wish editedWish, [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentUser )
		{
			if (ModelState.IsValid)
			{
				Wish wish = _wishService.GetWish( wishId );

				if (!wish.Owner.Name.Equals( currentUser.Identity.Name, StringComparison.InvariantCultureIgnoreCase ))
				{
					throw new InvalidOperationException( "Cannot edit another user's wish" );
				}

				wish.Name = editedWish.Name;
				wish.Description = editedWish.Description;
				wish.LinkUrl = editedWish.LinkUrl;
				_wishService.SaveWish( wish, false );

				return RedirectToAction( "Show", "List", new { id = wish.Owner.Name } );
			}

			return View( editedWish );
		}

		// /Wish/New
		[AcceptVerbs( "GET" )]
		public virtual ActionResult New()
		{
			return View();
		}

		[AcceptVerbs( "POST" )]
		[Authorize]
		public virtual ActionResult New( Wish newWish, [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal user )
		{
			if (ModelState.IsValid)
			{
				newWish.Owner = _userService.GetUser( user.Identity.Name );
				_wishService.SaveWish( newWish, true );

				return RedirectToAction( "Show", "List", new { id = user.Identity.Name } );
			}

			return View(newWish);
		}

		[Authorize]
		public virtual ActionResult Call( int wishId, [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal user )
		{
			if (user == null)
			{
				throw new InvalidOperationException( "Cannot call wish when user is not logged in!" );
			}

			Wish wish = _wishService.GetWish( wishId );
			if (wish.IsCalled)
			{
				throw new InvalidOperationException( "Cannot call wish that has already been called!" );
			}

			wish.CalledByUser = _userService.GetUser( user.Identity.Name );
			_wishService.SaveWish( wish, true );

			return Content( "ok" );

		}

		[Authorize]
		public virtual ActionResult UnCall( int wishId, [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal user )
		{
			if (user == null)
			{
				throw new InvalidOperationException( "Cannot uncall wish when user is not logged in!" );
			}

			Wish wish = _wishService.GetWish( wishId );
			wish.CalledByUser = null;
			_wishService.SaveWish( wish, true );

			return RedirectToAction( "Show", "List", new { id = wish.Owner.Name } );
		}

		[Authorize]
		public virtual ActionResult Delete( int wishId, [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentPrincipal )
		{
			if (currentPrincipal == null)
			{
				throw new InvalidOperationException( "Cannot delete wish when user is not logged in!" );
			}

			Wish wish = _wishService.GetWish( wishId );
			User currentUser = _userService.GetUser( currentPrincipal.Identity.Name );
			if (wish.Owner.Id != currentUser.Id)
			{
				throw new InvalidOperationException( "Cannot delete another user's wish" );
			}

			_wishService.RemoveWish( wish );
			return RedirectToAction( "Show", "List", new { id = wish.Owner.Name } );
		}
	}
}
