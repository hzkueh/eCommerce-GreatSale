using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace API.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the DbContextOptions registration
            var optionsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<StoreContext>));
            if (optionsDescriptor != null)
                services.Remove(optionsDescriptor);

            // Remove the options configuration action (holds the UseSqlServer() call)
            var optionConfigs = services
                .Where(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<StoreContext>))
                .ToList();
            foreach (var d in optionConfigs)
                services.Remove(d);

            services.AddDbContext<StoreContext>(options =>
                options.UseInMemoryDatabase("TestDb"));
        });
    }

    public void SeedDatabase(Action<StoreContext> seed)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StoreContext>();
        db.Products.RemoveRange(db.Products);
        db.SaveChanges();
        seed(db);
        db.SaveChanges();
    }
}
