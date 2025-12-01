using TodoApi.Dtos;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/signup", async (SignupDto dto, UserService users, JwtService jwt) =>
        {
            var user = await users.CreateUser(dto.UserName, dto.Password);
            var token = jwt.CreateToken(user);
            return Results.Ok(new { token });
        });

        group.MapPost("/login", async (LoginDto dto, UserService users, JwtService jwt) =>
        {
            var user = await users.ValidateUser(dto.UserName, dto.Password);
            if (user == null) return Results.Unauthorized();

            var token = jwt.CreateToken(user);
            return Results.Ok(new { token });
        });
    }
}
