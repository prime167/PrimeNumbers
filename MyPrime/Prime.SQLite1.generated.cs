//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/t4models).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;

using LinqToDB;
using LinqToDB.Mapping;

namespace DataModel
{
	/// <summary>
	/// Database       : Prime
	/// Data Source    : Prime
	/// Server Version : 3.7.17
	/// </summary>
	public partial class PrimeDB : LinqToDB.Data.DataConnection
	{
		public ITable<Prime> Primes { get { return this.GetTable<Prime>(); } }

		public PrimeDB()
		{
		}

		public PrimeDB(string configuration)
			: base(configuration)
		{
		}
	}

	[Table("Prime")]
	public partial class Prime
	{
		[Column, NotNull] public long   Id          { get; set; } // integer
		[Column, NotNull] public string PrimeNumber { get; set; } // nvarchar(1000)
	}
}
