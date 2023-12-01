var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//1. Install EntityGuardian NuGet package.PM> Install-Package EntityGuardian

//2. Add the `EntityGuardianInterceptor` interceptor.
builder.Services.AddDbContext<MemoryDbContext>(
    (serviceProvider, options) =>
    options.AddInterceptors(serviceProvider.GetRequiredService<EntityGuardianInterceptor>()));

//3 .Add and configure builder.Services.AddEntityGuardian().
builder.Services.AddEntityGuardian(
    builder.Configuration.GetConnectionString("SqlServerConnection"),
    options =>
    {
        options.StorageType = StorageType.SqlServer;
        options.DataSynchronizationTimeout = 30;
        options.ClearDataOnStartup = false;
        options.RoutePrefix = "entity-guardian";
        options.EntityGuardianSchemaName = "EntityGuardian";
    });

builder.Services.AddTransient<IPublisherRepository, PublisherRepository>();

var app = builder.Build();

//4. Add app.UseEntityGuardian()
app.UseEntityGuardian();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/insert", async (IPublisherRepository publisherRepository) =>
{
    await publisherRepository.SavePublisher();
})
    .WithName("PublisherInsert")
    .WithOpenApi();

app.MapGet("/update", async (IPublisherRepository publisherRepository) =>
    {
        await publisherRepository.UpdatePublisher();
    })
    .WithName("PublisherUpdate")
    .WithOpenApi();

app.MapGet("/delete", async (IPublisherRepository publisherRepository) =>
    {
        await publisherRepository.DeletePublisher();
    })
    .WithName("PublisherDelete")
    .WithOpenApi();

app.Run();
