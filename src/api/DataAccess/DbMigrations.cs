using System.Diagnostics;
using Dapper;
using MySqlConnector;

namespace WishList.Api.DataAccess;

public class DbMigrations
{
	//Bumpas varje gång någon databasändring görs
	public const int CurrentVersion = 6;

	public static void Run(ILogger<DbMigrations> logger, string? connectionString)
	{
		if (string.IsNullOrWhiteSpace(connectionString))
		{
			logger.LogInformation("Ingen connectionsträng konfigurerad -- hoppar över migrering");
			return;
		}


		using var conn = new MySqlConnection(connectionString);
		conn.Open();
		var dbVersion = GetDbVersion(conn, logger);

		if (dbVersion == CurrentVersion)
		{
			logger.LogInformation("Ingen migrering nödvändig, databasen är redan version {CurrentVersion}", CurrentVersion);
			return;
		}
		if (dbVersion > CurrentVersion)
		{
			logger.LogError("Högre version i databasen ({dbVersion}) än i koden ({CurrentVersion})! Detta kan ställa till problem!!", dbVersion, CurrentVersion);
			return;
		}

		//Annars är dbVersion < CurrentVersion, och då behöver vi uppgradera.
		UpgradeDatabase(conn, dbVersion, logger);
	}

	private static int GetDbVersion(MySqlConnection conn, ILogger<DbMigrations> logger)
	{
		try
		{
			return conn.QuerySingleOrDefault<int>(
				@"CREATE TABLE IF NOT EXISTS DbSettings (
						DbVersion int
					);
					SELECT DbVersion from DbSettings LIMIT 1;"
				);
		}
		catch (Exception ex)
		{
			logger.LogCritical(ex, "Kunde inte hämta databasversionen");
			return CurrentVersion; //För om det inte ens gick att hämta ut versionen vill vi inte försöka göra någon migrering
		}
	}

	private static void UpgradeDatabase(MySqlConnection conn, int dbVersion, ILogger<DbMigrations> logger)
	{
		/*
		* OBS! Denna metod måste skrivas oerhört defensivt, den får aldrig kunna förstöra databasen!
		* Det ska vara helt okej att köra denna metod upprepade gånger utan att någon går sönder, så varje ändring
		* måste först kontrollera att den ska genomföras (t.ex. kolla att en tabell/kolumn inte redan finns).
		* */

		var stopwatch = new Stopwatch();
		stopwatch.Start();

		using var trans = conn.BeginTransaction();

		try
		{
			if (dbVersion == 0) //Betyder att databasen inte ens innehåller versioninformation ännu, vi måste alltså skapa versionstabellen och stoppa in ett värde.
			{
				// conn.Execute(@"CREATE TABLE [dbo].[DbSettings](DbVersion [int] NOT NULL)", transaction: trans);
				conn.Execute("INSERT INTO DbSettings (DbVersion) VALUES (@CurrentVersion)", new { CurrentVersion }, transaction: trans);
			}
			else
			{
				conn.Execute("UPDATE DbSettings SET DbVersion = @CurrentVersion", new { CurrentVersion }, transaction: trans);
			}

			if (dbVersion < 1)
			{
				//Skapa Wish-tabellen
				conn.Execute(@"CREATE TABLE IF NOT EXISTS Wish (
								Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
								Name VARCHAR(255) NOT NULL,
								Description VARCHAR(1000) NULL,
								LinkUrl VARCHAR(255) NULL
								)", transaction: trans);
			}

			if (dbVersion < 2)
			{
				//Skapa User-tabellen
				conn.Execute(@"CREATE TABLE IF NOT EXISTS User (
								Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
								Name VARCHAR(50) NOT NULL,
								Email VARCHAR(255) NOT NULL,
								Password VARCHAR(255) NOT NULL,
								Verified BIT NULL
								)", transaction: trans);
			}

			if (dbVersion < 3)
			{
				//Lägg till OwnerId och TjingadBy i Wish
				conn.Execute(@"ALTER TABLE Wish ADD COLUMN IF NOT EXISTS OwnerId INT NOT NULL;
							   ALTER TABLE Wish ADD COLUMN IF NOT EXISTS TjingadBy INT NULL;", transaction: trans);
			}

			if (dbVersion < 4)
			{
				//Lägg till kolumner för reset password
				conn.Execute(@"ALTER TABLE User ADD COLUMN IF NOT EXISTS PwdResetToken VARCHAR(255) NULL;
							   ALTER TABLE User ADD COLUMN IF NOT EXISTS PwdResetExpires DATETIME NULL", transaction: trans);
			}

			if (dbVersion < 5)
			{
				//Lägg till created och updated på Wish
				conn.Execute(@"ALTER TABLE Wish ADD COLUMN IF NOT EXISTS Created DATETIME NOT NULL;
							   ALTER TABLE Wish ADD COLUMN IF NOT EXISTS Updated DATETIME NULL", transaction: trans);
			}

			if (dbVersion < 6)
			{
				//Lägg till kolumn för notifiering
				conn.Execute(@"ALTER TABLE User ADD COLUMN IF NOT EXISTS Notify BIT NULL", transaction: trans);
			}

			trans.Commit();
			logger.LogInformation("Genomförde migrering av databasen från version {dbVersion} till version {CurrentVersion} utan problem", dbVersion, CurrentVersion);
		}
		catch (Exception ex)
		{
			logger.LogCritical(ex, "Kunde inte genomföra migrering från version {dbVersion} till {CurrentVersion}", dbVersion, CurrentVersion);
			trans.Rollback();
		}
		finally
		{
			stopwatch.Stop();
			logger.LogDebug("Migreringen tog {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds);
		}
	}
}