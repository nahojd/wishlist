using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace WishList.Api.Security;

public static class AuthenticationHelper
{
	public static AuthenticationBuilder SetupAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		return services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
				using RSA rsa = RSA.Create();
				rsa.ImportRSAPublicKey(
					source: Convert.FromBase64String(configuration.JwtSettings_PublicKey()),
					bytesRead: out int _
				);
				var key = new RsaSecurityKey(rsa.ExportParameters(false));

				options.IncludeErrorDetails = configuration.JwtSettings_IncludeErrorDetails(); // <- great for debugging

				options.TokenValidationParameters = new TokenValidationParameters {
					IssuerSigningKey = key,
					ValidIssuers = [configuration.JwtSettings_Issuer()],
					ValidAudiences = [configuration.JwtSettings_Audience()],
					RequireSignedTokens = true,
					RequireExpirationTime = true, // <- JWTs are required to have "exp" property set
					ValidateLifetime = true, // <- the "exp" WILL be validated
					ValidateAudience = true,
					ValidateIssuer = true
				};

				options.Events = new JwtBearerEvents
				{
					OnTokenValidated = ctx => {
						var claims = new List<Claim>();
						//Om vi behöver sätta claims ska vi göra det här.
						var identity = new ClaimsIdentity(claims);
						ctx.Principal!.AddIdentity(identity);
						return Task.CompletedTask;
					}
				};
			});
	}
}