using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Calls.Infrastructure.Data;

public class CallsDbContextFactory : IDesignTimeDbContextFactory<CallsDbContext>
{
    public CallsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CallsDbContext>();
        
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        
        optionsBuilder.UseNpgsql(connectionString);

        return new CallsDbContext(optionsBuilder.Options);
    }
}

