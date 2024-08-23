using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adiciona o serviço do DbContext na memória
builder.Services.AddDbContext<RouteContext>(options => options.UseInMemoryDatabase("RoutesDb"));

// Adiciona o serviço RouteService
builder.Services.AddScoped<RouteService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "API de Rota de Viagem");

// Endpoint para obter todas as rotas
app.MapGet("/routes", async (RouteContext context) =>
{
    return await context.Routes.ToListAsync();
});

// Endpoint para obter uma rota por ID
app.MapGet("/routes/{id:int}", async (int id, RouteContext context) =>
{
    var route = await context.Routes.FindAsync(id);
    return route is not null ? Results.Ok(route) : Results.NotFound();
});

// Endpoint para adicionar uma nova rota
app.MapPost("/routes", async (RouteRequest route, RouteContext context) =>
{
    context.Routes.Add(route);
    await context.SaveChangesAsync();
    return Results.Created($"/routes/{route.Id}", route);
});

// Endpoint para atualizar uma rota existente
app.MapPut("/routes/{id:int}", async (int id, RouteRequest route, RouteContext context) =>
{
    var existingRoute = await context.Routes.FindAsync(id);
    if (existingRoute is null)
    {
        return Results.NotFound();
    }

    existingRoute.Origin = route.Origin;
    existingRoute.Destination = route.Destination;
    existingRoute.Price = route.Price;

    await context.SaveChangesAsync();
    return Results.Ok(existingRoute);
});

// Endpoint para deletar uma rota
app.MapDelete("/routes/{id:int}", async (int id, RouteContext context) =>
{
    var route = await context.Routes.FindAsync(id);
    if (route is null)
    {
        return Results.NotFound();
    }

    context.Routes.Remove(route);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

// Endpoint para calcular a rota de menor custo
app.MapGet("/routes/cheapest/{origin}/{destination}", async (string origin, string destination, RouteService routeService) =>
{
    var result = routeService.FindCheapestRoute(origin, destination);
    return result is not null ? Results.Ok(result) : Results.NotFound("Rota não encontrada");
});

app.Run();