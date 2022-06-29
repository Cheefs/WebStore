using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using WebStore.DAL.Context;
using WebStore.Domain.Entities;
using WebStore.Domain.Entities.Identity;
using WebStore.Interfaces.Services;
using WebStore.Services.Data;
using WebStore.Services.Services.InSQL;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var services = builder.Services;

var dbType = config["DB:Type"];
var dbConnectionString = config.GetConnectionString(dbType);

switch (dbType)
{
    case "DockerDB":
    case "SqlServer":
        services.AddDbContext<WebStoreDB>(opt => opt.UseSqlServer(dbConnectionString));
        break;
    case "Sqlite":
        services.AddDbContext<WebStoreDB>(opt => opt.UseSqlite(dbConnectionString, o => o.MigrationsAssembly("WebStore.DAL.Sqlite")));
        break;
}

services.AddScoped<DbInitializer>();

services.AddIdentity<User, Role>()
   .AddEntityFrameworkStores<WebStoreDB>()
   .AddDefaultTokenProviders();

services.Configure<IdentityOptions>(opt =>
{
#if DEBUG
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 3;
    opt.Password.RequiredUniqueChars = 3;
#endif

    opt.User.RequireUniqueEmail = false;
    opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIGKLMNOPQRSTUVWXYZ1234567890";

    opt.Lockout.AllowedForNewUsers = false;
    opt.Lockout.MaxFailedAccessAttempts = 10;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
});

services.AddScoped<IEmployeesData, SqlEmployeesData>();
services.AddScoped<IProductData, SqlProductData>();
services.AddScoped<IOrderService, SqlOrderService>();


services.AddControllers(opt =>
{
    opt.InputFormatters.Add(new XmlSerializerInputFormatter(opt));
    opt.OutputFormatters.Add(new XmlSerializerOutputFormatter());
});

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(opt =>
{
    var webstore_webapi_xml = Path.ChangeExtension(Path.GetFileName(typeof(Program).Assembly.Location), ".xml");
    var webstore_domain_xml = Path.ChangeExtension(Path.GetFileName(typeof(Product).Assembly.Location), ".xml");

    IncludeDocumentation<Program>(opt);
    IncludeDocumentation<Product>(opt);
});

static void IncludeDocumentation<T>(SwaggerGenOptions opt)
{
    var fileName = Path.ChangeExtension(Path.GetFileName(typeof(T).Assembly.Location), ".xml");
    const string debugPath = "bin/Debug/net6.0";

    if (File.Exists(fileName))
        opt.IncludeXmlComments(fileName);
    else if (File.Exists(Path.Combine(debugPath, fileName)))
        opt.IncludeXmlComments(Path.Combine(debugPath, fileName));
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db_initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await db_initializer.InitializeAsync(
        RemoveBefore: app.Configuration.GetValue("DB:Recreate", false),
        AddTestData: app.Configuration.GetValue("DB:AddTestData", false));
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
