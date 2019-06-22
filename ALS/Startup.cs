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
using Generator.Parsing;

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

            services.AddScoped<IParser, Parser>();
            
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

            if (!context.Roles.Any())
            {
                context.Roles.Add(new Role { RoleId = 1, RoleName = RoleEnum.Student });
                context.Roles.Add(new Role { RoleId = 2, RoleName = RoleEnum.Teacher });
                context.Roles.Add(new Role { RoleId = 3, RoleName = RoleEnum.Admin });

                context.SaveChanges();
            }
            if (!context.Users.Any())
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
            if (!context.Disciplines.Any())
            {
                context.Disciplines.Add(new Discipline { Name = "Programming", Cipher = "pr1" });
                context.SaveChanges();
            }

            if (!context.Specialties.Any())
            {
                var softwareEngineering = new Specialty { Name = "Программная инженерия", Code = "09.03.04" };
                var computerScienceAndComputing = new Specialty
                    { Name = "Информатика и вычислительная техника", Code = "09.03.01" };
                context.Specialties.Add(softwareEngineering);
                context.Specialties.Add(computerScienceAndComputing);
                
                context.Groups.Add(new Group { Name = "ДИПР-31", Year = 2019, Specialty = softwareEngineering} );
                context.Groups.Add(new Group
                    { Name = "ДИНР-31", Year = 2019, Specialty = computerScienceAndComputing} );
                context.SaveChanges();
            }


            if (!context.Themes.Any())
            {
                context.Themes.Add(new Theme { Name = "Тема 1"});
                context.Themes.Add(new Theme { Name = "Тема 2" });
                context.SaveChanges();
            }
            if (!context.TemplateLaboratoryWorks.Any())
            {
                context.TemplateLaboratoryWorks.Add(new TemplateLaboratoryWork { TemplateTask = @"file:///C:/Users/kampukter/source/repos/ALS/ALS.GeneratorModule/Docs/Exp.txt", ThemeId = 2 });
                context.TemplateLaboratoryWorks.Add(new TemplateLaboratoryWork { TemplateTask = @"file:///C:/Users/kampukter/source/repos/ALS/ALS.GeneratorModule/Docs/Exp2.txt", ThemeId = 2 });
                context.SaveChanges();
            }

            if (!context.LaboratoryWorks.Any())
            {
                context.LaboratoryWorks.Add(new LaboratoryWork { UserId = 2, ThemeId = 1, Name = "lr1", Description = "lr1_description", Constraints = "{\"Memory\": 4096000, \"Time\": 60000}", DisciplineCipher = "pr1"});
                context.LaboratoryWorks.Add(new LaboratoryWork { UserId = 2, ThemeId = 1, Name = "lr2", Description = "Вывести четные элементы", Constraints = "{\"Memory\": 4096000, \"Time\": 60000}", DisciplineCipher = "pr1"});
                context.LaboratoryWorks.Add(new LaboratoryWork { TemplateLaboratoryWorkId = 1, UserId = 2, ThemeId = 2, Name = "lr3", Description = "descrition", Constraints = "{\"Memory\": 4096000, \"Time\": 60000}", DisciplineCipher = "pr1" });
                context.SaveChanges();
            }
            if (!context.Variants.Any())
            {
                context.Variants.Add(new Variant { VariantNumber = 1, LaboratoryWorkId = 1, Description = "var descr" });
                context.Variants.Add(new Variant { VariantNumber = 1, LaboratoryWorkId = 2, Description = "smpl", InputDataRuns = "[{\"Name\":\"тест1\",\"Data\":[\"#rnd(1 | 20 | int | 10)\"]},{\"Name\":\"тест2\",\"Data\":[\"#rnd(1 | 20 | int | 10)\"]},{\"Name\":\"тест3\",\"Data\":[\"1\",\"2\",\"3\",\"4\",\"5\",\"6\",\"7\",\"8\",\"9\",\"10\"]}]"});
                context.SaveChanges();
            }
            if (!context.Solutions.Any())
            {
                context.Solutions.Add(new Solution { IsSolved = true, VariantId = 2, SendDate = DateTime.Now.AddDays(2), UserId = 3, SourceCode = "#include <iostream>\n\nint func_arr(int* arr, size_t len);\nvoid input_arr(int* &arr, size_t len);\n\nint main()\n{\n	// Очень важный комментарий\n	// Или не очень\n	cout << \"Программа вывода суммы квадратов вектора\" << endl;\n	int len = 3;\n	int* arr = new int[len];\n	input_arr(arr, len);\n	cout << \"Результат равен \" << func_arr(arr, len) << endl;\n	return 0;\n}\n\nint func_arr(int* arr, size_t len)\n{\n	/*\n	Возвращает сумму квадратов элементов массива\n	*/\n	int res = 0;\n	for (size_t i = 0; i < len; ++i)\n	{\n		res += arr[i] * arr[i];\n	}\n	return res;\n}\n\nvoid input_arr(int* &arr, size_t len)\n{\n	// Ввод массива\n	for (size_t i = 0; i < len; ++i)\n		cin >> arr[i];\n}" });
                context.Solutions.Add(new Solution { IsSolved = true, VariantId = 2, SendDate = DateTime.Now.AddDays(1), UserId = 2, SourceCode = "#include <iostream>\n\nint func_arr(int* arr, size_t len);\nvoid input_arr(int* &arr, size_t len);\n\nint main()\n{\n	// Очень важный комментарий\n	// Или не очень\n	cout << \"Программа вывода суммы квадратов вектора\" << endl;\n	int len = 3;\n	int* arr = new int[len];\n	input_arr(arr, len);\n	cout << \"Результат равен \" << func_arr(arr, len) << endl;\n	return 0;\n}\n\nint func_arr(int* arr, size_t len)\n{\n	/*\n	Возвращает сумму квадратов элементов массива\n	*/\n	int res = 0;\n	for (size_t i = 0; i < len; ++i)\n	{\n		res += arr[i] * arr[i];\n	}\n	return res;\n}\n\nvoid input_arr(int* &arr, size_t len)\n{\n	// Ввод массива\n	for (size_t i = 0; i < len; ++i)\n		cin >> arr[i];\n}" });
                context.Solutions.Add(new Solution { IsSolved = true, VariantId = 2, SendDate = DateTime.Now, UserId = 1, SourceCode = "#include <iostream>\n\nint func_massiv(int* massiv, size_t size_a)\n{\n	/*\n	Возвращает сумму квадратов элементов массива\n	*/\n	int res = 0;\n	for (size_t i = 0; i < size_a; ++i)\n	{\n		res += massiv[i] * massiv[i];\n	}\n	return res;\n}\n\nvoid input_massiv(int* &massiv, size_t size_a)\n{\n	// Ввод массива\n	for (size_t i = 0; i < size_a; ++i)\n		cin >> massiv[i];\n}\n\nint main()\n{\n	cout << \"Моя(нет) программа вывода суммы квадратов элементов массива\" << endl;\n	int size_a = 3;\n	int* massiv = new int[size_a];\n	input_massiv(massiv, size_a);\n	cout << \"Ответ \" << func_massiv(massiv, size_a) << endl;\n	return 0;\n}\n\n" });
                context.Solutions.Add(new Solution { IsSolved = false, VariantId = 1, UserId = 1});
                context.SaveChanges();
            }
        }
    }
}