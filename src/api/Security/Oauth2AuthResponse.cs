using System.Text.Json.Serialization;

namespace WishList.Api.Security;

public class Oauth2AuthResponse
{
	public Oauth2AuthResponse() {

	}

	public Oauth2AuthResponse(string accessToken, int expiresIn)
	{
		AccessToken = accessToken;
		ExpiresIn = expiresIn;
		TokenType = "Bearer";
	}

	[JsonPropertyName("access_token")]
	public string AccessToken { get; set; } = string.Empty;
	[JsonPropertyName("expires_in")]
	public int ExpiresIn { get; set; }
	[JsonPropertyName("token_type")]
	public string? TokenType { get; set; }
	// [JsonPropertyName("scope")]
	// public string? Scope { get; set; }
	// [JsonPropertyName("refresh_token")]
	// public string? RefreshToken { get; set; }
}
