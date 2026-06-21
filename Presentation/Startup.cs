using System;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Infrastructure;
using Infrastructure.DAL;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Presentation.Areas.Identity.Data;
using Presentation.Enums;
using Presentation.Middleware;

namespace Presentation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PresentationIdentityDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ProductionDbString"))
            );

            services
                .AddDefaultIdentity<IdentityUser>(options =>
                    options.SignIn.RequireConfirmedAccount = false
                )
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<PresentationIdentityDbContext>();

            services.AddDbContext<TtcDbContext>(
                options =>
                {
                    options
                        .UseSqlServer(
                            Configuration.GetConnectionString("ProductionDbString"),
                            optionsBuilder => optionsBuilder.MigrationsAssembly("Presentation")
                        )
                        .EnableSensitiveDataLogging() // shows parameter values
                        .LogTo(Console.WriteLine, LogLevel.Information);
                },
                ServiceLifetime.Transient
            );

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(
                                Configuration.GetSection("AppSettings:Token").Value
                            )
                        ),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });

            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 10 * 1024 * 1024; // 100MB limit
                options.CompactionPercentage = 0.5; // Remove 20% of entries when over limit
            });

            services.AddSingleton<ICacheService, CacheService>();

            // services.AddDbContext<TtcDbContext> (options => {
            //     options.UseSqlite (Configuration.GetConnectionString ("AppConectionString"),
            //         optionsBuilder =>
            //         optionsBuilder.MigrationsAssembly ("Presentation"));
            // });

            //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=conferences.db"), ServiceLifetime.Transient);

            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
                        .Json
                        .ReferenceLoopHandling
                        .Ignore
                );
            services.AddRazorPages();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    name: "v1",
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "TTC web api",
                        Version = "v1",
                    }
                );
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //custom services
            //  services.AddScoped<IMediaRepo, MediaRepo> ();
            services.AddScoped<IBlobRepo, BlobRepo>();
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IServiceProvider service
        )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "TTC web api");
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting(); // 1. figure out which endpoint matches

            // global cors policy
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); // 2. CORS needs to be after UseRouting, before UseAuthorization

            app.UseAuthentication(); // 3. who are you
            app.UseAuthorization(); // 4. are you allowed (built-in [Authorize] checks)

            // custom middleware that may depend on endpoint/auth info
            app.UseMiddleware<ApiKeyValidationMiddleware>();
            app.UseMiddleware<RateLimitingMiddleware>();
            app.UseMiddleware<AuthorizationEnforcementMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //CreateRoles (service).Wait ();
        }

        // private void UpdateDatabase (IApplicationBuilder app)
        // {
        //     using (var serviceScope = app.ApplicationServices
        //         .GetRequiredService<IServiceScopeFactory> ()
        //         .CreateScope ())
        //     {
        //         using (var context = serviceScope.ServiceProvider.GetService<TtcDbContext> ())
        //         {
        //             context.Database.Migrate ();
        //         }

        //         using (var context = serviceScope.ServiceProvider.GetService<PresentationIdentityDbContext> ())
        //         {
        //             context.Database.Migrate ();
        //         }
        //     }
        // }

        // private async Task CreateRoles (IServiceProvider serviceProvider)
        // {
        //     var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>> ();
        //     var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>> ();

        //     IdentityResult roleResult;

        //     string[] roles = new string[] { AppUserRoles.Admin.ToString(),
        //                 AppUserRoles.Author.ToString(),
        //                 AppUserRoles.Contributor.ToString(),
        //                 AppUserRoles.Writer.ToString()};

        //     foreach (var item in roles)
        //     {
        //         var roleCheck = await roleManager.RoleExistsAsync (item);
        //         if (!roleCheck)
        //         {
        //             roleResult = await roleManager.CreateAsync (new IdentityRole (item));
        //         }
        //     }

        //     var user = new IdentityUser { Email = "admin@ttc", UserName = "admin@ttc", EmailConfirmed = true };
        //     var isPresent = await userManager.FindByEmailAsync (user.Email);
        //     if (isPresent == null)
        //     {
        //         var result = await userManager.CreateAsync (user, "1089Tgh007$");
        //         if (result.Succeeded)
        //         {
        //             await userManager.AddToRoleAsync (user, "Admin");
        //         }
        //     }

        // }
    }
}
