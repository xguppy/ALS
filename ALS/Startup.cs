using System;
using System.IO;
using System.Linq;
using System.Text;
using ALS.AntiPlagModule.Services;
using ALS.AntiPlagModule.Services.LexerFactory;
using ALS.AntiPlagModule.Services.LexerService;
using ALS.CheckModule.Processes;
using ALS.EntityСontext;
using ALS.Services;
using ALS.Services.AuthService;
using Generator.Parsing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ALS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationContext>();

            services.AddSingleton<IAuthService>(new AuthService(Configuration));
            services.AddSingleton<ILexer>(new CppLexer(new CppLexerFactory()));
            services.AddHttpClient();
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
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                        ClockSkew = TimeSpan.Zero
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

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}");
            });

            dbContext.Database.EnsureCreated();

            CreateNeedDirectory();
            SetupDatabase(dbContext);
        }
        /// <summary>
        /// Создание требуемых для приложения директорий если их нет в системе
        /// </summary>
        void CreateNeedDirectory()
        {
            if (!Directory.Exists("sourceCodeUser"))
            {
                Directory.CreateDirectory("sourceCodeUser");
            }

            if (!Directory.Exists("uploads"))
            {
                Directory.CreateDirectory("uploads");
            }

            if (!Directory.Exists("sourceCodeModel"))
            {
                Directory.CreateDirectory("sourceCodeModel");
            }

            if (!Directory.Exists("executeUser"))
            {
                Directory.CreateDirectory("executeUser");
            }

            if (!Directory.Exists("executeModel"))
            {
                Directory.CreateDirectory("executeModel");
            }
        }

        void SetupDatabase(ApplicationContext context)
        {
            // check and add roles
            AuthService auth = new AuthService(Configuration);
            
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
            
            if (!context.Roles.Any())
            {
                context.Roles.Add(new Role { RoleId = 1, RoleName = RoleEnum.Student });
                context.Roles.Add(new Role { RoleId = 2, RoleName = RoleEnum.Teacher });
                context.Roles.Add(new Role { RoleId = 3, RoleName = RoleEnum.Admin });

                context.SaveChanges();
            }
            
            if (!context.Users.Any())
            {
                var student = new User { Surname = "Студентов", Name = "Студент", Patronymic = "Студентович", Email = "tmpstudent@mail.com", PwHash = auth.GetHashedPassword("tmpstudent"), GroupId = 1};
                var student1 = new User { Surname = "Иванов", Name = "Иван", Patronymic = "Иванович", Email = "ivan@mail.com", PwHash = auth.GetHashedPassword("ivan"), GroupId = 2};
                var student2 = new User { Surname = "Петров", Name = "Петр", Patronymic = "Петрович", Email = "petr@mail.com", PwHash = auth.GetHashedPassword("petr"), GroupId = 1};
                var student3 = new User { Surname = "Макаров", Name = "Макар", Patronymic = "Макарович", Email = "makar@mail.com", PwHash = auth.GetHashedPassword("makar"), GroupId = 2};
                var student4 = new User { Surname = "Сидорова", Name = "Александра", Patronymic = "Михайловна", Email = "alexandrochka@mail.com", PwHash = auth.GetHashedPassword("alexandrochka"), GroupId = 1};
                var student5 = new User { Surname = "Владов", Name = "Владислав", Patronymic = "Владиславович", Email = "vlad@mail.com", PwHash = auth.GetHashedPassword("vlad"), GroupId = 2};
                var student6 = new User { Surname = "Федоров", Name = "Федор", Patronymic = "Федорович", Email = "fedor@mail.com", PwHash = auth.GetHashedPassword("fedor"), GroupId = 1};
                var teacher = new User { Surname = "Преподов", Name = "Препод", Patronymic = "Преподович", Email = "tmpprepod@mail.com", PwHash = auth.GetHashedPassword("tmpprepod") };
                var admin = new User { Surname = "Админов", Name = "Админ", Patronymic = "Админович", Email = "tmpadmin@mail.com", PwHash = auth.GetHashedPassword("tmpadmin") };

                
                context.Users.Add(student);
                context.Users.Add(student1);
                context.Users.Add(student2);
                context.Users.Add(student3);
                context.Users.Add(student4);
                context.Users.Add(student5);
                context.Users.Add(student6);
                context.Users.Add(admin);
                context.Users.Add(teacher);
                context.SaveChanges();

                context.UserRoles.Add(new UserRole { UserId = student.Id, RoleId = 1 });
                context.UserRoles.Add(new UserRole { UserId = student1.Id, RoleId = 1 });
                context.UserRoles.Add(new UserRole { UserId = student2.Id, RoleId = 1 });
                context.UserRoles.Add(new UserRole { UserId = student3.Id, RoleId = 1 });
                context.UserRoles.Add(new UserRole { UserId = student4.Id, RoleId = 1 });
                context.UserRoles.Add(new UserRole { UserId = student5.Id, RoleId = 1 });
                context.UserRoles.Add(new UserRole { UserId = student6.Id, RoleId = 1 });
                context.UserRoles.Add(new UserRole { UserId = teacher.Id, RoleId = 2 });
                context.UserRoles.Add(new UserRole { UserId = admin.Id, RoleId = 3 });
                
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
                context.TemplateLaboratoryWorks.Add(new TemplateLaboratoryWork { TemplateTask = @"C:/Users/kampukter/source/repos/ALS/ALS.GeneratorModule/Docs/Exp.txt", ThemeId = 1 });
                context.TemplateLaboratoryWorks.Add(new TemplateLaboratoryWork { TemplateTask = @"C:/Users/kampukter/source/repos/ALS/ALS.GeneratorModule/Docs/Exp3.txt", ThemeId = 1 });
                context.TemplateLaboratoryWorks.Add(new TemplateLaboratoryWork { TemplateTask = @"C:/Users/kampukter/source/repos/ALS/ALS.GeneratorModule/Docs/Exp.txt", ThemeId = 2 });
                context.TemplateLaboratoryWorks.Add(new TemplateLaboratoryWork { TemplateTask = @"C:/Users/kampukter/source/repos/ALS/ALS.GeneratorModule/Docs/Exp2.txt", ThemeId = 2 });
                context.SaveChanges();
            }

            if (!context.LaboratoryWorks.Any())
            {
                context.LaboratoryWorks.Add(new LaboratoryWork { UserId = 9, ThemeId = 2, Name = "lr1", Description = "lr1_description", Constraints = "{\"Memory\": 4096000, \"Time\": 60000}", DisciplineCipher = "pr1"});
                context.LaboratoryWorks.Add(new LaboratoryWork { UserId = 9, ThemeId = 1, Name = "lr2", Description = "Вывести четные элементы", Constraints = "{\"Memory\": 4096000, \"Time\": 60000}", DisciplineCipher = "pr1"});
                context.LaboratoryWorks.Add(new LaboratoryWork { TemplateLaboratoryWorkId = 1, UserId = 8, ThemeId = 2, Name = "lr3", Description = "descrition", Constraints = "{\"Memory\": 4096000, \"Time\": 60000}", DisciplineCipher = "pr1" });
                context.LaboratoryWorks.Add(new LaboratoryWork { UserId = 2, ThemeId = 1, Name = "lr1", Description = "lr1_description", Constraints = "{\"Memory\": 4096000, \"Time\": 60000}", DisciplineCipher = "pr1"});
                context.LaboratoryWorks.Add(new LaboratoryWork { UserId = 2, ThemeId = 1, Name = "lr2", Description = "Вывести четные элементы", Constraints = "{\"Memory\": 4096000, \"Time\": 60000}", DisciplineCipher = "pr1"});
                context.LaboratoryWorks.Add(new LaboratoryWork { TemplateLaboratoryWorkId = 3, UserId = 2, ThemeId = 2, Name = "lr3", Description = "descrition", Constraints = "{\"Memory\": 4096000, \"Time\": 60000}", DisciplineCipher = "pr1" });
                context.SaveChanges();
            }
            if (!context.Variants.Any())
            {
                context.Variants.Add(new Variant { VariantNumber = 1, LaboratoryWorkId = 5, Description = "var descr" });
                context.Variants.Add(new Variant { VariantNumber = 2, LaboratoryWorkId = 5, Description = "Тест" });
                context.Variants.Add(new Variant { VariantNumber = 1, LaboratoryWorkId = 6, LinkToModel = Path.Combine(Environment.CurrentDirectory, "executeModel", $"{ProcessCompiler.CreatePath(2, 1)}.exe"), Description = "В стандартный поток ввода принимаются 10 целых чисел. Вывести в стандартный поток вывода только чётные числа.", InputDataRuns = "[{\"Name\":\"тест1\",\"Data\":[\"#rnd(1 | 20 | int | 10)\"]},{\"Name\":\"тест2\",\"Data\":[\"#rnd(1 | 20 | int | 10)\"]},{\"Name\":\"тест3\",\"Data\":[\"1\",\"2\",\"3\",\"4\",\"5\",\"6\",\"7\",\"8\",\"9\",\"10\"]}]"});
                context.SaveChanges();
            }

            if (!context.AssignedVariants.Any())
            {
                context.AssignedVariants.Add(new AssignedVariant {UserId = 3, VariantId = 2});
                context.AssignedVariants.Add(new AssignedVariant {UserId = 2, VariantId = 2});
                context.AssignedVariants.Add(new AssignedVariant {UserId = 1, VariantId = 2});
                context.AssignedVariants.Add(new AssignedVariant {UserId = 1, VariantId = 1});
                
                context.AssignedVariants.Add(new AssignedVariant {UserId = 4, VariantId = 1});

                context.AssignedVariants.Add(new AssignedVariant {UserId = 5, VariantId = 1});
                context.AssignedVariants.Add(new AssignedVariant {UserId = 5, VariantId = 2});

                context.AssignedVariants.Add(new AssignedVariant {UserId = 6, VariantId = 1});
                context.AssignedVariants.Add(new AssignedVariant {UserId = 6, VariantId = 2});
                
                context.SaveChanges();
            }
            
            if (!context.Solutions.Any())
            {
                context.Solutions.Add(new Solution { IsSolved = true, AssignedVariantId = 1, CompilerFailsNumbers = 3, SendDate = DateTime.Now.AddDays(2),  SourceCode = "#include <iostream>\n\nint func_arr(int* arr, size_t len);\nvoid input_arr(int* &arr, size_t len);\n\nint main()\n{\n	// Очень важный комментарий\n	// Или не очень\n	cout << \"Программа вывода суммы квадратов вектора\" << endl;\n	int len = 3;\n	int* arr = new int[len];\n	input_arr(arr, len);\n	cout << \"Результат равен \" << func_arr(arr, len) << endl;\n	return 0;\n}\n\nint func_arr(int* arr, size_t len)\n{\n	/*\n	Возвращает сумму квадратов элементов массива\n	*/\n	int res = 0;\n	for (size_t i = 0; i < len; ++i)\n	{\n		res += arr[i] * arr[i];\n	}\n	return res;\n}\n\nvoid input_arr(int* &arr, size_t len)\n{\n	// Ввод массива\n	for (size_t i = 0; i < len; ++i)\n		cin >> arr[i];\n}" });
                context.Solutions.Add(new Solution { IsSolved = true, AssignedVariantId = 2, CompilerFailsNumbers = 2, SendDate = DateTime.Now.AddDays(1),  SourceCode = "#include <iostream>\n\nint func_arr(int* arr, size_t len);\nvoid input_arr(int* &arr, size_t len);\n\nint main()\n{\n	// Очень важный комментарий\n	// Или не очень\n	cout << \"Программа вывода суммы квадратов вектора\" << endl;\n	int len = 3;\n	int* arr = new int[len];\n	input_arr(arr, len);\n	cout << \"Результат равен \" << func_arr(arr, len) << endl;\n	return 0;\n}\n\nint func_arr(int* arr, size_t len)\n{\n	/*\n	Возвращает сумму квадратов элементов массива\n	*/\n	int res = 0;\n	for (size_t i = 0; i < len; ++i)\n	{\n		res += arr[i] * arr[i];\n	}\n	return res;\n}\n\nvoid input_arr(int* &arr, size_t len)\n{\n	// Ввод массива\n	for (size_t i = 0; i < len; ++i)\n		cin >> arr[i];\n}" });
                context.Solutions.Add(new Solution { IsSolved = true, AssignedVariantId = 3, CompilerFailsNumbers = 1, SendDate = DateTime.Now,  SourceCode = "#include <iostream>\n\nint func_massiv(int* massiv, size_t size_a)\n{\n	/*\n	Возвращает сумму квадратов элементов массива\n	*/\n	int res = 0;\n	for (size_t i = 0; i < size_a; ++i)\n	{\n		res += massiv[i] * massiv[i];\n	}\n	return res;\n}\n\nvoid input_massiv(int* &massiv, size_t size_a)\n{\n	// Ввод массива\n	for (size_t i = 0; i < size_a; ++i)\n		cin >> massiv[i];\n}\n\nint main()\n{\n	cout << \"Моя(нет) программа вывода суммы квадратов элементов массива\" << endl;\n	int size_a = 3;\n	int* massiv = new int[size_a];\n	input_massiv(massiv, size_a);\n	cout << \"Ответ \" << func_massiv(massiv, size_a) << endl;\n	return 0;\n}\n\n" });
                context.Solutions.Add(new Solution { IsSolved = false, SendDate = DateTime.Now.AddHours(20), CompilerFailsNumbers = 1, AssignedVariantId = 4});
                
                
                context.Solutions.Add(new Solution { IsSolved = false, CompilerFailsNumbers = 1, SendDate = DateTime.Now.AddHours(2), AssignedVariantId = 5});
                context.Solutions.Add(new Solution { IsSolved = false, CompilerFailsNumbers = 2, SendDate = DateTime.Now.AddHours(12), AssignedVariantId = 5});
                
                context.Solutions.Add(new Solution { IsSolved = false, CompilerFailsNumbers = 0, SendDate = DateTime.Now.AddHours(10), AssignedVariantId = 6});
                context.Solutions.Add(new Solution { IsSolved = true, CompilerFailsNumbers = 0, SendDate = DateTime.Now.AddHours(9), AssignedVariantId = 6});
                context.Solutions.Add(new Solution { IsSolved = false, CompilerFailsNumbers = 0, SendDate = DateTime.Now.AddHours(5), AssignedVariantId = 7});
                context.Solutions.Add(new Solution { IsSolved = true, CompilerFailsNumbers = 0, SendDate = DateTime.Now.AddHours(17), AssignedVariantId = 7});
                
                context.Solutions.Add(new Solution { IsSolved = true, CompilerFailsNumbers = 5, SendDate = DateTime.Now.AddHours(15), AssignedVariantId = 8});
                context.Solutions.Add(new Solution { IsSolved = true, CompilerFailsNumbers = 5,SendDate = DateTime.Now.AddHours(6), AssignedVariantId = 9});
                
                context.SaveChanges();
            }

            if (!context.Plans.Any())
            {
                context.Plans.Add(new Plan {DisciplineCipher = "pr1", GroupId = 1, UserId = 9 });
                context.Plans.Add(new Plan {DisciplineCipher = "pr1", GroupId = 2, UserId = 9 });
                
                context.SaveChanges();
            }
        }
    }
}