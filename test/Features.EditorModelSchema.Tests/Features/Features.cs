using Blacklite.Framework.Features;
using System;
using Blacklite.Framework.Features.OptionsModel;
using Microsoft.Framework.DependencyInjection;

namespace Features.EditorModelSchema.Tests.Features
{
    public static class FeatureRegistry
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<ApiDevelopmentFeatureA>();
            services.AddTransient<ApiDevelopmentFeatureB>();
            services.AddTransient<ApiDevelopmentFeatureC>();
            services.AddTransient<ApiDevelopmentFeatureD>();

            services.AddSingleton<ApiGeneralFeatureA>();
            services.AddTransient<ApiGeneralFeatureB>();
            services.AddTransient<ApiGeneralFeatureC>();
            services.AddTransient<ApiGeneralFeatureD>();

            services.AddSingleton<ApplicationDevelopmentFeatureA>();
            services.AddTransient<ApplicationDevelopmentFeatureB>();
            services.AddTransient<ApplicationDevelopmentFeatureC>();
            services.AddTransient<ApplicationDevelopmentFeatureD>();

            services.AddSingleton<ApplicationGeneralFeatureA>();
            services.AddTransient<ApplicationGeneralFeatureB>();
            services.AddTransient<ApplicationGeneralFeatureC>();
            services.AddTransient<ApplicationGeneralFeatureD>();

            services.AddSingleton<ApplicationSecurityFeatureA>();
            services.AddTransient<ApplicationSecurityFeatureB>();
            services.AddTransient<ApplicationSecurityFeatureC>();
            services.AddTransient<ApplicationSecurityFeatureD>();

            services.AddSingleton<ApplicationUserProfileFeatureA>();
            services.AddTransient<ApplicationUserProfileFeatureB>();
            services.AddTransient<ApplicationUserProfileFeatureC>();
            services.AddTransient<ApplicationUserProfileFeatureD>();

            services.AddSingleton<ApplicationUserInterfaceFeatureA>();
            services.AddTransient<ApplicationUserInterfaceFeatureB>();
            services.AddTransient<ApplicationUserInterfaceFeatureC>();
            services.AddTransient<ApplicationUserInterfaceFeatureD>();
        }
    }


    public enum CeeOptions
    {
        Private,
        Public,
        Protected
    }


    [FeatureGroup("API", "Development")]
    public class ApiDevelopmentFeatureA : Feature.AlwaysOn, IObservableFeature { }

    [FeatureGroup("API", "Development")]
    public class ApiDevelopmentFeatureB : Feature.AlwaysOff { }

    public class ApiDevelopmentFeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsEnabled { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("API", "Development")]
    public class ApiDevelopmentFeatureC : Feature.AlwaysOn<ApiDevelopmentFeatureCeeOptions>    {    }

    [ParentFeature(typeof(ApiDevelopmentFeatureA))]
    public class ApiDevelopmentFeatureD : Switch { }

    [FeatureGroup("API", "General")]
    public class ApiGeneralFeatureA : Feature.AlwaysOn, IObservableSwitch { }

    [FeatureGroup("API", "General")]
    public class ApiGeneralFeatureB : Feature.AlwaysOff { }

    public class ApiGeneralFeatureCeeOptions
    {
        public string SomeCustomOptionHere { get; set; }
        public bool IsEnabled { get; set; }
        public CeeOptions Options { get; set; }
    }

    [FeatureGroup("API", "General")]
    public class ApiGeneralFeatureC : Feature.AlwaysOn<ApiGeneralFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApiGeneralFeatureA))]
    public class ApiGeneralFeatureD : Switch
    {
    }
    [FeatureGroup("Application", "Development")]
    public class ApplicationDevelopmentFeatureA : Feature.AlwaysOn, IObservableSwitch { }

    [FeatureGroup("Application", "Development")]
    public class ApplicationDevelopmentFeatureB : Feature.AlwaysOff { }

    public class ApplicationDevelopmentFeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsEnabled { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("Application", "Development")]
    public class ApplicationDevelopmentFeatureC : Feature.AlwaysOn<ApplicationDevelopmentFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApplicationDevelopmentFeatureC))]
    public class ApplicationDevelopmentFeatureD : Switch
    {
    }
    [FeatureGroup("Application", "General")]
    public class ApplicationGeneralFeatureA : Feature.AlwaysOn, IObservableSwitch { }

    [FeatureGroup("Application", "General")]
    public class ApplicationGeneralFeatureB : Feature.AlwaysOff { }

    public class ApplicationGeneralFeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsEnabled { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("Application", "General")]
    public class ApplicationGeneralFeatureC : Feature.AlwaysOn<ApplicationGeneralFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApplicationGeneralFeatureB))]
    public class ApplicationGeneralFeatureD : Feature
    {
    }
    [FeatureGroup("Application", "Security")]
    public class ApplicationSecurityFeatureA : Feature.AlwaysOn, IObservableSwitch { }

    [FeatureGroup("Application", "Security")]
    public class ApplicationSecurityFeatureB : Feature.AlwaysOff { }

    public class ApplicationSecurityFeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsEnabled { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("Application", "Security")]
    public class ApplicationSecurityFeatureC : Feature.AlwaysOn<ApplicationSecurityFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApplicationSecurityFeatureA))]
    public class ApplicationSecurityFeatureD : Switch
    {
    }
    [FeatureGroup("Application", "User Profile")]
    public class ApplicationUserProfileFeatureA : Feature.AlwaysOn, IObservableSwitch { }

    [FeatureGroup("Application", "User Profile")]
    public class ApplicationUserProfileFeatureB : Feature.AlwaysOff { }

    public class ApplicationUserProfileFeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsEnabled { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("Application", "User Profile")]
    public class ApplicationUserProfileFeatureC : Feature.AlwaysOn<ApplicationUserProfileFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApplicationUserProfileFeatureC))]
    public class ApplicationUserProfileFeatureD : Switch
    {
    }
    [FeatureGroup("Application", "User Interface")]
    public class ApplicationUserInterfaceFeatureA : Feature.AlwaysOn, IObservableSwitch { }

    [FeatureGroup("Application", "User Interface")]
    public class ApplicationUserInterfaceFeatureB : Feature.AlwaysOff { }

    [FeatureGroup("Application", "User Interface")]
    public class ApplicationUserInterfaceFeatureC/*ApplicationUserInterfaceFeatureCeeOptions*/ : Feature
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CeeOptions Options { get; }
    }

    //public class ApplicationUserInterfaceFeatureC : Feature<ApplicationUserInterfaceFeatureCeeOptions>
    //{
    //    public ApplicationUserInterfaceFeatureC(IFeatureOptions<ApplicationUserInterfaceFeatureCeeOptions> _optionsContainer) : base(_optionsContainer)
    //    {
    //    }
    //}

    [ParentFeature(typeof(ApplicationUserInterfaceFeatureB))]
    public class ApplicationUserInterfaceFeatureD : Switch
    {
    }
}
