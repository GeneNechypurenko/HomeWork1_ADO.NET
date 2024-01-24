using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace HomeWork1
{
    internal class Program
    {
        static string connectionString = "Server=localhost;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
        static void Main(string[] args)
        {
            CreateDatabase();

            connectionString = GetConnectionString();

            CreateTable();

            AddMountain(
                new Mountain { Name = "Mount Everest", Height = 8848, Country = "Nepal" },
                new Mountain { Name = "K2", Height = 8611, Country = "Pakistan" },
                new Mountain { Name = "Kangchenjunga", Height = 8586, Country = "Nepal" },
                new Mountain { Name = "Lhotse", Height = 8516, Country = "Nepal" },
                new Mountain { Name = "Makalu", Height = 8485, Country = "Nepal" },
                new Mountain { Name = "Cho Oyu", Height = 8188, Country = "Nepal" },
                new Mountain { Name = "Dhaulagiri", Height = 8167, Country = "Nepal" },
                new Mountain { Name = "Manaslu", Height = 8163, Country = "Nepal" },
                new Mountain { Name = "Nanga Parbat", Height = 8126, Country = "Pakistan" },
                new Mountain { Name = "Annapurna", Height = 8091, Country = "Nepal" }
            );

            DisplayMountains(GetAllMountains());

            DisplayMountain(GetMountainById(1));

            DeleteHighestMountain();

            DisplayMountains(GetAllMountains());

            DeleteHighestMountainByCountry("Nepal");

            DisplayMountains(GetAllMountains());

        }
        static string GetConnectionString()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfiguration config = builder.Build();
            return config.GetConnectionString("DefaultConnection");
        }
        static void ExecuteQuery(string sqlQuery, Action<SqlDataReader> action)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                    action(reader);
            }
        }
        static void CreateDatabase()
        {
            string sqlQuery = "CREATE DATABASE [MountainsDatabase]";
            ExecuteQuery(sqlQuery, action => { });
        }
        static void CreateTable()
        {
            string sqlQuery = @"
        CREATE TABLE [Mountains]
        (
            [Id] INT PRIMARY KEY IDENTITY,
            [Name] NVARCHAR(255),
            [Height] INT,
            [Country] NVARCHAR(255)
        )";

            ExecuteQuery(sqlQuery, action => { });
        }
        static void AddMountain(params Mountain[] mountains)
        {
            var st = new StringBuilder();

            foreach (var mountain in mountains)
                st.Append(mountain.ToStringSql());

            st.Remove(st.Length - 1, 1);

            string sqlQuery = $"INSERT INTO [Mountains] (Name,Height,Country) VALUES ({st})";

            ExecuteQuery(sqlQuery, action => { });
        }

        static List<Mountain> GetAllMountains()
        {
            var mountains = new List<Mountain>();
            string sqlQuery = "SELECT * FROM [Mountains]";

            ExecuteQuery(sqlQuery, reader =>
            {
                while (reader.Read())
                {
                    mountains.Add(new Mountain
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Height = reader.GetInt32(2),
                        Country = reader.GetString(3)
                    });
                }
            });

            return mountains;
        }

        static Mountain GetMountainById(int id)
        {
            Mountain mountain = null;
            string sqlQuery = $"SELECT * FROM [Mountains] WHERE Id = {id}";

            ExecuteQuery(sqlQuery, reader =>
            {
                if (reader.Read())
                {
                    mountain = new Mountain
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Height = reader.GetInt32(2),
                        Country = reader.GetString(3)
                    };
                }
            });

            return mountain;
        }

        static void DeleteHighestMountain()
        {
            string sqlQuery = "DELETE FROM [Mountains] WHERE Id = (SELECT TOP 1 Id FROM [Mountains] ORDER BY Height DESC)";
            ExecuteQuery(sqlQuery, action => { });
        }

        static void DeleteHighestMountainByCountry(string country)
        {
            string sqlQuery = $"DELETE FROM [Mountains] WHERE Id = (SELECT TOP 1 Id FROM [Mountains] WHERE Country = '{country}' ORDER BY Height DESC)";
            ExecuteQuery(sqlQuery, action => { });
        }

        static void DisplayMountain(Mountain mountain)
        {
            if (mountain != null)
            {
                Console.WriteLine($"Id: {mountain.Id}, Name: {mountain.Name}, Height: {mountain.Height}, Country: {mountain.Country}");
            }
            else
            {
                Console.WriteLine("Mountain not found.");
            }
        }

        static void DisplayMountains(List<Mountain> mountains)
        {
            if (mountains.Count > 0)
            {
                foreach (var mountain in mountains)
                {
                    Console.WriteLine($"Id: {mountain.Id}, Name: {mountain.Name}, Height: {mountain.Height}, Country: {mountain.Country}");
                }
            }
            else
            {
                Console.WriteLine("No mountains found.");
            }
        }
    }
}