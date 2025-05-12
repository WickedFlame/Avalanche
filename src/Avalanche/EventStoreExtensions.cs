using Avalanche.WriteModel.Sqlite;
using System.Data.SQLite;

namespace Avalanche
{
    public static class EventStoreExtensions
    {
        public static void UseSqliteEventStore(this IApplicationBuilder app)
        {
            const string _query = @"
CREATE TABLE IF NOT EXISTS Events (
  Id VARCHAR(255),
  TestId VARCHAR(255),
  Time DATETIME,
  EventType VARCHAR(500),
  Value VARCHAR (2000)
);
";
            using (var connection = new SQLiteConnection(Constants.EventStoreDatabase))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = _query;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UseSqliteReadModel(this IApplicationBuilder app)
        {
            const string _query = @"
CREATE TABLE IF NOT EXISTS TestRun (
  TestId VARCHAR(255),
  Scenario VARCHAR(255),
  StartTime DATETIME,
  EndTime DATETIME,
  Status VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS RampupEvents (
  Id VARCHAR(255),
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  Name VARCHAR(255),
  Time DATETIME,
  ThreadId VARCHAR(255),
  Value INT
);

CREATE TABLE IF NOT EXISTS TestRunDetail (
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  TestCase VARCHAR(255),
  ThreadId VARCHAR(255),
  Throughput REAL,
  Iterations BIGINT
);

CREATE TABLE IF NOT EXISTS IterationEvents (
  Id VARCHAR(255),
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  Time DATETIME,
  ThreadId INT,
  TestName VARCHAR(255),
  Throughput REAL,
  AverageMilliseconds REAL
);


CREATE TABLE IF NOT EXISTS SummaryEvents (
  Id VARCHAR(255),
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  TestCase VARCHAR(255),
  Type VARCHAR(100),
  ThreadNumber VARCHAR(255),
  Iterations INT,
  AverageMilliseconds REAL,
  TotalMilliseconds REAL,
  Throughput REAL
);
";
            using (var connection = new SQLiteConnection(Constants.ReadModelDatabase))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = _query;

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    //protected void CreateTables()
    //    {
    //        const string sqlTextCreateTables = @"
    //    CREATE TABLE IF NOT EXISTS Addresses
    //    (
    //        Id INTEGER PRIMARY KEY NOT NULL,
    //        Street TEXT NOT NULL,
    //        Number INTEGER NOT NULL,
    //        Ext TEXT,
    //        ExtraLine TEXT,
    //        PostalCode TEXT NOT NULL,
    //        City TEXT NOT NULL,
    //        Country TEXT NOT NULL
    //    );
    //    CREATE INDEX IF NOT EXISTS indexAddresses ON Addresses (PostalCode, Number, Ext);

    //    CREATE TABLE IF NOT EXISTS Schools
    //    (
    //       Id INTEGER PRIMARY KEY NOT NULL,
    //       Name TEXT NOT NULL
    //    );

    //    CREATE TABLE IF NOT EXISTS Students
    //    (
    //        Id INTEGER PRIMARY KEY NOT NULL,
    //        Name TEXT NOT NULL,
    //        AddressId INTEGER NOT NULL,
    //        SchoolId INTEGER NOT NULL,

    //        FOREIGN KEY(AddressId) REFERENCES Addresses(Id)  ON DELETE NO ACTION,
    //        FOREIGN KEY(SchoolId) REFERENCES Schools(Id) ON DELETE CASCADE
    //    );

    //    CREATE TABLE IF NOT EXISTS Teachers
    //    (
    //        Id INTEGER PRIMARY KEY NOT NULL,
    //        Name TEXT NOT NULL,

    //        AddressId INTEGER NOT NULL,
    //        SchoolId INTEGER NOT NULL,

    //        FOREIGN KEY(AddressId) REFERENCES Addresses(Id)  ON DELETE NO ACTION,
    //        FOREIGN KEY(SchoolId) REFERENCES Schools(Id) ON DELETE CASCADE
    //    );

    //    CREATE TABLE IF NOT EXISTS TeachersStudents
    //    (
    //        TeacherId INTEGER NOT NULL,
    //        StudentId INTEGER NOT NULL,

    //        PRIMARY KEY (TeacherId, StudentId)
    //        FOREIGN KEY(TeacherId) REFERENCES Teachers(Id) ON DELETE NO ACTION,
    //        FOREIGN KEY(StudentId) REFERENCES Students(Id) ON DELETE NO ACTION
    //    )";

    //        var connectionString = this.Database.Connection.ConnectionString;
    //        using (var dbConnection = new System.Data.SQLite.SQLiteConnection(connectionString))
    //        {
    //            dbConnection.Open();
    //            using (var dbCommand = dbConnection.CreateCommand())
    //            {
    //                dbCommand.CommandText = sqlTextCreateTables;
    //                dbCommand.ExecuteNonQuery();
    //            }
    //        }
    //    }
    }
