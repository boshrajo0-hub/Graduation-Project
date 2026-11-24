//////var builder = WebApplication.CreateBuilder(args);

//////// Add services to the container.

//////builder.Services.AddControllers();
//////// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//////builder.Services.AddOpenApi();

//////var app = builder.Build();

//////// Configure the HTTP request pipeline.
//////if (app.Environment.IsDevelopment())
//////{
//////    app.MapOpenApi();
//////}

//////app.UseHttpsRedirection();

//////app.UseAuthorization();

//////app.MapControllers();

//////app.Run();



////using HealthCareCenter.Models;
////using HealthCareCenter.Services.Implementations;
////using HealthCareCenter.Services.Interfaces;
////using HealthCareCenter.Data;
////using Microsoft.AspNetCore.Authentication.JwtBearer;
////using Microsoft.AspNetCore.Identity;
////using Microsoft.EntityFrameworkCore;
////using Microsoft.IdentityModel.Tokens;
////using Microsoft.OpenApi.Models;
////using System.Text;

////var builder = WebApplication.CreateBuilder(args);

////// 1. Database (HealthCareCenterDB)
////builder.Services.AddDbContext<HealthCareCenterDBContext>(options =>
////    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

////// 2. HTTP Client & Application Services
////builder.Services.AddHttpClient();
////builder.Services.AddScoped<IAuthService, AuthService>();
////builder.Services.AddScoped<IAppointmentService, AppointmentService>();
////builder.Services.AddScoped<IClinicalService, ClinicalService>();
////builder.Services.AddScoped<IAiService, LlmAiService>();

////// 3. JWT Authentication
////var jwtSection = builder.Configuration.GetSection("Jwt");
////var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

////builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
////    .AddJwtBearer(options =>
////    {
////        options.TokenValidationParameters = new TokenValidationParameters
////        {
////            ValidateIssuer = true,
////            ValidateAudience = true,
////            ValidateLifetime = true,
////            ValidateIssuerSigningKey = true,
////            ValidIssuer = jwtSection["Issuer"],
////            ValidAudience = jwtSection["Audience"],
////            IssuerSigningKey = signingKey
////        };
////    });

////builder.Services.AddAuthorization();

////// 4. Controllers & Swagger (with Bearer token support)
////builder.Services.AddControllers();
////builder.Services.AddEndpointsApiExplorer();
////builder.Services.AddSwaggerGen(options =>
////{
////    options.SwaggerDoc("v1", new OpenApiInfo
////    {
////        Title = "Health Care Center Smart Management API",
////        Version = "v1"
////    });

////    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
////    {
////        Name = "Authorization",
////        Type = SecuritySchemeType.Http,
////        Scheme = "Bearer",
////        BearerFormat = "JWT",
////        In = ParameterLocation.Header,
////        Description = "Enter: Bearer {your JWT token}"
////    });

////    options.AddSecurityRequirement(new OpenApiSecurityRequirement
////    {
////        {
////            new OpenApiSecurityScheme
////            {
////                Reference = new OpenApiReference
////                {
////                    Type = ReferenceType.SecurityScheme,
////                    Id = "Bearer"
////                }
////            },
////            Array.Empty<string>()
////        }
////    });
////});

////var app = builder.Build();

////// 5. Optional Development Seed Data (creates a test Admin user if Users table is empty)
////if (app.Environment.IsDevelopment())
////{
////    using var scope = app.Services.CreateScope();
////    var db = scope.ServiceProvider.GetRequiredService<HealthCareCenterDBContext>();
////    if (!await db.Users.AnyAsync())
////    {
////        var hasher = new PasswordHasher<User>();
////        var admin = new User
////        {
////            Email = "admin@healthcomplex.local",
////            Role = "Admin",
////            IsActive = true
////        };
////        admin.PasswordHash = hasher.HashPassword(admin, "Admin@12345");

////        db.Users.Add(admin);
////        await db.SaveChangesAsync();
////    }

////    app.UseSwagger();
////    app.UseSwaggerUI();
////}

////app.UseHttpsRedirection();

////// Order matters: Authentication must come before Authorization
////app.UseAuthentication();
////app.UseAuthorization();

////app.MapControllers();

////app.Run();


//using HealthCareCenter.Models;
//using HealthCareCenter.Services.Implementations;
//using HealthCareCenter.Services.Interfaces;
//using HealthCareCenter.Data;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// 1. Database (HealthCareCenterDB)
//builder.Services.AddDbContext<HealthCareCenterDBContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// 2. HTTP Client & Application Services
//builder.Services.AddHttpClient();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IPatientService, PatientService>();
//builder.Services.AddScoped<IAppointmentService, AppointmentService>();
//builder.Services.AddScoped<IClinicalService, ClinicalService>();
//builder.Services.AddScoped<IAiService, LlmAiService>();

//// 3. JWT Authentication
//var jwtSection = builder.Configuration.GetSection("Jwt");
//var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = jwtSection["Issuer"],
//            ValidAudience = jwtSection["Audience"],
//            IssuerSigningKey = signingKey
//        };
//    });

//builder.Services.AddAuthorization();

//// 4. Controllers & Swagger (with Bearer token support)
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "Health Care Center Smart Management API",
//        Version = "v1"
//    });

//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        Scheme = "Bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "Enter: Bearer {your JWT token}"
//    });

//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

//var app = builder.Build();

//// 5. Optional Development Seed Data (creates a test Admin user if Users table is empty)
//if (app.Environment.IsDevelopment())
//{
//    using var scope = app.Services.CreateScope();
//    var db = scope.ServiceProvider.GetRequiredService<HealthCareCenterDBContext>();
//    if (!await db.Users.AnyAsync())
//    {
//        var hasher = new PasswordHasher<User>();
//        var admin = new User
//        {
//            Email = "admin@healthcomplex.local",
//            Role = "Admin",
//            IsActive = true
//        };
//        admin.PasswordHash = hasher.HashPassword(admin, "Admin@12345");

//        db.Users.Add(admin);
//        await db.SaveChangesAsync();
//    }

//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();


using HealthCareCenter.Models;
using HealthCareCenter.Services.Implementations;
using HealthCareCenter.Services.Interfaces;
using HealthCareCenter.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Database (HealthCareCenterDB)
builder.Services.AddDbContext<HealthCareCenterDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. HTTP Client & Application Services
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IClinicalService, ClinicalService>();
builder.Services.AddScoped<IAiService, LlmAiService>();

// 3. JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = signingKey
        };
    });

builder.Services.AddAuthorization();

// 4. Controllers & Swagger (with Bearer token support)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Health Care Center Smart Management API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// 5. Optional Development Seed Data (creates a test Admin user if Users table is empty)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<HealthCareCenterDBContext>();
    if (!await db.Users.AnyAsync())
    {
        var hasher = new PasswordHasher<User>();
        var admin = new User
        {
            Email = "admin@healthcomplex.local",
            Role = "Admin",
            IsActive = true
        };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin@12345");

        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Order matters: Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();