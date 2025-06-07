using System.Security.Claims;
using System.Text;
using Azure.Identity;
using BookingService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//Lägg på Key Vault som config-source
var vaultUri = new Uri(builder.Configuration["KeyVault:VaultUri"]!);
builder.Configuration.AddAzureKeyVault(vaultUri, new DefaultAzureCredential());

//Läs in secreyts
//Databas‐anslutning
var connString = builder.Configuration.GetConnectionString("DefaultConnection")!;

//JWT‐inställningar
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;
var jwtExpires = builder.Configuration["Jwt:ExpiresInMinutes"]!;

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
        }
        else
        {
            policy
              .WithOrigins("https://ashy-cliff-0942cde03.6.azurestaticapps.net")
              .AllowAnyMethod()
              .AllowAnyHeader();
        }
    });
});

//Entity Framework Core
builder.Services.AddDbContext<DataContext>(opts =>
    opts.UseSqlServer(connString));

//OpenAPI (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EventService API", Version = "v1" });

    
    var bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,      
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Skriv: Bearer {token}",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme         
        }
    };

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, bearerScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { bearerScheme, Array.Empty<string>() }
    });
});

//Authentication / JWT
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(opts =>
  {
      opts.RequireHttpsMetadata = true;
      opts.SaveToken = true;
      opts.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtIssuer,
          ValidAudience = jwtAudience,
          IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
          RoleClaimType = ClaimTypes.Role
      };
  });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

//Middleware
app.UseHttpsRedirection();
app.UseCors("DefaultCors");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookingService API V1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();
app.Run();
