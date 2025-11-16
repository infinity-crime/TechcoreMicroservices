using GrpcService2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddGrpcClient<GrpcService1.Greeter.GreeterClient>(opt =>
{
    opt.Address = new Uri("https://localhost:7136");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();

app.MapGet("/test", async (GrpcService1.Greeter.GreeterClient client) =>
{
    var response = await client.SayHelloAsync(new GrpcService1.HelloRequest
    {
        Name = "Service B"
    });

    return Results.Ok(response.Message);
});

app.Run();
