using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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

		services.AddControllers();

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();
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