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

namespace WishList.IntegrationTests
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class WishListIntegrationTests
	{
		private UserService service;
		private IWishListRepository rep;
		private WishService wishService;

		public WishListIntegrationTests()
		{
			rep = new SqlWishListRepository();
			service = new UserService( rep );
			wishService = new WishService( rep, new TestMailService() );
		}

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

		}

		[TestCleanup]
		public void TearDown()
		{
			//Clean up database
			using (SqlRepository.LinqWishListDataContext db = new SqlRepository.LinqWishListDataContext())
			{
				//Remove wishes that was added in SqlRepository_Can_Add_Wish and SqlRepository_Can_Remove_Wish
				var removeWishQuery = from w in db.Wishes
									  where w.Name == "Addtest wish"
									  || w.Name == "Removetest wish"
									  || w.Name == "Update test wish" || w.Name == "Updated test wish"
									  || w.Name.StartsWith( "Lorem ipsum" )
									  select w;
				db.Wishes.DeleteAllOnSubmit( removeWishQuery );

				//Remove user that was created in SqlRepository_Can_Create_User
				var removeUserQuery = from u in db.Users where u.Name == "CreateTestUser" select u;
				db.Users.DeleteAllOnSubmit( removeUserQuery );
				db.SubmitChanges();
			}
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
			int userId = 1;
			IList<Wish> shoppinglist = wishService.GetShoppingList( userId );
			Assert.IsNotNull( shoppinglist, "Shoppinglist was null" );
		}

		[TestMethod]
		public void SqlRepository_Can_Add_Wish()
		{
			//DateTime startTime = DateTime.Now;

			Wish wish = new Wish
			{
				Name = "Addtest wish", //If you change this, change cleanup code.
				Description = "Description of wish",
				Owner = new User() { Id = 1 },
				CalledByUser = new User() { Id = 2 },
				LinkUrl = "http://localhost"
			};

			rep.SaveWish( wish );

			using (SqlRepository.LinqWishListDataContext db = new SqlRepository.LinqWishListDataContext())
			{
				var loadedWishes = (from w in db.Wishes
									where w.Name == wish.Name
									select w).ToList();

				Assert.IsTrue( loadedWishes.Count == 1, "Did not contain 1 wish" );
				Assert.AreEqual( loadedWishes[0].Name, wish.Name, "Wish name was not correct" );
				Assert.AreEqual( loadedWishes[0].Description, wish.Description, "Wish description was not correct" );
				Assert.AreEqual( loadedWishes[0].OwnerId, wish.Owner.Id, "Wish ownerid was not correct" );
				Assert.AreEqual( loadedWishes[0].TjingedById, wish.CalledByUser.Id, "Wish tjingedbyid was not correct" );
				Assert.AreEqual( loadedWishes[0].LinkUrl, wish.LinkUrl, "Wish linkurl was not correct" );
				//Assert.IsTrue( loadedWishes[0].Created >= startTime, string.Format("Created time ({0}) was less than the start time ({1})", loadedWishes[0].Created.Ticks, startTime.Ticks ) );
				Assert.AreEqual( loadedWishes[0].Created, loadedWishes[0].Changed, "Change time was not equal to created time" );

			}
		}

		[TestMethod]
		public void Newly_Created_Wish_Will_Have_A_Change_Date()
		{
			Wish wish = new Wish { Owner = new User { Id = 1 }, Name = "Change date test wish", Description = "Whatever..." };
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
				Owner = new User { Id = 1 }
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
				Name = "Removetest wish", //If you change this, change cleanup code.
				Description = "Description of wish",
				Owner = new User() { Id = 1 },
				CalledByUser = new User() { Id = 2 },
				LinkUrl = "http://localhost"
			};
			int numberOfWishes = rep.GetWishes().Count<Wish>();
			Wish savedWish = rep.SaveWish( wish );

			Assert.AreEqual<int>( numberOfWishes + 1, rep.GetWishes().Count<Wish>() );
			rep.RemoveWish( savedWish );
			Assert.AreEqual<int>( numberOfWishes, rep.GetWishes().Count<Wish>(), "Number of wishes in repository did not decrease" );

			using (WishList.SqlRepository.LinqWishListDataContext db = new WishList.SqlRepository.LinqWishListDataContext())
			{
				WishList.SqlRepository.Data.Wish loadedWish = (from w in db.Wishes
															   where w.WishId == savedWish.Id
															   select w).SingleOrDefault<WishList.SqlRepository.Data.Wish>();

				Assert.IsNull( loadedWish, "Removed wish was still found in repository" );
			}
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
				Owner = new User() { Id = 1 },
				CalledByUser = null
			};
			Wish wishToUpdate = rep.SaveWish( wish );

			//Do the update 
			wishToUpdate.Name = "Updated test wish";
			wishToUpdate.Description = "Updated description";
			wishToUpdate.LinkUrl = "http://isupdated.now";
			wishToUpdate.CalledByUser = new User() { Id = 2 };
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
				Owner = new User() { Id = 1 },
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
				Owner = new User() { Id = 1 },
				CalledByUser = null
			};
			wish = rep.SaveWish( wish );
			DateTime lastChanged = wish.Changed.Value;

			wish.CalledByUser = new User { Id = 1 };
			rep.SaveWish( wish );

			var updatedWish = rep.GetWishes().WithId( wish.Id );

			Assert.IsTrue( lastChanged.IsAlmostEqualTo( updatedWish.Changed.Value ), "Calling a wish updated the change date" );
		}
	}
}
