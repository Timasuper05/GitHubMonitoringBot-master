using Microsoft.EntityFrameworkCore;

namespace GitHub_Monitoring_Bot;

public class DatabaseInitializer(DataContext dataContext)
{
    public async Task ApplyMigrationsAsync(CancellationToken cancellationToken = default)
    {
        await dataContext.Database.MigrateAsync(cancellationToken);
    }
}
