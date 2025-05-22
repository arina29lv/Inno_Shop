using Microsoft.EntityFrameworkCore;
using UserControl.Domain.Models;

namespace UserControl.Infrastructure.Persistence;

public class UserDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options){}
    
}
