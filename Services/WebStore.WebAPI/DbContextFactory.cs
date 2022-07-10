using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WebStore.DAL.Context;

namespace WebStore.WebAPI;

public class DbContextFactory : IDesignTimeDbContextFactory<WebStoreDB>
{
    WebStoreDB IDesignTimeDbContextFactory<WebStoreDB>.CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<WebStoreDB>();
        var connectionString = configuration.GetConnectionString("SqlServer");

        builder.UseSqlServer(connectionString);

        return new WebStoreDB(builder.Options);
    }
}