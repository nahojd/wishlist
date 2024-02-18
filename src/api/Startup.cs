using System.Text.Json;
using Microsoft.OpenApi.Models;
using WishList.Api.Security;

namespace WishList.Api;

public class Startup(IConfiguration configuration, IWebHostEnvironment environment)
{
	private readonly IConfiguration configuration = configuration;
	private readonly IWebHostEnvironment environment = environment;

	public void ConfigureServices(IServiceCollection services)
	{
		services.SetupAuthentication(configuration);
		services.AddAuthorization();

		services.AddControllers()
			.AddJsonOptions(options => {
				options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
				options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
				options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
			});

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(c => {
			c.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "WishList API",
				Version = "v1"
			});

			c.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme {
				Scheme = "bearer",
				BearerFormat = "JWT",
				Type = SecuritySchemeType.Http,
				In = ParameterLocation.Header
			});

			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme {
						Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "BearerAuth"}
					},
					Array.Empty<string>()
				}
			});
		});
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		// Configure the HTTP request pipeline.
		if (env.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseRouting();

		app.UseHttpsRedirection();
		app.UseAuthentication();
		app.UseAuthorization();

		app.UseEndpoints(builder => {
			builder.MapControllers();
		});
	}
}