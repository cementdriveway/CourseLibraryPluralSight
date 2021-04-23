using System;
using CourseLibrary.API.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CourseLibrary.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            // migrate the database.  Best practice = in Main, using service scope
            using (IServiceScope scope = host.Services.CreateScope())
            {
                try
                {
                    CourseLibraryContext context = scope.ServiceProvider.GetService<CourseLibraryContext>();
                    // for demo purposes, delete the database & migrate on startup so 
                    // we can start with a clean slate
                    context?.Database.EnsureDeleted();
                    context?.Database.Migrate();
                }
                catch (Exception ex)
                {
                    ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database");
                }
            }

            // run the web app
            host.Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}