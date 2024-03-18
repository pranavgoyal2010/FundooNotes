using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ModelLayer.Dto;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using System.Text;

// Create a new WebApplication builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Register AppDbContext as a singleton service
builder.Services.AddSingleton<AppDbContext>();

// Register IUserBL, IUserRL, and IAuthServiceRL as scoped services
builder.Services.AddScoped<IUserBL, UserBL>();
builder.Services.AddScoped<IUserRL, UserRL>();
builder.Services.AddScoped<IAuthServiceRL, AuthServiceRL>();
builder.Services.AddScoped<INoteBL, NoteBL>();
builder.Services.AddScoped<INoteRL, NoteRL>();
builder.Services.AddScoped<ICollaborateBL, CollaborateBL>();
builder.Services.AddScoped<ICollaborateRL, CollaborateRL>();

builder.Services.AddScoped<IMailServiceBL, MailServiceBL>();

builder.Services.Configure<EmailDto>(builder.Configuration.GetSection("EmailDto"));
builder.Services.AddScoped<IMailServiceRL, MailServiceRL>();

// Change the injection to use IOptions<EmailSettings>
builder.Services.AddScoped(sp => sp.GetRequiredService<IOptions<EmailDto>>().Value);

// Configure JWT authentication

// Retrieve the secret key from appsettings.json for JWT token validation
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]);


// Add authentication services with JWT bearer authentication scheme
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Configure JWT bearer options
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Enable validation of the issuer signing key
            ValidateIssuerSigningKey = true,

            // Specify the issuer signing key
            IssuerSigningKey = new SymmetricSecurityKey(key),

            // Disable issuer validation (not recommended for production)
            ValidateIssuer = false,

            // Disable audience validation (not recommended for production)
            ValidateAudience = false
        };
    });

// Add controllers to the service collection
builder.Services.AddControllers();

// Configure Swagger/OpenAPI

// Add API Explorer services to the service collection
builder.Services.AddEndpointsApiExplorer();

// Add Swagger generator services to the service collection
builder.Services.AddSwaggerGen(c =>
{
    // Configure Swagger document information
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FundooNotes", Version = "v1" });

    // Configure JWT authentication for Swagger

    // Add security definition for the Bearer scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        // Description of the security scheme
        Description = "JWT Authorization header using the Bearer scheme",

        // Name of the header that contains the JWT token
        Name = "Authorization",

        // Location of the JWT token in the HTTP request header
        In = ParameterLocation.Header,

        // Type of the security scheme (HTTP in this case)
        Type = SecuritySchemeType.Http,

        // The authentication scheme being used (Bearer in this case)
        Scheme = "bearer",

        // The format of the JWT token (JWT format)
        BearerFormat = "JWT"
    });


    // Add security requirement for Bearer scheme
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
        // Define the reference to the security scheme
            new OpenApiSecurityScheme
            {
            // Specify the type of reference (SecurityScheme)
                Reference = new OpenApiReference
                {
                // Specify the type of reference (SecurityScheme)
                    Type = ReferenceType.SecurityScheme,
                
                // Specify the identifier of the referenced security scheme
                    Id = "Bearer"
                }
            },
        // Define the list of scopes (no scopes defined here)
            new string[] {}
        }
    });
});

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline

// Enable Swagger UI if running in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable authentication
app.UseAuthentication();

// Enable authorization
app.UseAuthorization();

// Map controllers to HTTP request endpoints
app.MapControllers();

// Run the application
app.Run();
