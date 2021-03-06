﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WishList.SqlRepository
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="WishList")]
	public partial class LinqWishListDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertWish(WishList.SqlRepository.Data.Wish instance);
    partial void UpdateWish(WishList.SqlRepository.Data.Wish instance);
    partial void DeleteWish(WishList.SqlRepository.Data.Wish instance);
    partial void InsertFriend(WishList.SqlRepository.Data.Friend instance);
    partial void UpdateFriend(WishList.SqlRepository.Data.Friend instance);
    partial void DeleteFriend(WishList.SqlRepository.Data.Friend instance);
    partial void InsertUser(WishList.SqlRepository.Data.User instance);
    partial void UpdateUser(WishList.SqlRepository.Data.User instance);
    partial void DeleteUser(WishList.SqlRepository.Data.User instance);
    #endregion
		
		public LinqWishListDataContext() : 
				base(global::WishList.Data.Properties.Settings.Default.WishListConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public LinqWishListDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LinqWishListDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LinqWishListDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LinqWishListDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<WishList.SqlRepository.Data.Wish> Wishes
		{
			get
			{
				return this.GetTable<WishList.SqlRepository.Data.Wish>();
			}
		}
		
		public System.Data.Linq.Table<WishList.SqlRepository.Data.Friend> Friends
		{
			get
			{
				return this.GetTable<WishList.SqlRepository.Data.Friend>();
			}
		}
		
		public System.Data.Linq.Table<WishList.SqlRepository.Data.User> Users
		{
			get
			{
				return this.GetTable<WishList.SqlRepository.Data.User>();
			}
		}
	}
}
namespace WishList.SqlRepository.Data
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Wish")]
	public partial class Wish : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _WishId;
		
		private string _Name;
		
		private string _Description;
		
		private int _OwnerId;
		
		private System.Nullable<int> _TjingedById;
		
		private string _LinkUrl;
		
		private System.DateTime _Created;
		
		private System.Nullable<System.DateTime> _Changed;
		
		private EntityRef<User> _User;
		
		private EntityRef<User> _User1;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnWishIdChanging(int value);
    partial void OnWishIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnDescriptionChanging(string value);
    partial void OnDescriptionChanged();
    partial void OnOwnerIdChanging(int value);
    partial void OnOwnerIdChanged();
    partial void OnTjingedByIdChanging(System.Nullable<int> value);
    partial void OnTjingedByIdChanged();
    partial void OnLinkUrlChanging(string value);
    partial void OnLinkUrlChanged();
    partial void OnCreatedChanging(System.DateTime value);
    partial void OnCreatedChanged();
    partial void OnChangedChanging(System.Nullable<System.DateTime> value);
    partial void OnChangedChanged();
    #endregion
		
		public Wish()
		{
			this._User = default(EntityRef<User>);
			this._User1 = default(EntityRef<User>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_WishId", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int WishId
		{
			get
			{
				return this._WishId;
			}
			set
			{
				if ((this._WishId != value))
				{
					this.OnWishIdChanging(value);
					this.SendPropertyChanging();
					this._WishId = value;
					this.SendPropertyChanged("WishId");
					this.OnWishIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Description", DbType="NVarChar(500) NOT NULL", CanBeNull=false)]
		public string Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				if ((this._Description != value))
				{
					this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_OwnerId", DbType="Int NOT NULL")]
		public int OwnerId
		{
			get
			{
				return this._OwnerId;
			}
			set
			{
				if ((this._OwnerId != value))
				{
					if (this._User.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnOwnerIdChanging(value);
					this.SendPropertyChanging();
					this._OwnerId = value;
					this.SendPropertyChanged("OwnerId");
					this.OnOwnerIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TjingedById", DbType="Int")]
		public System.Nullable<int> TjingedById
		{
			get
			{
				return this._TjingedById;
			}
			set
			{
				if ((this._TjingedById != value))
				{
					if (this._User1.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnTjingedByIdChanging(value);
					this.SendPropertyChanging();
					this._TjingedById = value;
					this.SendPropertyChanged("TjingedById");
					this.OnTjingedByIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LinkUrl", DbType="NVarChar(255)")]
		public string LinkUrl
		{
			get
			{
				return this._LinkUrl;
			}
			set
			{
				if ((this._LinkUrl != value))
				{
					this.OnLinkUrlChanging(value);
					this.SendPropertyChanging();
					this._LinkUrl = value;
					this.SendPropertyChanged("LinkUrl");
					this.OnLinkUrlChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Created", DbType="DateTime NOT NULL")]
		public System.DateTime Created
		{
			get
			{
				return this._Created;
			}
			set
			{
				if ((this._Created != value))
				{
					this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Changed", DbType="DateTime")]
		public System.Nullable<System.DateTime> Changed
		{
			get
			{
				return this._Changed;
			}
			set
			{
				if ((this._Changed != value))
				{
					this.OnChangedChanging(value);
					this.SendPropertyChanging();
					this._Changed = value;
					this.SendPropertyChanged("Changed");
					this.OnChangedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Wish", Storage="_User", ThisKey="OwnerId", OtherKey="UserId", IsForeignKey=true)]
		public User User
		{
			get
			{
				return this._User.Entity;
			}
			set
			{
				User previousValue = this._User.Entity;
				if (((previousValue != value) 
							|| (this._User.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._User.Entity = null;
						previousValue.Wishes.Remove(this);
					}
					this._User.Entity = value;
					if ((value != null))
					{
						value.Wishes.Add(this);
						this._OwnerId = value.UserId;
					}
					else
					{
						this._OwnerId = default(int);
					}
					this.SendPropertyChanged("User");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Wish1", Storage="_User1", ThisKey="TjingedById", OtherKey="UserId", IsForeignKey=true)]
		public User User1
		{
			get
			{
				return this._User1.Entity;
			}
			set
			{
				User previousValue = this._User1.Entity;
				if (((previousValue != value) 
							|| (this._User1.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._User1.Entity = null;
						previousValue.Wishes1.Remove(this);
					}
					this._User1.Entity = value;
					if ((value != null))
					{
						value.Wishes1.Add(this);
						this._TjingedById = value.UserId;
					}
					else
					{
						this._TjingedById = default(Nullable<int>);
					}
					this.SendPropertyChanged("User1");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Friend")]
	public partial class Friend : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _UserId;
		
		private int _FriendId;
		
		private EntityRef<User> _User;
		
		private EntityRef<User> _User1;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnUserIdChanging(int value);
    partial void OnUserIdChanged();
    partial void OnFriendIdChanging(int value);
    partial void OnFriendIdChanged();
    #endregion
		
		public Friend()
		{
			this._User = default(EntityRef<User>);
			this._User1 = default(EntityRef<User>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UserId", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int UserId
		{
			get
			{
				return this._UserId;
			}
			set
			{
				if ((this._UserId != value))
				{
					if (this._User.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnUserIdChanging(value);
					this.SendPropertyChanging();
					this._UserId = value;
					this.SendPropertyChanged("UserId");
					this.OnUserIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FriendId", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int FriendId
		{
			get
			{
				return this._FriendId;
			}
			set
			{
				if ((this._FriendId != value))
				{
					if (this._User1.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnFriendIdChanging(value);
					this.SendPropertyChanging();
					this._FriendId = value;
					this.SendPropertyChanged("FriendId");
					this.OnFriendIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Friend", Storage="_User", ThisKey="UserId", OtherKey="UserId", IsForeignKey=true)]
		public User User
		{
			get
			{
				return this._User.Entity;
			}
			set
			{
				User previousValue = this._User.Entity;
				if (((previousValue != value) 
							|| (this._User.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._User.Entity = null;
						previousValue.Friends.Remove(this);
					}
					this._User.Entity = value;
					if ((value != null))
					{
						value.Friends.Add(this);
						this._UserId = value.UserId;
					}
					else
					{
						this._UserId = default(int);
					}
					this.SendPropertyChanged("User");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Friend1", Storage="_User1", ThisKey="FriendId", OtherKey="UserId", IsForeignKey=true)]
		public User User1
		{
			get
			{
				return this._User1.Entity;
			}
			set
			{
				User previousValue = this._User1.Entity;
				if (((previousValue != value) 
							|| (this._User1.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._User1.Entity = null;
						previousValue.Friends1.Remove(this);
					}
					this._User1.Entity = value;
					if ((value != null))
					{
						value.Friends1.Add(this);
						this._FriendId = value.UserId;
					}
					else
					{
						this._FriendId = default(int);
					}
					this.SendPropertyChanged("User1");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.[User]")]
	public partial class User : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _UserId;
		
		private string _Email;
		
		private string _Name;
		
		private bool _NotifyOnChange;
		
		private System.Nullable<System.Guid> _ApprovalTicket;
		
		private string _PasswordHash;
		
		private string _Salt;
		
		private EntitySet<Wish> _Wishes;
		
		private EntitySet<Wish> _Wishes1;
		
		private EntitySet<Friend> _Friends;
		
		private EntitySet<Friend> _Friends1;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnUserIdChanging(int value);
    partial void OnUserIdChanged();
    partial void OnEmailChanging(string value);
    partial void OnEmailChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnNotifyOnChangeChanging(bool value);
    partial void OnNotifyOnChangeChanged();
    partial void OnApprovalTicketChanging(System.Nullable<System.Guid> value);
    partial void OnApprovalTicketChanged();
    partial void OnPasswordHashChanging(string value);
    partial void OnPasswordHashChanged();
    partial void OnSaltChanging(string value);
    partial void OnSaltChanged();
    #endregion
		
		public User()
		{
			this._Wishes = new EntitySet<Wish>(new Action<Wish>(this.attach_Wishes), new Action<Wish>(this.detach_Wishes));
			this._Wishes1 = new EntitySet<Wish>(new Action<Wish>(this.attach_Wishes1), new Action<Wish>(this.detach_Wishes1));
			this._Friends = new EntitySet<Friend>(new Action<Friend>(this.attach_Friends), new Action<Friend>(this.detach_Friends));
			this._Friends1 = new EntitySet<Friend>(new Action<Friend>(this.attach_Friends1), new Action<Friend>(this.detach_Friends1));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UserId", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int UserId
		{
			get
			{
				return this._UserId;
			}
			set
			{
				if ((this._UserId != value))
				{
					this.OnUserIdChanging(value);
					this.SendPropertyChanging();
					this._UserId = value;
					this.SendPropertyChanged("UserId");
					this.OnUserIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Email", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				if ((this._Email != value))
				{
					this.OnEmailChanging(value);
					this.SendPropertyChanging();
					this._Email = value;
					this.SendPropertyChanged("Email");
					this.OnEmailChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NotifyOnChange", DbType="Bit NOT NULL")]
		public bool NotifyOnChange
		{
			get
			{
				return this._NotifyOnChange;
			}
			set
			{
				if ((this._NotifyOnChange != value))
				{
					this.OnNotifyOnChangeChanging(value);
					this.SendPropertyChanging();
					this._NotifyOnChange = value;
					this.SendPropertyChanged("NotifyOnChange");
					this.OnNotifyOnChangeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ApprovalTicket", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> ApprovalTicket
		{
			get
			{
				return this._ApprovalTicket;
			}
			set
			{
				if ((this._ApprovalTicket != value))
				{
					this.OnApprovalTicketChanging(value);
					this.SendPropertyChanging();
					this._ApprovalTicket = value;
					this.SendPropertyChanged("ApprovalTicket");
					this.OnApprovalTicketChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PasswordHash", DbType="NVarChar(128)")]
		public string PasswordHash
		{
			get
			{
				return this._PasswordHash;
			}
			set
			{
				if ((this._PasswordHash != value))
				{
					this.OnPasswordHashChanging(value);
					this.SendPropertyChanging();
					this._PasswordHash = value;
					this.SendPropertyChanged("PasswordHash");
					this.OnPasswordHashChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Salt", DbType="NVarChar(128)")]
		public string Salt
		{
			get
			{
				return this._Salt;
			}
			set
			{
				if ((this._Salt != value))
				{
					this.OnSaltChanging(value);
					this.SendPropertyChanging();
					this._Salt = value;
					this.SendPropertyChanged("Salt");
					this.OnSaltChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Wish", Storage="_Wishes", ThisKey="UserId", OtherKey="OwnerId")]
		public EntitySet<Wish> Wishes
		{
			get
			{
				return this._Wishes;
			}
			set
			{
				this._Wishes.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Wish1", Storage="_Wishes1", ThisKey="UserId", OtherKey="TjingedById")]
		public EntitySet<Wish> Wishes1
		{
			get
			{
				return this._Wishes1;
			}
			set
			{
				this._Wishes1.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Friend", Storage="_Friends", ThisKey="UserId", OtherKey="UserId")]
		public EntitySet<Friend> Friends
		{
			get
			{
				return this._Friends;
			}
			set
			{
				this._Friends.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Friend1", Storage="_Friends1", ThisKey="UserId", OtherKey="FriendId")]
		public EntitySet<Friend> Friends1
		{
			get
			{
				return this._Friends1;
			}
			set
			{
				this._Friends1.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Wishes(Wish entity)
		{
			this.SendPropertyChanging();
			entity.User = this;
		}
		
		private void detach_Wishes(Wish entity)
		{
			this.SendPropertyChanging();
			entity.User = null;
		}
		
		private void attach_Wishes1(Wish entity)
		{
			this.SendPropertyChanging();
			entity.User1 = this;
		}
		
		private void detach_Wishes1(Wish entity)
		{
			this.SendPropertyChanging();
			entity.User1 = null;
		}
		
		private void attach_Friends(Friend entity)
		{
			this.SendPropertyChanging();
			entity.User = this;
		}
		
		private void detach_Friends(Friend entity)
		{
			this.SendPropertyChanging();
			entity.User = null;
		}
		
		private void attach_Friends1(Friend entity)
		{
			this.SendPropertyChanging();
			entity.User1 = this;
		}
		
		private void detach_Friends1(Friend entity)
		{
			this.SendPropertyChanging();
			entity.User1 = null;
		}
	}
}
#pragma warning restore 1591
