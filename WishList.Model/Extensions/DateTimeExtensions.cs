using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WishList.Data.Extensions
{
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Find out if two datetimes are almost exactly the same, 
		/// that is the difference is less than 5 ms.
		/// This is needed due to the fact that the precision in datetimes
		/// in .NET and SQL Server differs.
		/// </summary>
		/// <param name="dateTime1"></param>
		/// <param name="dateTime2"></param>
		/// <returns></returns>
		public static bool IsAlmostEqualTo( this DateTime dateTime1, DateTime dateTime2 )
		{
			var diff = dateTime1 - dateTime2;
			return Math.Abs( diff.TotalMilliseconds ) <= 5;
		}
	}
}
