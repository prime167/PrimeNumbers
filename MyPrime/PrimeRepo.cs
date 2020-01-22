using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;

namespace MyPrime
{
    public static class PrimeRepo
    {
        public static string DbFile => Environment.CurrentDirectory + "\\Prime.db";

        public static SQLiteConnection Conn()
        {
            return new SQLiteConnection($@"Data Source='{DbFile}'");
        }

        public static IEnumerable<Prime> GetAll()
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            using (var cnn = Conn())
            {
                cnn.Open();
                return cnn.Query<Prime>("select * from prime");
            }
        }

        public static long Insert(Prime prime)
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            using (var cnn = Conn())
            {
                cnn.Open();
                return cnn.Insert<Prime>(prime);
            }
        }

        public static long Insert(List<Prime> primes)
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            using (var cnn = Conn())
            {
                cnn.Open();
                return cnn.Insert(primes);
            }
        }

        private static void CreateDatabase()
        {
            using (var cnn = Conn())
            {
                cnn.Open();
                cnn.Execute(
                    @"CREATE TABLE Prime ([Id] integer PRIMARY KEY NOT NULL,[Seq] integer NOT NULL,[PrimeNumber] nvarchar(1000) NOT NULL);");
            }
        }
    }
}
