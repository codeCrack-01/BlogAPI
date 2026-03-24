using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Data;

public class BlogDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
}