namespace WishList.Api.Extensions;

public static class ConfigurationExtensions
{
	public static string? WishListConnectionString(this IConfiguration config) => config.GetConnectionString("WishList");

	public static string JwtSettings_Audience(this IConfiguration config) => config.GetValue("JwtSettings:Audience", "https://wish.driessen.se")!;
	public static string? JwtSettings_PrivateKey(this IConfiguration config) => config["JwtSettings:PrivateKey"];
	public static string JwtSettings_PublicKey(this IConfiguration config) => config["JwtSettings:PublicKey"]!;
	public static string JwtSettings_Issuer(this IConfiguration config) => config.GetValue("JwtSettings:Issuer", "https://wish.driessen.se")!;
	public static bool JwtSettings_IncludeErrorDetails(this IConfiguration config) => config.GetValue("JwtSettings:IncludeErrorDetails", false);
}
