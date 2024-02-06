namespace WishList.Api.Model;

public partial class User
{
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Email { get; set; }
}

public partial class User
{
	public static User Create(DataAccess.Entities.User dbObj)
	{
		return new() {
			Id = dbObj.Id,
			Name = dbObj.Name,
			Email = dbObj.Email
		};
	}
}