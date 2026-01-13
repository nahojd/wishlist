global using WishList.Api.Extensions;

using System.Text.Json;
using Microsoft.OpenApi;
using WishList.Api.Security;
using CommandLine;
using WishList.Api;

if (args.Any(x => x == "--generatetoken")) {
	JwtHelper.GenerateApiToken(args);
	return;
}
if (args.Any(x => x == "--generatekeypair")) {
	JwtHelper.GenerateKeyPair();
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

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var environment = builder.Environment;

string DefaultCorsPolicy = "VeryRelaxedCorsPolicy";

// ConfigureServices
builder.Services.AddCors(options =>
{
	options.AddPolicy(DefaultCorsPolicy,
		builder =>
		{
			builder
// #if DEBUG
				.AllowAnyOrigin()
// #else
// 				.WithOrigins(appSettings.AllowedOrigins)
// #endif
				.SetIsOriginAllowedToAllowWildcardSubdomains()
				.AllowAnyHeader()
				.AllowAnyMethod();
		});
});

builder.Services.SetupAuthentication(configuration);
builder.Services.AddAuthorization();

builder.Services.AddControllers()
	.AddJsonOptions(options => {
		options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
		options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
	});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "WishList API",
		Version = "1.0"
	});

	c.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
	{
		Scheme = "bearer",
		BearerFormat = "JWT",
		Type = SecuritySchemeType.Http,
		In = ParameterLocation.Header
	});

	c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement()
	{
		[new OpenApiSecuritySchemeReference("BearerAuth", doc)] = ["readAccess", "writeAccess"]
	});
});

var app = builder.Build();

// Configure swagger
if (environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
		options.RoutePrefix = string.Empty;
	});
}

app.UseCors(DefaultCorsPolicy);
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UpgradeDatabase().Run();
