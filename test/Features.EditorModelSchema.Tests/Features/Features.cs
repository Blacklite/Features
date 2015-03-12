using Blacklite.Framework.Features;
using System;
using Blacklite.Framework.Features.OptionModel;
using Microsoft.Framework.DependencyInjection;
using Blacklite.Framework.Features.Aspects;
using Blacklite.Framework.Features.Traits;

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
    public class ApiDevelopmentFeatureA : Feature.AlwaysOn, IObservableAspect { }

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
    public class ApiDevelopmentFeatureC : Feature.AlwaysOn<ApiDevelopmentFeatureCeeOptions>
    {
        public ApiDevelopmentFeatureC(IAspectOptions<ApiDevelopmentFeatureCeeOptions> _optionsContainer) : base(_optionsContainer)
        {
        }
    }

    [ParentFeature(typeof(ApiDevelopmentFeatureA))]
    public class ApiDevelopmentFeatureD : Trait
    {
        public ApiDevelopmentFeatureD(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures)
        {
        }
    }
    [FeatureGroup("API", "General")]
    public class ApiGeneralFeatureA : Feature.AlwaysOn, IObservableTrait { }

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
        public ApiGeneralFeatureC(IAspectOptions<ApiGeneralFeatureCeeOptions> _optionsContainer) : base(_optionsContainer)
        {
        }
    }

    [ParentFeature(typeof(ApiGeneralFeatureA))]
    public class ApiGeneralFeatureD : Trait
    {
        public ApiGeneralFeatureD(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures)
        {
        }
    }
    [FeatureGroup("Application", "Development")]
    public class ApplicationDevelopmentFeatureA : Feature.AlwaysOn, IObservableTrait { }

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
        public ApplicationDevelopmentFeatureC(IAspectOptions<ApplicationDevelopmentFeatureCeeOptions> _optionsContainer) : base(_optionsContainer)
        {
        }
    }

    [ParentFeature(typeof(ApplicationDevelopmentFeatureC))]
    public class ApplicationDevelopmentFeatureD : Trait
    {
        public ApplicationDevelopmentFeatureD(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures)
        {
        }
    }
    [FeatureGroup("Application", "General")]
    public class ApplicationGeneralFeatureA : Feature.AlwaysOn, IObservableTrait { }

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
        public ApplicationGeneralFeatureC(IAspectOptions<ApplicationGeneralFeatureCeeOptions> _optionsContainer) : base(_optionsContainer)
        {
        }
    }

    [ParentFeature(typeof(ApplicationGeneralFeatureB))]
    public class ApplicationGeneralFeatureD : Aspect
    {
        public ApplicationGeneralFeatureD() : base()
        {
        }
    }
    [FeatureGroup("Application", "Security")]
    public class ApplicationSecurityFeatureA : Feature.AlwaysOn, IObservableTrait { }

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
        public ApplicationSecurityFeatureC(IAspectOptions<ApplicationSecurityFeatureCeeOptions> _optionsContainer) : base(_optionsContainer)
        {
        }
    }

    [ParentFeature(typeof(ApplicationSecurityFeatureA))]
    public class ApplicationSecurityFeatureD : Trait
    {
        public ApplicationSecurityFeatureD(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures)
        {
        }
    }
    [FeatureGroup("Application", "User Profile")]
    public class ApplicationUserProfileFeatureA : Feature.AlwaysOn, IObservableTrait { }

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
        public ApplicationUserProfileFeatureC(IAspectOptions<ApplicationUserProfileFeatureCeeOptions> _optionsContainer) : base(_optionsContainer)
        {
        }
    }

    [ParentFeature(typeof(ApplicationUserProfileFeatureC))]
    public class ApplicationUserProfileFeatureD : Trait
    {
        public ApplicationUserProfileFeatureD(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures)
        {
        }
    }
    [FeatureGroup("Application", "User Interface")]
    public class ApplicationUserInterfaceFeatureA : Feature.AlwaysOn, IObservableTrait { }

    [FeatureGroup("Application", "User Interface")]
    public class ApplicationUserInterfaceFeatureB : Feature.AlwaysOff { }

    public class ApplicationUserInterfaceFeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsEnabled { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("Application", "User Interface")]
    public class ApplicationUserInterfaceFeatureC : Aspect<ApplicationUserInterfaceFeatureCeeOptions>
    {
        public ApplicationUserInterfaceFeatureC(IAspectOptions<ApplicationUserInterfaceFeatureCeeOptions> _optionsContainer) : base(_optionsContainer)
        {
        }
    }

    [ParentFeature(typeof(ApplicationUserInterfaceFeatureB))]
    public class ApplicationUserInterfaceFeatureD : Trait
    {
        public ApplicationUserInterfaceFeatureD(IRequiredFeaturesService requiredFeatures) : base(requiredFeatures)
        {
        }
    }
}
