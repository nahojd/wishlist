namespace WishList.Api.DataAccess.Entities;

public class User
{
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Email { get; set; }
	public string? Password { get; set; }
	public bool Notify { get; set; }
	public bool Verified { get; set; }
	public string? PwdResetToken { get; set; }
	public DateTime? PwdResetExpires { get; set; }
}