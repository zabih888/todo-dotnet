using FluentValidation;
using Microsoft.EntityFrameworkCore;

using TodoApi.Dtos;
using TodoApi.Models;
using TodoApi.Services;
using TodoApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// PostgreSQL + EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Fix timestamp behavior for PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// TodoService (must be scoped, not singleton)
builder.Services.AddScoped<TodoService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// ------------------- ENDPOINTS ---------------------

app.MapPost("/signup", async (SignupDto dto, UserService users, JwtService jwt) =>
{
   var user = await users.CreateUser(dto.UserName,dto.Password);
   var token = jwt.CreateToken(user);
   return Results.Ok(new {token}); 
});

app.MapPost("/login", async(LoginDto dto, UserService users, JwtService jwt) =>
{
    var user = await users.ValidateUser(dto.UserName, dto.Password);
    if(user == null) return Results.Unauthorized();

    var token = jwt.CreateToken(user);
    return Results.Ok(new {token});
});

app.MapGet("/todos", async (TodoService service) =>
{
    return await service.GetAllAsync();
});

app.MapGet("/todos/{id}", async (int id, TodoService service) =>
{
    var todo = await service.GetTodoAsync(id);
    return todo is null ? Results.NotFound() : Results.Ok(todo);
});

app.MapGet("/todos/completed", async (TodoService service) =>
{
    return await service.GetAllCompletedAsync();
});

app.MapGet("/todos/pending", async (TodoService service) =>
{
    return await service.GetAllPendingAsync();
});

app.MapPost("/todos", async (TodoCreateDto dto, TodoService service) =>
{
    var todo = await service.CreateAsync(dto);
    return Results.Created($"/todos/{todo.Id}", todo);
});

app.MapPut("/todos/{id}", async (int id, TodoUpdateDto dto, TodoService service) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title))
        return Results.BadRequest("Title cannot be empty");

    var updated = await service.UpdateAsync(id, dto);
    return updated is not null ? Results.Ok(updated) : Results.NotFound();
});

app.MapDelete("/todos/{id}", async (int id, TodoService service) =>
{
    return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
});

app.Run();
