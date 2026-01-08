using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PosTerminalProcessor.Application.Authenticate;
using PosTerminalProcessor.Application.Customers;
using PosTerminalProcessor.Application.Interfaces;
using PosTerminalProcessor.Application.Terminals;
using PosTerminalProcessor.Infrastructure;
using RestSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationDbContext>(
    provider => provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<ITerminalService, TerminalService>();
builder.Services.AddTransient<IValidateService, ValidateService>();
builder.Services.AddTransient<IRestClient, RestClient>();
// Configure JWT authentication (signed tokens only)
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var secretBase64 = jwtSection["SecretKey"] ?? throw new InvalidOperationException("JwtSettings:SecretKey is required.");
var keyBytes = Convert.FromBase64String(secretBase64);
var symmetricKey = new SymmetricSecurityKey(keyBytes);
builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = symmetricKey,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
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
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
