namespace WishList.Api.DataAccess.Entities;

public class User
{
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Email { get; set; }
	public string? Password { get; set; }
	public bool Verified { get; set; }
}