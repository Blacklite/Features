﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.EditorModel
{
    public interface IFeatureEditorFactory
    {
        FeatureEditor GetFeatureEditor();
    }

    public class FeatureEditorFactory : IFeatureEditorFactory
    {
        private readonly IFeatureDescriberProvider _describerProvider;
        private readonly IServiceProvider _serviceProvider;

        public FeatureEditorFactory(IFeatureDescriberProvider describerProvider, IServiceProvider serviceProvider)
        {
            _describerProvider = describerProvider;
            _serviceProvider = serviceProvider;
        }

        public FeatureEditor GetFeatureEditor()
        {
            var featureDescribers = _describerProvider.Describers.Values;

            var childFeatures = featureDescribers
                .SelectMany(x => x.Children)
                .Distinct();

            var models = featureDescribers
                .Except(childFeatures)
                .OrderBy(x => x.FeatureType.Name)
                .Select(x => new FeatureModel(x));

            return new FeatureEditor(models, GetFeature, GetFeatureOptions);
        }

        private IFeature GetFeature(Type type)
        {
            return (IFeature)_serviceProvider.GetService(type);
        }

        private object GetFeatureOptions(Type type)
        {
            var describer = _describerProvider.Describers[type];
            if (!describer.HasOptions)
                return null;

            return _serviceProvider.GetService(describer.OptionsType);
        }
    }
}