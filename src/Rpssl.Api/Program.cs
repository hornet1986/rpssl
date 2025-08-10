using Microsoft.EntityFrameworkCore;
using Rpssl.Application;
using Rpssl.Infrastructure;
using Rpssl.Infrastructure.Database;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    RpsslDbContext dbContext = scope.ServiceProvider.GetRequiredService<RpsslDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.Run();
