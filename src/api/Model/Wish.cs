namespace WishList.Api.Model;

public partial class Wish
{
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Description { get; set; }
	public string? LinkUrl { get; set; }
}

public partial class Wish
{
	public static Wish Create(DataAccess.Entities.Wish dbObj)
	{
		return new() {
			Id = dbObj.Id,
			Description = dbObj.Description,
			LinkUrl = dbObj.LinkUrl,
			Name = dbObj.Name
		};
	}
}