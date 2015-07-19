using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Blacklite.Framework.Features;
using System.Threading.Tasks;
using Microsoft.Framework.Configuration;
using Microsoft.AspNet.Hosting;

namespace Http.Sample
{
    public class FeatureA : ObservableSwitch { }
    public class FeatureB : ObservableSwitch { }
    public class FeatureC : Switch { }

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Setup configuration sources.
            Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfiguration Configuration { get; set; }

        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFeaturesHttp()
                    .AddFeaturesConfiguration(Configuration, z => true);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseFeaturesHttp("/features");

            app.UseMiddleware<MiddlewareA>();
            app.UseMiddleware<MiddlewareB>();
            app.UseMiddleware<MiddlewareC>();
            app.Use(DoFallback);
        }

        private async Task DoFallback(HttpContext httpContext, Func<Task> next)
        {
            await httpContext.Response.WriteAsync("Fallback");
        }
    }

    public class MiddlewareA
    {
        private readonly ObservableFeature<FeatureA> _feature;
        private readonly RequestDelegate _next;

        public MiddlewareA(RequestDelegate next, ObservableFeature<FeatureA> feature)
        {
            _next = next;
            _feature = feature;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (_feature.Value.IsEnabled)
                await httpContext.Response.WriteAsync("FeatureA");
            else
                await _next(httpContext);
        }
    }

    public class MiddlewareB
    {
        private readonly ObservableFeature<FeatureB> _feature;
        private readonly RequestDelegate _next;

        public MiddlewareB(RequestDelegate next, ObservableFeature<FeatureB> feature)
        {
            _next = next;
            _feature = feature;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (_feature.Value.IsEnabled)
                await httpContext.Response.WriteAsync("FeatureB");
            else
                await _next(httpContext);
        }
    }

    public class MiddlewareC
    {
        private readonly RequestDelegate _next;

        public MiddlewareC(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, Feature<FeatureC> feature)
        {
            if (feature.Value.IsEnabled)
                await httpContext.Response.WriteAsync("FeatureC");
            else
                await _next(httpContext);
        }
    }
}
