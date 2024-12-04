using Microsoft.EntityFrameworkCore;
using MinimalApiApp.Models;

namespace MinimalApiApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public required DbSet<Product> Products { get; set; }

    public required DbSet<Stock> Stocks { get; set; }

    public required DbSet<Comment> Comments { get; set; }
}
