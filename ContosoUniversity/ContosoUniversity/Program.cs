using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add configuration.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .Build();

            // Configure services: register the SchoolContext as a service, specifying that it should use SQL Server as the database provider.
            builder.Services.AddDbContext<SchoolContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Add the database exception filter if the environment is in development mode
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            }
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Building the Application.
            var app = builder.Build();

            CreateDbIfNotExists(app);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            
            app.UseRouting();
            app.UseAuthorization();

            // Map default controller route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
        private static void CreateDbIfNotExists(WebApplication app)
        {
            
            using (var scope = app.Services.CreateScope())
            {
                
                var services = scope.ServiceProvider;
                try
                {
                   
                    var context = services.GetRequiredService<SchoolContext>();
                   
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }
    }
}