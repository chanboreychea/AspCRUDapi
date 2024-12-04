using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using MinimalApiApp.Data;
using MinimalApiApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PizzaStore API", Description = "Making the Pizzas you love", Version = "v1" });
});

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
    });
}

app.MapGet("/", () => "Welcome to ASP.NET Core Minimal API!");

// Get all products
app.MapGet("/products", async (AppDbContext db) => await db.Products.ToListAsync());

// Get a product by ID
app.MapGet("/products/{id}", async (AppDbContext db, int id) =>
    await db.Products.FindAsync(id) is Product product ? Results.Ok(product) : Results.NotFound());

// Add a new product
app.MapPost("/products", async (AppDbContext db, Product product) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

// Update a product
app.MapPut("/products/{id}", async (AppDbContext db, int id, Product updatedProduct) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    product.Name = updatedProduct.Name;
    product.Price = updatedProduct.Price;
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// Delete a product
app.MapDelete("/products/{id}", async (AppDbContext db, int id) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapControllers();

app.Run();
