using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.SqlServer;
using ElsaQuickstarts.Server.ApiEndpoints.Activities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ElsaQuickstarts.Server.ApiEndpoints
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        private IWebHostEnvironment Environment { get; }
        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var elsaSection = Configuration.GetSection("Elsa");
            var connectionString = Configuration.GetConnectionString("SqlServer");

            if (connectionString is null) throw new Exception("Sql Server Connection String is not defined or missing in app settings");

            // Elsa services.
            //DB EF Core: Turned On. Enabling it will allow all workflows to be saved into pertaining database to maintain
            //Console: Turned On
            //Quartz Schedule/Jobs: Turned On
            //Javascript based DOM: Turned On
            //Custom Activities: Loaded On
            //Custom Workflows: Turned Off for now to avoid logging

            services
                .AddElsa(elsa => elsa
                    .UseEntityFrameworkPersistence(ef => ef.UseSqlServer(connectionString))
                    .AddConsoleActivities()
                    .AddHttpActivities(elsaSection.GetSection("Server").Bind)
                    .AddQuartzTemporalActivities()
                    .AddJavaScriptActivities()
                    .AddActivitiesFrom(typeof(IQCustomActivity))
                    //.AddWorkflowsFrom<Startup>() 
                    .AddActivity<WriteToFileActivity>() //Alternatively Type of IQActivity interface can be used to load all activities of that type.
                );

            // Elsa API endpoints.
            services.AddElsaApiEndpoints();

            // Allow arbitrary client browser apps to access the API.
            // In a production environment, make sure to allow only origins you trust.
            services.AddCors(cors => cors.AddDefaultPolicy(policy => policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .WithExposedHeaders("Content-Disposition"))
            );
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseCors()
                .UseHttpActivities()
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
                    endpoints.MapControllers();
                });
        }
    }
}
