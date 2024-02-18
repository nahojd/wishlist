using System.Security.Claims;

namespace WishList.Api.Extensions;

public static class UserExtensions
{
	public static int GetUserId(this ClaimsPrincipal user) => user.HasClaim(x => x.Type == ClaimTypes.Name) ? int.Parse(user.Claims.Single(x => x.Type == ClaimTypes.Name).Value) : -1;
}