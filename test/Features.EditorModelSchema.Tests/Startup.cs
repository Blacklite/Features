using System;
using System.Linq;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Console;
using Blacklite.Framework.Features;
using Features.EditorModelSchema.Tests.Features;
using Blacklite.Framework.Features.Editors;
using System.Collections.Generic;

namespace Features.EditorModelSchema.Tests
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Setup configuration sources.
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC services to the services container.
            services.AddMvc();

            // Uncomment the following line to add Web API servcies which makes it easier to port Web API 2 controllers.
            // You need to add Microsoft.AspNet.Mvc.WebApiCompatShim package to project.json
            // services.AddWebApiConventions();
            services.AddFeatures()
                .AddFeatureEditorModel()
                .AddFeaturesMvc()
                .AddFeaturesHttp()
                .AddFeaturesConfiguration(Configuration.GetConfigurationSection("Features"));
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            // Configure the HTTP request pipeline.
            // Add the console logger.
            loggerfactory.AddConsole();

            // Add the following to the request pipeline only in development environment.
            if (string.Equals(env.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
                app.UseErrorPage(ErrorPageOptions.ShowAll);
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // send the request to the following path or controller action.
                app.UseErrorHandler("/Home/Error");
            }

            app.UseFeaturesHttp("/features");
            // Add static files to the request pipeline.
            app.UseStaticFiles();

            app.Map("/configuration", b =>
            {
                b.Use(async (httpContext, next) =>
                {
                    var root = this.Configuration as IConfigurationSource;
                    var keys = root.OfType<BaseConfigurationSource>()
                        .SelectMany(z => z.Data)
                        .Where(x => x.Key.StartsWith("Features:", StringComparison.Ordinal))
                        .GroupBy(z => z.Key)
                        .Select(z => z.First())
                        .ToArray();
                    await httpContext.Response.WriteAsync(string.Join("\n", keys.Select(z => $"{z.Key}: {z.Value}")));
                });
            });


            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                // Uncomment the following line to add a route for porting Web API 2 controllers.
                // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
            });
        }
    }
}
