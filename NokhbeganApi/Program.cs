using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NokhbeganApi.Context;
using NokhbeganApi.Model;
using NokhbeganApi.Customized;
using Serilog;
using System.Text;
using NokhbeganApi.Helper;
using Microsoft.Extensions.FileProviders;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/ResponseInformation.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();



var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllersWithViews().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<NokhbeganDbContext>(option => 
option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

builder.Services.AddIdentity<T_CustomUser,IdentityRole>(option =>
{
    option.User.RequireUniqueEmail = false;
    

}).AddEntityFrameworkStores<NokhbeganDbContext>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new()
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidAudience = builder.Configuration["Jwt:Audience"],
    };
});

var config = builder.Configuration;
builder.Services.AddSwaggerGen(c =>
{
    // Add the JWT Authorization header to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT token with Bearer prefix, e.g., 'Bearer {token}'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Nokhbegan API",
        Version = "v1",
        Description = "An API to perform English stuff operations",
        Contact = new OpenApiContact
        {
            Name = "Meisam Sohrabi",
            Email = "empty.",
        },
    });
});

builder.Services.AddCustomServices();
//builder.Services.AddSingleton<ArvanCloud3Service>();
builder.Host.UseSerilog();
builder.Services.AddHttpClient();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(5);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())  // The 'using' ensures proper disposal
    {
        var services = scope.ServiceProvider;
        var seeder = services.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync();  // Seed data
    }

}


app.UseHttpsRedirection();
app.UseSession();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "userimage")),
    RequestPath = "/userimage"
});
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
