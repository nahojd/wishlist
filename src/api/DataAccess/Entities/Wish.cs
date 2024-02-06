namespace WishList.Api.DataAccess.Entities;

public class Wish
{
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Description { get; set; }
	public string? LinkUrl { get; set; }
}