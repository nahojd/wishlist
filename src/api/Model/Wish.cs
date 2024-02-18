namespace WishList.Api.Model;
using Db = DataAccess.Entities;

public partial class Wish
{
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Description { get; set; }
	public string? LinkUrl { get; set; }
	public User? Owner { get; set; }
	public User? TjingadBy { get; set; }
}

public partial class Wish
{
	public static Wish Create(Db.Wish dbObj, Db.User? owner, Db.User? tjingadBy)
	{
		return new() {
			Id = dbObj.Id,
			Description = dbObj.Description,
			LinkUrl = dbObj.LinkUrl,
			Name = dbObj.Name,
			Owner = owner != null ? User.Create(owner) : null,
			TjingadBy = tjingadBy != null ? User.Create(tjingadBy) : null
		};
	}
}
