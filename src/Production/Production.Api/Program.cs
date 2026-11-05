using Microsoft.EntityFrameworkCore;
using Production.Application.Interfaces;
using Production.Application.UseCases.ProductionOrders.CreateProductionOrder;
using Production.Application.UseCases.ProductionOrders.GetProductionOrder;
using Production.Application.UseCases.ProductionOrders.GetProductionOrders;
using Production.Application.UseCases.ProductionOrders.ReportProduction;
using Production.Infrastructure.Persistence;
using Production.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ProductionDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ProductionDatabase"))
);

// Repository
builder.Services.AddScoped<IProductionOrderRepository, ProductionOrderRepository>();

// Application Use Cases
builder.Services.AddScoped<CreateProductionOrderHandler>();
builder.Services.AddScoped<GetProductionOrderHandler>();
builder.Services.AddScoped<GetProductionOrdersHandler>();
builder.Services.AddScoped<ReportProductionHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
