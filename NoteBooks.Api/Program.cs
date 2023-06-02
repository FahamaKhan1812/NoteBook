using Microsoft.EntityFrameworkCore;
using NoteBook.DataService.Data;
using NoteBook.DataService.IConfiguration;
using Microsoft.AspNetCore.Mvc;
using NoteBook.Authentication.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

// SQL Connection
builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SQLdbConnection"));
}
);
// Update the JWT config from settings
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JWTConfig"));

builder.Services.AddControllers();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiVersioning(opt =>
{
    // Provides to the client the different version that we have
    opt.ReportApiVersions = true;

    // this will allow the api
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.DefaultApiVersion = ApiVersion.Default;

});
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{   
    // Getting secret key from the app.stteting.json
    var key = Encoding.ASCII.GetBytes(builder.Configuration["JWTConfig:Secret"]);

    // adding configurations
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = false,
        ValidateLifetime = true
    };
});

builder.Services.AddDefaultIdentity<IdentityUser>(opt => opt.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>();

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
