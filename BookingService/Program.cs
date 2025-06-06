using System.Text;
using BookingService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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
                .WithOrigins("https://ashy-cliff-0942cde03.6.azurestaticapps.net", "http://localhost:5173/bookings")
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    });
});

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwt["Issuer"],
          ValidAudience = jwt["Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(key)
      };

      options.Events = new JwtBearerEvents
      {
          OnAuthenticationFailed = ctx =>
          {
              Console.WriteLine($"✖ Auth failed: {ctx.Exception.Message}");
              return Task.CompletedTask;
          },
          OnTokenValidated = ctx =>
          {
              Console.WriteLine($"✔ Token valid för: {ctx.Principal!.Identity?.Name}");
              return Task.CompletedTask;
          }
      };
  });

builder.Services.AddAuthorization();
builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookingService API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
          "JWT Authorization header.\r\n\r\n" +
          "Format: **Bearer &lt;token&gt;**\r\n\r\n" +
          "Klistra in endast token‐strängen (utan citattecken),\r\n" +
          "Swagger UI lägger själv till \"Bearer \"-prefixet.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",           
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
      {
        new OpenApiSecurityScheme
        {
          Reference = new OpenApiReference {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
          },
          In = ParameterLocation.Header,
          Scheme = "bearer"
        },
        new string[]{}
      }
    });
});


var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("DefaultCors");
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventService API V1");
    c.RoutePrefix = string.Empty;
});


app.MapControllers();
app.Run();
