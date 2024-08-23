using Microsoft.EntityFrameworkCore;

public class RouteContext : DbContext
{
    public RouteContext(DbContextOptions<RouteContext> options) : base(options) { }

    public DbSet<RouteRequest> Routes { get; set; }
}
