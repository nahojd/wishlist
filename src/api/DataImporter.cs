using System.Data;
using CommandLine;
using Dapper;
using Microsoft.AspNetCore.Identity;
using WishList.Api.DataAccess;

namespace WishList.Api;

public class DataImporter(DataImportOptions options)
{
	public void ImportData()
	{
		if (options.UsersFile is null && options.FriendsFile is null && options.UsersFile is null)
		{
			Console.WriteLine("Inget att importera, varken Users, Friends eller Wishes angivet");
			return;
		}

		Console.WriteLine("Import kommer att skriva över de angivna tabllerna helt.");
		Console.WriteLine("Tryck ENTER för att importera eller Ctrl-C för att avbryta");
		Console.ReadLine();

		var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> {
						{ "ConnectionStrings:WishList", options.ConnectionStringDb }
					}).Build();
		using var conn = DbHelper.OpenConnection(config);
		using var transaction = conn.BeginTransaction();

		try
		{
			if (!string.IsNullOrWhiteSpace(options.UsersFile))
				ImportUsers(options.UsersFile, conn, transaction);
			if (!string.IsNullOrWhiteSpace(options.FriendsFile))
				ImportFriends(options.FriendsFile, conn, transaction);
			if (!string.IsNullOrWhiteSpace(options.WishesFile))
				ImportWishes(options.WishesFile, conn, transaction);

			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
			throw;
		}
	}

	private static void ImportUsers(string usersFile, IDbConnection conn, IDbTransaction transaction)
	{
		Console.WriteLine();
		Console.WriteLine("Importerar användare...");
		conn.Execute("delete from User", transaction: transaction);

		var lines = File.ReadAllLines(usersFile);
		foreach(var line in lines.Where(x => !string.IsNullOrWhiteSpace(x)))
		{
			var data = line.Split(';');

			if (!int.TryParse(data[0], out int userId))
			{
				// Console.WriteLine("  Hoppar över rad - börjar inte med userid");
				continue;
			}
			if (data[3] != "NULL")
			{
				Console.WriteLine("  Hoppar över ej bekräftad användare " + data[1]);
				continue;
			}

			var email = data[1];
			var name = data[2];
			var notify = data[4] == "1";
			var passwordHasher = new PasswordHasher<string>();
			var hash = passwordHasher.HashPassword(email, Guid.NewGuid().ToString()); //Sätt random lösenord

			conn.Execute("insert into User (Id, Name, Email, Password, Verified, Notify) values (@userId, @name, @email, @hash, 1, @notify)",
				new { userId, name, email, hash, notify }, transaction: transaction);

			Console.WriteLine($"  Importerade {name} ({email})");
		}
	}

	private static void ImportFriends(string friendsFile, IDbConnection conn, IDbTransaction transaction)
	{
		Console.WriteLine();
		Console.Write("Importerar vänner");
		conn.Execute("delete from Friend", transaction: transaction);

		var lines = File.ReadAllLines(friendsFile);
		var count = 0;
		foreach(var line in lines.Where(x => !string.IsNullOrWhiteSpace(x)))
		{
			var data = line.Split(';');
			if (!int.TryParse(data[0], out int userId))
			{
				// Console.WriteLine("  Hoppar över rad - börjar inte med userid");
				continue;
			}

			int friendId = int.Parse(data[1]);

			conn.Execute("insert into Friend (UserId, FriendId) values (@userId, @friendId)",
				new { userId, friendId }, transaction: transaction);

			Console.Write($".");
			count++;
		}

		Console.WriteLine();
		Console.WriteLine($"  Importerade {count} vänner.");
	}

	private void ImportWishes(string wishesFile, IDbConnection conn, IDbTransaction transaction)
	{
		Console.WriteLine();
		Console.Write("Importerar önskningar");
		conn.Execute("delete from Wish", transaction: transaction);

		var lines = File.ReadAllLines(wishesFile);
		var count = 0;
		foreach(var line in lines.Where(x => !string.IsNullOrWhiteSpace(x)))
		{
			var data = line.Split(';');
			if (!int.TryParse(data[0], out int wishId))
			{
				// Console.WriteLine("  Hoppar över rad - börjar inte med wishId");
				continue;
			}

			var name = TaBortOnödigaCitationstecken(data[1]);
			var description = TaBortOnödigaCitationstecken(data[2]);
			var ownerId = int.Parse(data[3]);
			var tjingadBy = data[4] != "NULL" ? int.Parse(data[4]) : (int?)null;
			var linkUrl = data[5] != "NULL" ? data[5] : null;
			var created = DateTime.Parse(data[6]);
			var updated = DateTime.Parse(data[7]);

			conn.Execute("insert into Wish (Id, Name, Description, LinkUrl, OwnerId, TjingadBy, Created, Updated) values (@wishId, @name, @description, @linkUrl, @ownerId, @tjingadBy, @created, @updated)",
				new { wishId, name, description, ownerId, tjingadBy, linkUrl, created, updated }, transaction: transaction);

			Console.Write($".");
			count++;
		}

		Console.WriteLine();
		Console.WriteLine($"  Importerade {count} önskningar.");
	}

	private static string TaBortOnödigaCitationstecken(string str)
	{
		if (str[0] == '"')
			str = str[1..];
		if (str[^1] == '"')
			str = str[..^1];
		str = str.Replace("\"\"", "\"");
		return str;
	}
}

public class DataImportOptions
{
	[Option("importdata", HelpText = "Startar applikationen i importera-data-läge")]
	public bool ImportData { get; set; }

	[Option("db", Required = true, HelpText = "Connectionsträng till databasen")]
	public string? ConnectionStringDb { get; set; }
	[Option("users", HelpText = "Sökväg till csv-filen med Users")]
	public string? UsersFile { get; set; }
	[Option("wishes", HelpText = "Sökväg till csv-filen med Wishes")]
	public string? WishesFile { get; set; }
	[Option("friends", HelpText = "Sökväg till csv-filen med Friends")]
	public string? FriendsFile { get; set; }
}