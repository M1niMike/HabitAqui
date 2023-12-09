using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TP2324.Models;

namespace TP2324.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

  //  public DbSet<Admin> Admins { get; set; }
   // public DbSet<Client> Clients { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Home> Homes { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<TypeResidence> TypeResidences { get; set; }
  //  public DbSet<Contract> Contracts { get; set; }
    public DbSet<Renting> Rentings { get; set; }
    public DbSet<Company> Companies { get; set; }
}

