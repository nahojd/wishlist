using System.Text.RegularExpressions;

namespace WishList.Api.Extensions;

public static partial class StringExtensions
{
	[GeneratedRegex("^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)+$")]
	public static partial Regex ValidEmailExpression();
	public static bool IsValidEmail(this string? email) => !string.IsNullOrWhiteSpace(email) && ValidEmailExpression().IsMatch(email);

	public static bool IsValidPassword(this string? pwd) => !string.IsNullOrEmpty(pwd) && pwd.Length >= 12;
}