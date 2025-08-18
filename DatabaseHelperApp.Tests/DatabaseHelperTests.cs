using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseHelperApp;
using System.Data;

namespace DatabaseHelperApp.Tests
{
    [TestClass]
    public class DatabaseHelperTests
    {
        [TestMethod]
        public void TestSqliteDatabase()
        {
            // Use an in-memory SQLite database for this test
            var providerName = "Microsoft.Data.Sqlite";
            var connectionString = "Data Source=:memory:";

            var dbHelper = new DatabaseHelper(providerName, connectionString);

            using (var connection = dbHelper.CreateConnection())
            {
                connection.Open();

                // 1. Create a table
                var createTableSql = "CREATE TABLE Users (Id INTEGER PRIMARY KEY, Name TEXT, Email TEXT)";
                dbHelper.ExecuteNonQuery(createTableSql, CommandType.Text);

                // 2. Insert data
                var insertSql = "INSERT INTO Users (Name, Email) VALUES (@Name, @Email)";
                dbHelper.ExecuteNonQuery(insertSql, CommandType.Text,
                    dbHelper.CreateParameter("@Name", "Alice"),
                    dbHelper.CreateParameter("@Email", "alice@example.com"));
                dbHelper.ExecuteNonQuery(insertSql, CommandType.Text,
                    dbHelper.CreateParameter("@Name", "Bob"),
                    dbHelper.CreateParameter("@Email", "bob@example.com"));

                // 3. Retrieve data using GetDataTable and assert
                var selectSql = "SELECT * FROM Users";
                var dataTable = dbHelper.GetDataTable(selectSql, CommandType.Text);
                Assert.AreEqual(2, dataTable.Rows.Count);
                Assert.AreEqual("Alice", dataTable.Rows[0]["Name"]);
                Assert.AreEqual("Bob", dataTable.Rows[1]["Name"]);

                // 4. Retrieve a single value using ExecuteScalar and assert
                var countSql = "SELECT COUNT(*) FROM Users";
                var userCount = dbHelper.ExecuteScalar(countSql, CommandType.Text);
                Assert.AreEqual(2L, userCount); // SQLite returns a long for COUNT
            }
        }
    }
}
