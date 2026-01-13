using WishList.Api.DataAccess;

namespace WishList.Api;

public static class HostExtensions {
	public static IHost UpgradeDatabase(this IHost host)
	{
		var configuration = host.Services.GetService<IConfiguration>();
		var logger = host.Services.GetService<ILogger<DbMigrations>>();

		DbMigrations.Run(logger!, configuration?.WishListConnectionString());

		return host;
	}
}