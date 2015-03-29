using Blacklite.Framework.Features.OptionsModel;
using System.Reflection;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;

namespace Blacklite.Framework.Features.Composition
{
    public class OptionsFeatureComposer : IFeatureComposition, IPreFeatureComposition
    {
        private readonly IFeatureFactory _featureFactory;
        private readonly IFeatureOptionsProvider _optionsProvider;

        public OptionsFeatureComposer(IFeatureFactory featureFactory,
            IFeatureOptionsProvider optionsProvider)
        {
            _featureFactory = featureFactory;
            _optionsProvider = optionsProvider;
        }

        public int Priority { get; } = int.MaxValue;

        public T Configure<T>(T feature, IFeatureDescriber describer)
        {
            var traitOptions = feature as IFeatureOptions;
            if (traitOptions != null)
            {
                object options;
                if (typeof(IFeature).GetTypeInfo().IsAssignableFrom(describer.Options.TypeInfo))
                {
                    options = _featureFactory.GetFeature(describer.Options.Type);
                }
                else
                {
                    options = _optionsProvider.GetOptions(describer.Options.Type);
                }
                traitOptions.SetOptions(options);
            }

            return feature;
        }

        public bool IsApplicableTo(IFeatureDescriber describer)
        {
            return describer.HasOptions;
        }
    }
}
