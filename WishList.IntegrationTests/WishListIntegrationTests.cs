using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WishList.Data.DataAccess;
using WishList.Data;
using WishList.Data.Extensions;
using WishList.Services;
using WishList.Data.Filters;
using DB = WishList.SqlRepository.Data;
using System.Configuration;

namespace WishList.IntegrationTests
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class WishListIntegrationTests
	{
		private SqlRepository.LinqWishListDataContext dataContext;
		private UserService service;
		private SqlWishListRepository rep;
		private WishService wishService;

		private int firstUserId;
		private int secondUserId;

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}



		[TestInitialize]
		public void SetUp()
		{
#if DEBUG
			dataContext = new SqlRepository.LinqWishListDataContext(ConfigurationManager.ConnectionStrings["LocalTestDb"].ConnectionString);
#else 
			dataContext = new SqlRepository.LinqWishListDataContext();
#endif

			dataContext.Connection.Open();
			var transaction = dataContext.Connection.BeginTransaction();
			dataContext.Transaction = transaction;
			rep = new SqlWishListRepository( dataContext );
			service = new UserService( rep );
			wishService = new WishService( rep, new TestMailService() );


			PopulateDB();
		}

		private void PopulateDB()
		{
			var firstUser = new DB.User { Name = "First", Email = "first@example.com" };
			var secondUser = new DB.User { Name = "Second", Email = "second@example.com" };
			dataContext.Users.InsertOnSubmit( firstUser );
			dataContext.Users.InsertOnSubmit( secondUser );

			var wishes = new List<DB.Wish>{ 
				new DB.Wish { Name = "First wish", Description = "1st", Created = DateTime.Now, User = firstUser }, 
				new DB.Wish { Name = "Second wish", Description = "2nd", Created = DateTime.Now, User = secondUser } 
			};
			dataContext.Wishes.InsertAllOnSubmit( wishes );

			dataContext.SubmitChanges();
			firstUserId = firstUser.UserId;
			secondUserId = secondUser.UserId;
		}

		[TestCleanup]
		public void TearDown()
		{
			dataContext.Transaction.Rollback();
			dataContext.Connection.Close();
			rep.Dispose();
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion



		[TestMethod]
		public void SqlRepository_Can_Return_Wishes()
		{
			IList<Wish> wishes = rep.GetWishes().ToList<Wish>();

			Assert.IsNotNull( wishes, "Wishes was null" );
			Assert.IsTrue( wishes.Count > 0, "Wish count was 0" );
		}

		[TestMethod]
		public void SqlRepository_Can_Return_ShoppingList()
		{
			IList<Wish> shoppinglist = wishService.GetShoppingList( firstUserId );
			Assert.IsNotNull( shoppinglist, "Shoppinglist was null" );
		}

		[TestMethod]
		public void SqlRepository_Can_Add_Wish()
		{
			Wish wish = new Wish
			{
				Name = "Addtest wish",
				Description = "Description of wish",
				Owner = new User() { Id = firstUserId },
				CalledByUser = new User() { Id = secondUserId },
				LinkUrl = "http://localhost"
			};

			rep.SaveWish( wish );

			var loadedWishes = (from w in dataContext.Wishes
								where w.Name == wish.Name
								select w).ToList();

			Assert.IsTrue( loadedWishes.Count == 1, "Did not contain 1 wish" );
			Assert.AreEqual( loadedWishes[0].Name, wish.Name, "Wish name was not correct" );
			Assert.AreEqual( loadedWishes[0].Description, wish.Description, "Wish description was not correct" );
			Assert.AreEqual( loadedWishes[0].OwnerId, wish.Owner.Id, "Wish ownerid was not correct" );
			Assert.AreEqual( loadedWishes[0].TjingedById, wish.CalledByUser.Id, "Wish tjingedbyid was not correct" );
			Assert.AreEqual( loadedWishes[0].LinkUrl, wish.LinkUrl, "Wish linkurl was not correct" );
			Assert.AreEqual( loadedWishes[0].Created, loadedWishes[0].Changed, "Change time was not equal to created time" );
		}

		[TestMethod]
		public void Newly_Created_Wish_Will_Have_A_Change_Date()
		{
			Wish wish = new Wish { Owner = new User { Id = firstUserId }, Name = "Change date test wish", Description = "Whatever..." };
			wish = rep.SaveWish( wish );

			Assert.IsNotNull( wish.Changed, "Changed date was null on newly created wish" );
		}

		[TestMethod]
		public void Saving_Wish_Will_Truncate_Too_Long_String_Properties()
		{
			Wish wish = new Wish
			{
				Name = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec tempus, neque eu imperdiet posuere, orci sapien faucibus metus, a suscipit lacus metus.",
				Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas ut mi et enim condimentum blandit. Vivamus tincidunt nulla eu sapien. Morbi vel metus. Nunc ac turpis. Duis sollicitudin viverra arcu. Aliquam erat volutpat. Quisque lorem libero, consequat a, mollis et, rhoncus vel, velit. Donec elementum luctus nunc. Mauris ultrices, turpis et condimentum ornare, purus augue malesuada lacus, nec porta ipsum nisi eu ligula. Donec eu massa. Maecenas at dolor. Curabitur non augue. Praesent viverra. Donec sit amet nisl. Mauris consequat. In volutpat, est sit amet condimentum tempus, tellus nullam.",
				LinkUrl = "http://Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean magna purus, dictum et, interdum vitae, adipiscing non, enim. Nunc nibh mi, malesuada sit amet, tristique et, pellentesque eget, nisi. Etiam metus urna, pellentesque id, volutpat at, aliquet vitae, dolor. In accumsan. Praesent volutpat.com",
				Owner = new User { Id = firstUserId }
			};

			Wish createdWish = rep.SaveWish( wish );

			Assert.AreEqual( wish.Name.Substring( 0, 100 ), createdWish.Name );
			Assert.AreEqual( wish.Description.Substring( 0, 500 ), createdWish.Description );
			Assert.AreEqual( wish.LinkUrl.Substring( 0, 255 ), createdWish.LinkUrl );
		}

		[TestMethod]
		public void SqlRepository_Can_Remove_Wish()
		{
			//First we add the wish we want to remove later.
			Wish wish = new Wish
			{
				Name = "Removetest wish",
				Description = "Description of wish",
				Owner = new User() { Id = firstUserId },
				CalledByUser = new User() { Id = secondUserId },
				LinkUrl = "http://localhost"
			};
			int numberOfWishes = rep.GetWishes().Count<Wish>();
			Wish savedWish = rep.SaveWish( wish );

			Assert.AreEqual<int>( numberOfWishes + 1, rep.GetWishes().Count() );
			rep.RemoveWish( savedWish );
			Assert.AreEqual<int>( numberOfWishes, rep.GetWishes().Count(), "Number of wishes in repository did not decrease" );

			var loadedWish = (from w in dataContext.Wishes
							  where w.WishId == savedWish.Id
							  select w).SingleOrDefault();

			Assert.IsNull( loadedWish, "Removed wish was still found in repository" );
		}

		[TestMethod]
		public void SqlRepository_Can_Update_Wish()
		{
			//Create a wish to work with
			Wish wish = new Wish()
			{
				Name = "Update test wish",
				Description = "The test description",
				LinkUrl = "http://notupdated.yet",
				Owner = new User() { Id = firstUserId },
				CalledByUser = null
			};
			Wish wishToUpdate = rep.SaveWish( wish );

			//Do the update 
			wishToUpdate.Name = "Updated test wish";
			wishToUpdate.Description = "Updated description";
			wishToUpdate.LinkUrl = "http://isupdated.now";
			wishToUpdate.CalledByUser = new User() { Id = secondUserId };
			Wish updatedWish = rep.SaveWish( wishToUpdate );

			Assert.IsNotNull( updatedWish, "Updated wish could not be loaded" );
			Assert.AreEqual<string>( wishToUpdate.Name, updatedWish.Name, "Name was not updated correctly" );
			Assert.AreEqual<string>( wishToUpdate.Description, updatedWish.Description, "Description was not updated correctly" );
			Assert.AreEqual<string>( wishToUpdate.LinkUrl, updatedWish.LinkUrl, "LinkUrl was not updated correctly" );
			Assert.AreEqual<int?>( wishToUpdate.CalledByUser.Id, updatedWish.CalledByUser.Id, "CalledByUser was not updated correctly" );
			Assert.AreEqual( wishToUpdate.Created, updatedWish.Created, "Creation time changed while updating!" );

		}

		[TestMethod]
		public void Updating_Wish_Will_Update_ChangeDate()
		{
			//Create a wish to work with
			Wish wish = new Wish()
			{
				Name = "Update test wish",
				Description = "The test description",
				LinkUrl = "http://notupdated.yet",
				Owner = new User() { Id = firstUserId },
				CalledByUser = null
			};
			wish = rep.SaveWish( wish );
			DateTime lastChanged = wish.Changed.Value;

			rep.SaveWish( wish );
			var updatedWish = rep.GetWishes().WithId( wish.Id );

			Assert.IsTrue( updatedWish.Changed > lastChanged, "Change date was not updated on save" );
		}

		[TestMethod]
		public void Calling_Wish_Will_Not_Update_ChangeDate()
		{
			//Create a wish to work with
			Wish wish = new Wish()
			{
				Name = "Update test wish",
				Description = "The test description",
				LinkUrl = "http://notupdated.yet",
				Owner = new User() { Id = firstUserId },
				CalledByUser = null
			};
			wish = rep.SaveWish( wish );
			DateTime lastChanged = wish.Changed.Value;

			wish.CalledByUser = new User { Id = firstUserId };
			rep.SaveWish( wish );

			var updatedWish = rep.GetWishes().WithId( wish.Id );

			Assert.IsTrue( lastChanged.IsAlmostEqualTo( updatedWish.Changed.Value ), "Calling a wish updated the change date" );
		}
	}
}
