global using WishList.Api.Extensions;
using WishList.Api;
using WishList.Api.DataAccess;

if (args.Any(x => x == "--generatetoken")) {
	WishList.Api.Security.JwtHelper.GenerateApiToken(args);
	return;
}
if (args.Any(x => x == "--generatekeypair")) {
	WishList.Api.Security.JwtHelper.GenerateKeyPair();
	return;
}


Host.CreateDefaultBuilder(args)
	.ConfigureWebHostDefaults(webBuilder =>
	{
		webBuilder.UseStartup<Startup>();
	})
	.Build()
	.UpgradeDatabase()
	.Run();

public static class HostExtensions {
	public static IHost UpgradeDatabase(this IHost host)
	{
		var configuration = host.Services.GetService<IConfiguration>();
		var logger = host.Services.GetService<ILogger<DbMigrations>>();

		DbMigrations.Run(logger!, configuration?.WishListConnectionString());

		return host;
	}
}