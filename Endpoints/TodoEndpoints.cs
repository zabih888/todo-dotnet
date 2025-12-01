using TodoApi.Dtos;
using TodoApi.Services;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/todos");

        group.MapGet("/", async (TodoService service) =>
        {
            return await service.GetAllAsync();
        });

        group.MapGet("/{id}", async (int id, TodoService service) =>
        {
            var todo = await service.GetTodoAsync(id);
            return todo is null ? Results.NotFound() : Results.Ok(todo);
        });

        group.MapGet("/completed", async (TodoService service) =>
        {
            return await service.GetAllCompletedAsync();
        });

        group.MapGet("/pending", async (TodoService service) =>
        {
            return await service.GetAllPendingAsync();
        });

        group.MapPost("/", async (TodoCreateDto dto, TodoService service) =>
        {
            var todo = await service.CreateAsync(dto);
            return Results.Created($"/todos/{todo.Id}", todo);
        });

        group.MapPut("/{id}", async (int id, TodoUpdateDto dto, TodoService service) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return Results.BadRequest("Title cannot be empty");

            var updated = await service.UpdateAsync(id, dto);
            return updated is not null ? Results.Ok(updated) : Results.NotFound();
        });

        group.MapDelete("/{id}", async (int id, TodoService service) =>
        {
            return await service.DeleteAsync(id) 
                ? Results.NoContent() 
                : Results.NotFound();
        });
    }
}
