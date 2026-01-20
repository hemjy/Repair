using Repair.Application;
using Repair.Application.Features.Brands.Commands;
using Repair.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();
// get config
var config = builder.Configuration;
// Add services to the container.
builder.Services.AddInfrastructure(config);
// register mediator
builder.Services.AddMediatR(cfg =>
cfg.RegisterServicesFromAssemblies(new[] { typeof(Program).Assembly, typeof(AppAssembly).Assembly }));

//builder.Services.AddValidatorsFromAssembly(typeof(AppAssembly).Assembly);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
