using Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Meltix.IntegrationTests
{
    public static class DbContextProvider
    {
        public static MeltixContext SetupContext(SqliteConnection? connection = null)
        {
            connection ??= new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<MeltixContext>()
                .UseSqlite(connection)
                .Options;

            var context = new MeltixContext(options);

            context.Database.EnsureCreated();

            return context;
        }
    }
}
