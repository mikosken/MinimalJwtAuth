using System.Text;
using AuthAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Step 1. Inject our database context and configure it to use SQLite.
builder.Services.AddDbContext<ApplicationContext>(options => {
    options.UseSqlite(builder.Configuration.GetConnectionString("sqlite"));
});

// Step 2.
// Add Identity and specify user & role data models.
// Specify password requirements.
// Specify user requirements.
// Add EF store for our application data context where we store Identity stuff.
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;

        options.User.RequireUniqueEmail = true;

        // Lots of other possible options...
}).AddEntityFrameworkStores<ApplicationContext>();

// Step 3.
// Add Authentication and specify options.
// Add JWT bearer to enable JWT authentication and specify options.
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSigningKey"))
        ),
        ValidateLifetime = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        ClockSkew = TimeSpan.Zero
    };
});

// Step 4.
// Add Authorization and configure policies.
builder.Services.AddAuthorization(options => {
    options.AddPolicy("Admins", policy => policy.RequireClaim("Admin"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Step 5. Tell app to use the authentication we have added above.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
