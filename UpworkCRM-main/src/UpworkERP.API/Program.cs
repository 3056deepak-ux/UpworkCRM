using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using UpworkERP.Application.Services.Implementation;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Enums;
using UpworkERP.Core.Interfaces;
using UpworkERP.Infrastructure.Data;
using UpworkERP.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Database
builder.Services.AddDbContext<ERPDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("UpworkERP.Infrastructure")
    )
);

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register Services
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<IAuditTrailService, AuditTrailService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

// Configure CORS
var corsSettings = builder.Configuration.GetSection("CorsSettings");
var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ERPCorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is missing");
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Marketing Platform API",
        Version = "v1",
        Description = "RESTful API for Marketing Platform - Comprehensive SMS, MMS & Email Marketing Solution with advanced features including contact management, campaign automation, analytics, and integrations.",
        Contact = new OpenApiContact
        {
            Name = "Marketing Platform Support",
            Email = "support@marketingplatform.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments for better API documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    //options.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            }
    //        },
    //        Array.Empty<string>()
    //    }
    //});

    //// Enable annotations for better documentation
    //options.EnableAnnotations();
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Marketing Platform API");
});
// Apply database migrations automatically
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ERPDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Applying database migrations...");
    dbContext.Database.Migrate();
    logger.LogInformation("Database migrations applied successfully.");
    
    // Seed initial data if Users table is empty
    if (!dbContext.Users.Any())
    {
        logger.LogInformation("Seeding initial admin user...");
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        dbContext.Users.Add(new User 
        { 
            UserName = "admin", 
            Email = "admin@upworkerp.com",
            PasswordHash = hashedPassword,
            FirstName = "Admin",
            LastName = "User",
            Role = UserRole.Admin,
            IsActive = true
        });
        dbContext.SaveChanges();
        logger.LogInformation("Initial admin user created successfully.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while applying database migrations or seeding data.");
    throw;
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UpworkERP API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at app root
    });
}

app.UseHttpsRedirection();
app.UseCors("ERPCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
