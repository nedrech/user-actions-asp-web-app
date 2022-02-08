using Microsoft.EntityFrameworkCore;

namespace Nedrech.WebApp.Models;

public static class IdentityData
{
    public static async void EnsureMigrated(IApplicationBuilder app) {

        AppIdentityDbContext context = app.ApplicationServices
            .CreateScope().ServiceProvider
            .GetRequiredService<AppIdentityDbContext>();
        if ((await context.Database.GetPendingMigrationsAsync()).Any()) {
            await context.Database.MigrateAsync();
        }
    }
}