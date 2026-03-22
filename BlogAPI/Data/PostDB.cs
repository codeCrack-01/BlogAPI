using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Data;

public class PostDB(DbContextOptions options) : DbContext(options)
{
    public DbSet<Post> Posts { get; set; } = null!;
}