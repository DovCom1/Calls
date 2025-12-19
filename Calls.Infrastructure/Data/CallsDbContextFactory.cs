using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Calls.Infrastructure.Data;

public class CallsDbContextFactory : IDesignTimeDbContextFactory<CallsDbContext>
{
    public CallsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CallsDbContext>();
        
        // Используем connection string по умолчанию для разработки
        // В продакшене будет использоваться строка из конфигурации
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=CallsDb;Username=postgres;Password=postgres";
        
        optionsBuilder.UseNpgsql(connectionString);

        return new CallsDbContext(optionsBuilder.Options);
    }
}

