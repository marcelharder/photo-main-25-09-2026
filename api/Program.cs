using fotoservice.data.seed;
using fotoservice.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//Initialize AppSettings
AppSettings.Initialize(builder.Configuration);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; });
builder.Services.AddCors();
builder.Services.Configure<ComSettings>(builder.Configuration.GetSection("ComSettings"));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 builder.Services.AddSwaggerGen(c => 
{ 
    c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme 
    { 
        Type = SecuritySchemeType.Http, 
        Scheme = "bearer", 
        BearerFormat = "JWT", 
        Description = "JWT Authorization header using the Bearer scheme." 
    }); 

    c.AddSecurityRequirement(new OpenApiSecurityRequirement 
{ 
    { 
        new OpenApiSecurityScheme 
        { 
            Reference = new OpenApiReference 
            { 
                Type = ReferenceType.SecurityScheme, 
                Id = "bearerAuth" 
            } 
        }, 
        new string[] {} 
    } 
});
});

var app = builder.Build();

// initialize the database ...

        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var dapper = services.GetRequiredService<DapperContext>();
            var image = services.GetRequiredService<IImage>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
            await context.Database.EnsureCreatedAsync();
             await Seed.SeedUsers(userManager, roleManager, builder.Configuration);
           
            
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during application initialization");
            throw;
        }


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();


}



//app.UseHttpsRedirection();
/* app.UseStaticFiles(new StaticFileOptions{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "MyStaticFiles")),
    RequestPath = "/StaticFiles"
});
 */
 app.UseCors(x => x.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins("http://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapFallbackToController("Index", "Fallback");

app.Run();
