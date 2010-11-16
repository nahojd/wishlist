using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WishList.Data;
using WishList.Data.DataAccess;
using WishList.Data.Filters;
using WishList.Services;

namespace WishList.Tests
{
    /// <summary>
    /// Summary description for WishTests
    /// </summary>
    [TestClass]
    public class WishServiceTests
    {
        private WishService Service
        {
            get;
            set;
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

        [TestInitialize]
        public void Setup()
        {
			IWishListRepository rep = new TestWishListRepository();
			Service = new WishService( rep, new TestMailService() );
        }


        #region Wish tests
        [TestMethod]
        public void Can_Call_Wish()
        {
            Wish wish = new Wish();
            Assert.IsFalse(wish.IsCalled, "Newly created Wish was called");

            User user = new User() { Id = 1 };

            wish.CalledByUser = user;
            Assert.IsTrue(wish.IsCalled, "Wish was not called");
            Assert.AreEqual<User>(wish.CalledByUser, user, "User and CalledByUser was not equal!");
        }
        #endregion

        #region Repository tests

        [TestMethod]
        public void Repository_Repository_IsNotNull()
        {
            IWishListRepository rep = new TestWishListRepository();
            Assert.IsNotNull(rep.GetUsers(), "Users in repository was null");
            Assert.IsNotNull(rep.GetWishes(), "Wishes in repository was null");
        }

        [TestMethod]
        public void Repository_Has_ForUser_Filter_For_Wishes()
        {
            IWishListRepository rep = new TestWishListRepository();
            IList<Wish> wishes = rep.GetWishes()
                .ForUser(1)
                .ToList();

            Assert.IsNotNull(wishes);
        }

        [TestMethod]
        public void Repository_WishFilter_Returns_5_Wishes_For_User_1()
        {
            IWishListRepository rep = new TestWishListRepository();

            IList<Wish> wishes = rep.GetWishes().ForUser(1).ToList();

            Assert.AreEqual(5, wishes.Count);
        }

        #endregion

        #region Service tests


        [TestMethod]
        public void WishList_Can_Load_Wishes_For_User_By_Id()
        {
            int ownerId = 1;
            WishList.Data.WishList list = Service.GetWishList(ownerId);

            Assert.AreEqual<int>(ownerId, list.UserId, "Wrong user id in loadedWish list");
            Assert.AreEqual<int>(5, list.Wishes.Count, "Number of wishes for user 1 was wrong");
        }

        [TestMethod]
        public void WishList_Can_Load_Wishes_For_User_By_Name()
        {
            string userName = "User 1";
            WishList.Data.WishList list = Service.GetWishList(userName);

            Assert.AreEqual<int>(1, list.UserId, "Wrong user id in loaded Wishlist");
            Assert.AreEqual<int>(5, list.Wishes.Count, "Number of wishes for user 1 was wrong");
        }

        [TestMethod]
        public void WishList_Can_Add_Wish_For_User()
        {
            int ownerId = 1;
            WishList.Data.WishList list = Service.GetWishList(ownerId);

            int numberOfWishes = list.Wishes.Count;
            Wish newWish = new Wish()
            {
                Owner = new User() { Id = ownerId },
                Name = "Testönskning",
                Description = "Lorem ipsum dolor sit amet"
            };
            Service.SaveWish(newWish, true);
            list = Service.GetWishList(ownerId);

            Assert.AreEqual<int>(numberOfWishes + 1, list.Wishes.Count, "Number of wishes did not increase by one");

            Wish loadedWish = (from w in list.Wishes where w.Name == newWish.Name select w).SingleOrDefault<Wish>();
            Assert.IsNotNull(loadedWish, "Wish was null");
            Assert.AreEqual<string>(newWish.Description, loadedWish.Description, "Description did not match");
        }

        [TestMethod]
        public void Newly_Created_Wish_Will_Have_A_Change_Date()
        {
            Wish newWish = new Wish { Owner = new User { Id = 1 }, Name = "Change date test wish", Description = "Whatever..." };
            Service.SaveWish(newWish, true);

            Assert.IsNotNull(newWish.Changed, "Changed date was null on newly created wish");
        }

        [TestMethod]
        public void WishList_Can_Remove_Wish_For_User()
        {
            int ownerId = 1;
            IWishListRepository repository = new TestWishListRepository();
            WishList.Data.WishList list = Service.GetWishList(ownerId);

            int numberOfWishes = list.Wishes.Count;
            Wish wishToBeRemoved = list.Wishes[0];

            Service.RemoveWish(wishToBeRemoved);
            list = Service.GetWishList(ownerId);

            Assert.AreEqual<int>(numberOfWishes - 1, list.Wishes.Count, "Number of wishes did not decrease by one when wish list was reloaded");
        }

        [TestMethod]
        public void WishService_Can_Get_Wish_1()
        {
            int wishId = 1;
            WishList.Data.Wish wish = Service.GetWish(wishId);

            Assert.IsNotNull(wish, "Wish was null");
            Assert.AreEqual<int>(wishId, wish.Id, "Wish id did not match");
            Assert.AreEqual<int>(1, wish.Owner.Id, "Owner.Id was not 1");
        }

        [TestMethod]
        public void WishService_Can_Update_Wish()
        {
            int wishId = 1;
            WishList.Data.Wish wish = Service.GetWish(wishId);

            Assert.AreEqual<int>(wishId, wish.Id, "Wish id did not match");

            wish.Name = "Wish1 - Updated";
            wish.Description = "Updated description";
            wish.LinkUrl = "http://updatedlink.com";

            Service.SaveWish(wish, false);

            WishList.Data.Wish updatedWish = Service.GetWish(wishId);

            Assert.IsNotNull(updatedWish, "Updated wish was null");
            Assert.AreEqual<string>(wish.Name, updatedWish.Name, "Name did not match");
            Assert.AreEqual<string>(wish.Description, updatedWish.Description, "Description did not match");
            Assert.AreEqual<string>(wish.LinkUrl, updatedWish.LinkUrl, "LinkUrl did not match");
        }

        [TestMethod]
        public void Updating_Wish_Will_Update_ChangeDate()
        {
            var wish = Service.GetWish(2);
            DateTime lastChanged = wish.Changed.Value;

            Service.SaveWish(wish, false);
            var updatedWish = Service.GetWish(2);

            Assert.IsTrue(updatedWish.Changed > lastChanged, "Change date was not updated on save");
        }

        [TestMethod]
        public void Calling_Wish_Will_Not_Update_ChangeDate()
        {
            var wish = Service.GetWish(2);
            DateTime lastChanged = wish.Changed.Value;

            wish.CalledByUser = new User { Id = 1 };
            Service.SaveWish(wish, true);
            var updatedWish = Service.GetWish(2);

            Assert.AreEqual(lastChanged, updatedWish.Changed.Value, "Calling a wish updated the change date");
        }

        [TestMethod]
        public void WishService_Can_Get_ShoppingList_For_User()
        {
            int userId = 1;
            IList<Wish> shoppinglist = Service.GetShoppingList(userId);
            Assert.AreEqual<int>(4, shoppinglist.Count, "Wrong number of wishes in shoppinglist");
        }

        [TestMethod]
        public void WishService_Can_Get_LatestActivityList_For_User()
        {
            int userId = 1;
            var wishes = Service.GetLatestActivityList(userId);

            Assert.IsNotNull(wishes, "Latest wishes was null");
            Assert.AreEqual(10, wishes.Count, "Wish count was not 10");
        }



        #endregion
    }
}
