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

            // Create a database and seed it with initial data
            CreateDbIfNotExists(app);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // Enable HTTPS redirection and serve static files like CSS, JavaScript, and image
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Enable routing and authorization middleware.
            // Routing is responsible for determining which controller and action should handle a given request,
            // while authorization ensures that users have the necessary permissions to access certain resources.
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
            //Creates a new scope within which you can resolve and use services.
            using (var scope = app.Services.CreateScope())
            {
                // This line retrieves the IServiceProvider from the created scope.
                // The IServiceProvider is responsible for managing and providing access to registered services.
                var services = scope.ServiceProvider;
                try
                {
                    // Requesting an instance of the SchoolContext class from the service provider.
                    // The GetRequiredService method is used to retrieve a service of a specific type. 
                    var context = services.GetRequiredService<SchoolContext>();
                    //Assuming that context now holds an instance of SchoolContext, this line calls the DbInitializer.Initialize method
                    //and passes the SchoolContext instance as an argument. This is where the actual database initialization and seed data insertion occur.
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