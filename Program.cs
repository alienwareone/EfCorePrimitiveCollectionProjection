using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;



var dbContext = new ApplicationDbContext();
await dbContext.Database.EnsureDeletedAsync();
await dbContext.Database.EnsureCreatedAsync();



var newCategories = new Category[]
{
  new() { Name = "Category 1", Hierarchy = new[] { 1 } },
  new() { Name = "Category 11", Hierarchy = new[] { 1, 10 } },
  new() { Name = "Category 111", Hierarchy = new[] { 1, 10, 100 } },
  new() { Name = "Category 2", Hierarchy = new[] { 2 } },
  new() { Name = "Category 2", Hierarchy = new[] { 2, 20 } },
};
dbContext.Categories.AddRange(newCategories);
await dbContext.SaveChangesAsync();



var categoriesSQL = dbContext.Categories.ToQueryString();
var categories = dbContext.Categories.ToArray();
var categoriesDto = dbContext.Categories
  .Select(x => new CategoryDto
  {
    Id = x.Id,
    Hierarchy = x.Hierarchy,
    Name = x.Name,
    // Message = x.Message,
  })
  .ToArray();

Console.WriteLine(categoriesSQL);
Console.Read();




public class ApplicationDbContext : DbContext
{
  public DbSet<Category> Categories { get; set; } = null!;

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder
      .UseSqlServer("Data Source = .\\SQLEXPRESS; Database = EfCorePrimitiveCollectionProjection; Integrated Security = True; TrustServerCertificate=True")
      .LogTo(Console.WriteLine, LogLevel.Debug)
      .EnableSensitiveDataLogging()
      .EnableDetailedErrors();

    base.OnConfiguring(optionsBuilder);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Category>().OwnsOne(x => x.Message);
    base.OnModelCreating(modelBuilder);
  }
}

public class Category
{
  public int Id { get; private set; }
  public required int[] Hierarchy { get; set; }
  public required string Name { get; set; }
  public Message? Message { get; set; }
  public override string ToString() => $"{Name} - {Hierarchy}";
}

public class Message
{
  public string? Text { get; set; }
}

public class CategoryDto
{
  public int Id { get; set; }
  public required int[] Hierarchy { get; set; }
  public required string Name { get; set; }
  // public Message? Message { get; set; }
}