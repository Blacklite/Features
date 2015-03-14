using Blacklite.Framework.Features;
using System;
using Blacklite.Framework.Features.OptionsModel;
using Microsoft.Framework.DependencyInjection;

namespace Features.EditorModelSchema.Tests.Features
{
    public enum CeeOptions
    {
        Private,
        Public,
        Protected
    }


    [FeatureGroup("API", "Development")]
    public class ApiDevelopmentFeatureA : Feature.AlwaysOn, IObservableFeature
    {
        public string CustomProperty { get; set; }
    }

    [FeatureGroup("API", "Development")]
    public class ApiDevelopmentFeatureB : Feature.AlwaysOff { }

    [FeatureGroup("API", "Development")]
    public class ApiDevelopmentFeatureCeeOptions : Switch
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("API", "Development")]
    public class ApiDevelopmentFeatureC : Feature.AlwaysOn<ApiDevelopmentFeatureCeeOptions> { }

    [ParentFeature(typeof(ApiDevelopmentFeatureA))]
    public class ApiDevelopmentFeatureD : Switch { }

    [FeatureGroup("API", "General")]
    public class ApiGeneralFeatureA : Feature.AlwaysOn, IObservableSwitch
    {
        public string CustomProperty { get; set; }
    }

    [FeatureGroup("API", "General")]
    public class ApiGeneralFeatureB : Feature.AlwaysOff { }

    public class ApiGeneralFeatureCeeOptions
    {
        public string SomeCustomOptionHere { get; set; }
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
    public class ApplicationDevelopmentFeatureA : Feature.AlwaysOn, IObservableSwitch
    {
        public string CustomProperty { get; set; }
    }

    [FeatureGroup("Application", "Development")]
    public class ApplicationDevelopmentFeatureB : Feature.AlwaysOff { }

    public class ApplicationDevelopmentFeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("Application", "Development")]
    public class ApplicationDevelopmentFeatureC : Feature.AlwaysOn<ApplicationDevelopmentFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApplicationDevelopmentFeatureC))]
    [FeatureGroup("Application", "Development")]
    public class ApplicationDevelopmentFeatureD : Switch
    {
    }

    [FeatureGroup("Application", "General")]
    public class ApplicationGeneralFeatureA : Feature.AlwaysOn, IObservableSwitch
    {
        public string CustomProperty { get; set; }
    }

    [FeatureGroup("Application", "General")]
    public class ApplicationGeneralFeatureB : Feature.AlwaysOff { }

    public class ApplicationGeneralFeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("Application", "General")]
    public class ApplicationGeneralFeatureC : Feature.AlwaysOn<ApplicationGeneralFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApplicationGeneralFeatureB))]
    [FeatureGroup("Application", "General")]
    public class ApplicationGeneralFeatureD : Feature
    {
    }

    [FeatureGroup("Application", "Security")]
    public class ApplicationSecurityFeatureA : Feature.AlwaysOn, IObservableSwitch
    {
        public string CustomProperty { get; set; }
    }

    [FeatureGroup("Application", "Security")]
    public class ApplicationSecurityFeatureB : Feature.AlwaysOff { }

    public class ApplicationSecurityFeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("Application", "Security")]
    public class ApplicationSecurityFeatureC : Feature.AlwaysOn<ApplicationSecurityFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApplicationSecurityFeatureA))]
    [FeatureGroup("Application", "User Profile")]
    public class ApplicationSecurityFeatureD : Switch
    {
    }

    [FeatureGroup("Application", "User Profile")]
    public class ApplicationUserProfileFeatureA : Feature.AlwaysOn, IObservableSwitch
    {
        public string CustomProperty { get; set; }
    }

    [FeatureGroup("Application", "User Profile")]
    public class ApplicationUserProfileFeatureB : Feature.AlwaysOff { }

    public class ApplicationUserProfileFeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("Application", "User Profile")]
    public class ApplicationUserProfileFeatureC : Feature.AlwaysOn<ApplicationUserProfileFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApplicationUserProfileFeatureC))]
    [FeatureGroup("Application", "User Profile")]
    public class ApplicationUserProfileFeatureD : Switch
    {
    }

    [FeatureGroup("Application", "User Interface")]
    public class ApplicationUserInterfaceFeatureA : Feature.AlwaysOn, IObservableSwitch
    {
        public string CustomProperty { get; set; }
    }

    [FeatureGroup("Application", "User Interface")]
    public class ApplicationUserInterfaceFeatureB : Feature.AlwaysOff { }

    [FeatureGroup("Application", "User Interface")]
    public class ApplicationUserInterfaceFeatureCeeOptions : Feature
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("Application", "User Interface")]
    public class ApplicationUserInterfaceFeatureC : Switch//<ApplicationUserInterfaceFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApplicationUserInterfaceFeatureB))]
    [FeatureGroup("Application", "User Interface")]
    public class ApplicationUserInterfaceFeatureD : Switch
    {
    }
}
