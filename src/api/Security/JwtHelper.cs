using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace WishList.Api.Security;

public class JwtHelper
{
	public static void GenerateApiToken(string[] args)
	{
		var builder = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.AddEnvironmentVariables()
			.AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly())
			.AddCommandLine(source => {
				source.Args = args;
			});

		var config = builder.Build();

		var userId = args?.FirstOrDefault(x => x.StartsWith("--userid="))?.Replace("--userid=", string.Empty) ?? "0";

		var (token, tokenString) = GenerateToken(config, int.Parse(userId));

		Console.WriteLine($"-----JWT Token-----{Environment.NewLine}{token}{Environment.NewLine}");
		Console.WriteLine($"-----Signed And Encoded-----{Environment.NewLine}{tokenString}{Environment.NewLine}");
	}

	public static (JwtSecurityToken, string) GenerateToken(IConfiguration configuration, int userId, DateTime? expires = null, IDictionary<string, string>? extraClaims = null)
	{
		var privateKey = configuration.JwtSettings_PrivateKey();
		if (privateKey is null)
			throw new InvalidOperationException("JwtSettings:PrivateKey was null!");

		using RSA rsa = RSA.Create();
		rsa.ImportRSAPrivateKey( // Convert the loaded key from base64 to bytes.
			source: Convert.FromBase64String(privateKey), // Use the private key to sign tokens
			bytesRead: out int _); // Discard the out variable

		var creds = new SigningCredentials(
			key: new RsaSecurityKey(rsa) {
				CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false } //Annars kommer det smälla varannan gång!!
				//https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/1433
			},
			algorithm: SecurityAlgorithms.RsaSha256 // Important to use RSA version of the SHA algo
		);

		DateTime jwtDate = DateTime.Now;
		var issuer = configuration.JwtSettings_Issuer();
		var audience = configuration.JwtSettings_Audience();

		var claims = new[] { new Claim(ClaimTypes.Name, userId.ToString()) };
		if (extraClaims != null)
			claims = claims.Concat(extraClaims.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Select(x => new Claim(x.Key, x.Value))).ToArray();

		var token = new JwtSecurityToken(
			issuer,
			audience,
			claims,
			notBefore: jwtDate,
			expires: expires ?? jwtDate.AddYears(10),
			signingCredentials: creds);

		var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

		return (token, tokenString);
	}

	public static void GenerateKeyPair()
	{
		using RSA rsa = RSA.Create();
		Console.WriteLine($"-----Private key-----{Environment.NewLine}{Convert.ToBase64String(rsa.ExportRSAPrivateKey())}{Environment.NewLine}");
		Console.WriteLine($"-----Public key-----{Environment.NewLine}{Convert.ToBase64String(rsa.ExportRSAPublicKey())}");
	}
}
