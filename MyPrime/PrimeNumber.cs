using Dapper.Contrib.Extensions;

namespace MyPrime
{
    [Table("Prime")]
    public class Prime
    {
        [Key] 
        public long Id { get; set; }

        public long Seq { get; set; }

        public string PrimeNumber { get; set; }
    }
}
