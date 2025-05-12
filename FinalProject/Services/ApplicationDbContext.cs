using FinalProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Services;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }

    public DbSet<Product> Products { get; set; }
}
