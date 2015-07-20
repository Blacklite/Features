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

    [FeatureGroup("API", "Development"), ConfigurationFeature]
    [FeatureDescription("Description text...")]
    public class ApiDevelopmentFeatureA : Feature.AlwaysOn, IObservableFeature
    {
        public string CustomProperty { get; set; }
    }

    [FeatureGroup("API", "Development"), ConfigurationFeature]
    public class ApiDevelopmentFeatureB : Feature.AlwaysOff { }

    [RequiredFeature(typeof(ApiDevelopmentFeatureD))]
    [FeatureGroup("API", "Development"), ConfigurationFeature]
    public class ApiDevelopmentFeatureCeeOptions : Switch
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("API", "Development"), ConfigurationFeature]
    public class ApiDevelopmentFeatureC : Feature.AlwaysOn<ApiDevelopmentFeatureCeeOptions> { }

    [ParentFeature(typeof(ApiDevelopmentFeatureA)), ConfigurationFeature]
    public class ApiDevelopmentFeatureD : Switch { }

    [FeatureGroup("API", "General"), ConfigurationFeature]
    public class ApiGeneralFeatureA : Feature.AlwaysOn, IObservableSwitch
    {
        public string CustomProperty { get; set; }
    }

    [FeatureGroup("API", "General"), ConfigurationFeature]
    public class ApiGeneralFeatureB : Feature.AlwaysOff { }

    public class ApiGeneralFeatureCeeOptions
    {
        public string SomeCustomOptionHere { get; set; }
        public CeeOptions Options { get; set; }
    }

    [FeatureGroup("API", "General"), ConfigurationFeature]
    public class ApiGeneralFeatureC : Feature.AlwaysOn<ApiGeneralFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApiGeneralFeatureA)), ConfigurationFeature]
    public class ApiGeneralFeatureD : Switch
    {
    }
    [FeatureGroup("Application", "Development"), ConfigurationFeature]
    public class ApplicationDevelopmentFeatureA : Feature.AlwaysOn, IObservableSwitch
    {
        public string CustomProperty { get; set; }
    }

    [FeatureGroup("Application", "Development"), ConfigurationFeature]
    public class ApplicationDevelopmentFeatureB : Feature.AlwaysOff { }

    public class ApplicationDevelopmentFeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("Application", "Development"), ConfigurationFeature]
    public class ApplicationDevelopmentFeatureC : Feature.AlwaysOn<ApplicationDevelopmentFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApplicationDevelopmentFeatureC))]
    [FeatureGroup("Application", "Development"), ConfigurationFeature]
    public class ApplicationDevelopmentFeatureD : Switch
    {
    }

    [FeatureGroup("Application", "General"), ConfigurationFeature]
    public class ApplicationGeneralFeatureA : Feature.AlwaysOn, IObservableSwitch
    {
        public string CustomProperty { get; set; }
    }

    [FeatureGroup("Application", "General"), ConfigurationFeature]
    public class ApplicationGeneralFeatureB : Feature.AlwaysOff { }

    public class ApplicationGeneralFeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("Application", "General"), ConfigurationFeature]
    public class ApplicationGeneralFeatureC : Feature.AlwaysOn<ApplicationGeneralFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApplicationGeneralFeatureB))]
    [FeatureGroup("Application", "General"), ConfigurationFeature]
    public class ApplicationGeneralFeatureD : Feature
    {
    }

    [FeatureGroup("Application", "Security"), ConfigurationFeature]
    public class ApplicationSecurityFeatureA : Feature.AlwaysOn, IObservableSwitch
    {
        public string CustomProperty { get; set; }
    }

    [FeatureGroup("Application", "Security"), ConfigurationFeature]
    public class ApplicationSecurityFeatureB : Feature.AlwaysOff { }

    public class ApplicationSecurityFeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("Application", "Security"), ConfigurationFeature]
    public class ApplicationSecurityFeatureC : Feature.AlwaysOn<ApplicationSecurityFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApplicationSecurityFeatureA))]
    [FeatureGroup("Application", "Security"), ConfigurationFeature]
    public class ApplicationSecurityFeatureD : Switch
    {
    }

    [FeatureGroup("Application", "User Profile"), ConfigurationFeature]
    public class ApplicationUserProfileFeatureA : Feature.AlwaysOn, IObservableSwitch
    {
        public string CustomProperty { get; set; }
    }

    [FeatureGroup("Application", "User Profile"), ConfigurationFeature]
    public class ApplicationUserProfileFeatureB : Feature.AlwaysOff { }

    public class ApplicationUserProfileFeatureCeeOptions
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CeeOptions Options { get; }
    }

    [FeatureGroup("Application", "User Profile"), ConfigurationFeature]
    public class ApplicationUserProfileFeatureC : Feature.AlwaysOn<ApplicationUserProfileFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApplicationUserProfileFeatureC)), ConfigurationFeature]
    [FeatureGroup("Application", "User Profile")]
    public class ApplicationUserProfileFeatureD : Switch
    {
    }

    [FeatureGroup("Application", "User Interface"), ConfigurationFeature]
    public class ApplicationUserInterfaceFeatureA : Feature.AlwaysOn, IObservableSwitch
    {
        public string CustomProperty { get; set; }
    }

    [FeatureGroup("Application", "User Interface"), ConfigurationFeature]
    public class ApplicationUserInterfaceFeatureB : Feature.AlwaysOff { }

    [FeatureGroup("Application", "User Interface"), ConfigurationFeature]
    public class ApplicationUserInterfaceFeatureCeeOptions : Feature
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CeeOptions Options { get; }
    }

    [RequiredFeature(typeof(ApplicationUserInterfaceFeatureD), false)]
    [FeatureGroup("Application", "User Interface"), ConfigurationFeature]
    public class ApplicationUserInterfaceFeatureC : Switch//<ApplicationUserInterfaceFeatureCeeOptions>
    {
    }

    [ParentFeature(typeof(ApplicationUserInterfaceFeatureB))]
    [FeatureGroup("Application", "User Interface"), ConfigurationFeature]
    public class ApplicationUserInterfaceFeatureD : Switch
    {
    }
}
