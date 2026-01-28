using Asp.Versioning;
using AuthService.API.Middleware;
using AuthService.Application;
using AuthService.Application.Options;
using AuthService.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Phase 1: Configure options

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

// Phase 2: Infrastructure & App Services

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Phase 3: Framework Services (ORDER MATTERS HERE)---

// First API Versioning

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Controller 

builder.Services.AddControllers();

// Authentication

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtOptions = builder.Configuration
            .GetSection("Jwt")
            .Get<JwtOptions>()!;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Secret)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });


// Authorization 

builder.Services.AddAuthorization();

// SWAGGER LAST
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Auth Service API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT Token as: Bearer {your token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id= "Bearer"
                }
            },
            Array.Empty<string>()
        }

    });
});


// Phase 4: Build & Pipeline

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Exception Handling Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>(); 

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
