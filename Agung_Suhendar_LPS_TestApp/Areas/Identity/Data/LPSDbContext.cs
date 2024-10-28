using Agung_Suhendar_LPS_TestApp.Areas.Identity.Data;
using Agung_Suhendar_LPS_TestApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Agung_Suhendar_LPS_TestApp.Data;

public class LPSDbContext : IdentityDbContext<ApplicationUser>
{
    public LPSDbContext(DbContextOptions<LPSDbContext> options)
        : base(options)
    {
    }

    public DbSet<Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
