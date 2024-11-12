using Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Repository;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    public DbSet<Bank> Banks { get; set; }
}

