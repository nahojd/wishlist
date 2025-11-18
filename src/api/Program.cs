global using WishList.Api.Extensions;
using CommandLine;
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
if (args.Contains("--importdata"))
{
	DataImportOptions? options = null;
	Parser.Default.ParseArguments<DataImportOptions>(args)
		.WithParsed(opts => {
				options = opts;
		});

	if (options is null)
	{
		Console.WriteLine("Options missing?");
		return;
	}

	var importer = new DataImporter(options);
	importer.ImportData();
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