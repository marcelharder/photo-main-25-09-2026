

namespace fotoservice.extensions;
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {

            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;

            })
                 .AddRoles<AppRole>()
                 .AddRoleManager<RoleManager<AppRole>>()
                 .AddSignInManager<SignInManager<AppUser>>()
                 .AddRoleValidator<RoleValidator<AppRole>>()
                 .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                         ValidateIssuer = false,
                         ValidateAudience = false
                     };
                     options.Events = new JwtBearerEvents
                     {
                         OnMessageReceived = context =>
                         {
                             var accessToken = context.Request.Query["access_token"];
                             var path = context.HttpContext.Request.Path;
                             if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                             {
                                 context.Token = accessToken;
                             };
                             return Task.CompletedTask;
                         }
                     };
                 });
           
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;

                options.User.RequireUniqueEmail = true;
               
            });

            services.Configure<DataProtectionTokenProviderOptions>(opt=>opt.TokenLifespan = TimeSpan.FromHours(2));
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("RequireChefRole", policy => policy.RequireRole("Chef"));
            });
            return services;
        }
    }
