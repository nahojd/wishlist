using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace WishList.Data
{
	public class Wish : ICloneable
	{
		private User _calledByUser;

		public int Id { get; set; }

		[Required( ErrorMessage = "Du måste ange ett namn" )]
		[StringLength( 100, ErrorMessage = "Namnet får vara max 100 tecken." )]
		public string Name { get; set; }

		[Required( ErrorMessage = "Du måste ange en beskrivning" )]
		[StringLength( 500, ErrorMessage = "Beskrivningen får vara max 500 tecken." )]
		public string Description { get; set; }

		[StringLength( 255, ErrorMessage = "Länken får vara max 255 tecken." )]
		public string LinkUrl { get; set; }

		public User Owner { get; set; }

		public User CalledByUser
		{
			get
			{
				return _calledByUser;
			}
			set
			{
				if (value != null && _calledByUser != null)
				{
					throw new Exception( "Wish is already called!" );
				}
				_calledByUser = value;
			}
		}

		public DateTime Created { get; internal set; }

		public DateTime? Changed { get; internal set; }

		public bool IsCalled
		{
			get
			{
				return _calledByUser != null;
			}
		}

		/// <summary>
		/// Find out if caller differs from the caller of this wish
		/// </summary>
		/// <param name="calledById">Id of the caller to compare with, or null if it is not called</param>
		/// <returns>true if the caller was changed, false otherwise</returns>
		public bool CalledDiffers( int? calledById )
		{
			bool calledByWasChanged = (this.IsCalled ? this.CalledByUser.Id : -1) != (calledById ?? -1);
			return calledByWasChanged;
		}

		#region ICloneable Members

		public object Clone()
		{
			return new Wish()
			{
				Id = this.Id,
				Name = this.Name,
				Description = this.Description,
				LinkUrl = this.LinkUrl,
				Owner = this.Owner,
				CalledByUser = this.CalledByUser,
				Changed = this.Changed,
				Created = this.Created
			};
		}

		#endregion
	}
}
