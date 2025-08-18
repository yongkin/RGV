using System;
using System.Data;
using DatabaseHelperApp;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("DatabaseHelper Example");
        Console.WriteLine("======================");

        // Use an in-memory SQLite database for this example
        var providerName = "Microsoft.Data.Sqlite";
        var connectionString = "Data Source=:memory:";

        // The static constructor of DatabaseHelper will register the providers.
        // We just need to create an instance.
        var dbHelper = new DatabaseHelper(providerName, connectionString);

        // We need to open and hold the connection for in-memory databases
        using (var connection = dbHelper.CreateConnection())
        {
            connection.Open();

            // 1. Create a table
            Console.WriteLine("\n1. Creating 'Users' table...");
            var createTableSql = "CREATE TABLE Users (Id INTEGER PRIMARY KEY, Name TEXT, Email TEXT)";
            dbHelper.ExecuteNonQuery(createTableSql, CommandType.Text);
            Console.WriteLine("'Users' table created successfully.");

            // 2. Insert data
            Console.WriteLine("\n2. Inserting data into 'Users' table...");
            var insertSql = "INSERT INTO Users (Name, Email) VALUES (@Name, @Email)";
            dbHelper.ExecuteNonQuery(insertSql, CommandType.Text,
                dbHelper.CreateParameter("@Name", "Alice"),
                dbHelper.CreateParameter("@Email", "alice@example.com"));
            dbHelper.ExecuteNonQuery(insertSql, CommandType.Text,
                dbHelper.CreateParameter("@Name", "Bob"),
                dbHelper.CreateParameter("@Email", "bob@example.com"));
            Console.WriteLine("Data inserted successfully.");

            // 3. Retrieve data using GetDataTable
            Console.WriteLine("\n3. Retrieving data using GetDataTable...");
            var selectSql = "SELECT * FROM Users";
            var dataTable = dbHelper.GetDataTable(selectSql, CommandType.Text);
            foreach (DataRow row in dataTable.Rows)
            {
                Console.WriteLine($"   Id: {row["Id"]}, Name: {row["Name"]}, Email: {row["Email"]}");
            }

            // 4. Retrieve a single value using ExecuteScalar
            Console.WriteLine("\n4. Retrieving user count using ExecuteScalar...");
            var countSql = "SELECT COUNT(*) FROM Users";
            var userCount = dbHelper.ExecuteScalar(countSql, CommandType.Text);
            Console.WriteLine($"   User count: {userCount}");

            // 5. Retrieve data using ExecuteReader
            Console.WriteLine("\n5. Retrieving data using ExecuteReader...");
            using (var reader = dbHelper.ExecuteReader(selectSql, CommandType.Text))
            {
                while (reader.Read())
                {
                    Console.WriteLine($"   Id: {reader["Id"]}, Name: {reader["Name"]}, Email: {reader["Email"]}");
                }
            }
        }
    }
}
