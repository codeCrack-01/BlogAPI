using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using MiniValidation;

var builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("Post") ?? "DataSource=Post.db";
builder.Services.AddSqlite<BlogDbContext>(connString);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.MapGet("/api/hello", () => Results.Ok(new { message = "Hello, world!" }));
app.MapGet("/api/hello/{name}", (string name) => Results.Ok(new { message = $"Hello, {name}!" }));

// THE POST SECTION
app.MapGet(("/api/posts"), async (BlogDbContext dbContext) =>
{
    var posts = await dbContext.Posts.ToListAsync();
    return Results.Ok(posts);
});
app.MapGet("/api/posts/{id}", async (BlogDbContext dbContext, int id) => {
    var post = await dbContext.Posts.FindAsync(id);
    return post is not null ? Results.Ok(post) : Results.NotFound();
});
app.MapPost("/api/posts", async (BlogDbContext dbContext, CreatePostDto dto) =>
{
    if (!MiniValidator.TryValidate(dto, out var errors))
        return Results.ValidationProblem(errors);
    
    var post = new Post
    {
        Title = dto.Title,
        Content = dto.Content
    };
    await dbContext.Posts.AddAsync(post);
    await dbContext.SaveChangesAsync();
    
    return Results.Created($"/api/posts/{post.Id}", post);
});

// THE USER SECTION
app.MapPost("/api/auth/register", async (BlogDbContext db, RegisterDto dto) =>
{
    bool exists = await db.Users.AnyAsync(user => user.Email == dto.Email);
    if (exists)
        return Results.Conflict();

    var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

    var user = new User
    {
        Email = dto.Email,
        PasswordHash = hash
    };
    await db.Users.AddAsync(user);
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/auth/users/{user.Id}", user.Id);
});
app.MapPost("/api/auth/login", async (BlogDbContext db, LoginDto dto) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
    if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        return Results.Unauthorized();

    return Results.Ok(new { token = "jwt_coming_soon" });
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}