using ALS.EntityСontext;
using ALS.Services;
using ALS.Services.AuthService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Linq;
using ALS.AntiPlagModule.Services;
using ALS.AntiPlagModule.Services.LexerService;
using ALS.AntiPlagModule.Services.LexerFactory;

namespace ALS
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationContext>();
            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

            services.AddSingleton<IAuthService>(new AuthService(Configuration));
            services.AddSingleton<ILexer>(new CppLexer(new CppLexerFactory()));
            
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidateAudience = true,
                        ValidAudience = Configuration["JwtAudience"],
                        
                        //ValidateIssuer = false,
                        //ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            dbContext.Database.EnsureCreated();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
            
            if (!System.IO.Directory.Exists("sourceCodeUser"))
            {
                System.IO.Directory.CreateDirectory("sourceCodeUser");
            }
            
            if (!System.IO.Directory.Exists("sourceCodeModel"))
            {
                System.IO.Directory.CreateDirectory("sourceCodeModel");
            }
            
            if (!System.IO.Directory.Exists("executeUser"))
            {
                System.IO.Directory.CreateDirectory("executeUser");
            }
            
            if (!System.IO.Directory.Exists("executeModel"))
            {
                System.IO.Directory.CreateDirectory("executeModel");
            }
            
            SetupDatabase(dbContext);
        }

        void SetupDatabase(ApplicationContext context)
        {
            // check and add roles
            AuthService auth = new AuthService(Configuration);

            if (context.Roles.ToList().Count == 0)
            {
                context.Roles.Add(new Role { RoleName = RoleEnum.Student });
                context.Roles.Add(new Role { RoleName = RoleEnum.Teacher });
                context.Roles.Add(new Role { RoleName = RoleEnum.Admin });

                context.SaveChanges();
            }
            if (context.Users.ToList().Count == 0)
            {
                var student = new User { Surname = "Студентов", Name = "Студент", Patronymic = "Студентович", Email = "tmpstudent@mail.com", PwHash = auth.GetHashedPassword("tmpstudent") };
                var teacher = new User { Surname = "Преподов", Name = "Препод", Patronymic = "Преподович", Email = "tmpprepod@mail.com", PwHash = auth.GetHashedPassword("tmpprepod") };
                var admin = new User { Surname = "Админов", Name = "Админ", Patronymic = "Админович", Email = "tmpadmin@mail.com", PwHash = auth.GetHashedPassword("tmpadmin") };
                
                context.Users.Add(admin);
                context.Users.Add(teacher);
                context.Users.Add(student);

                context.SaveChanges();

                context.UserRoles.Add(new UserRole { UserId = student.Id, RoleId = 1 });
                context.UserRoles.Add(new UserRole { UserId = teacher.Id, RoleId = 2 });
                context.UserRoles.Add(new UserRole { UserId = admin.Id, RoleId = 3 });

                context.SaveChanges();
            }

        }
    }
}