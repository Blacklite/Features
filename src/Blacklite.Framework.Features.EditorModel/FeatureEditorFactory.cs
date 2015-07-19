using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Editors.Factory;
using Blacklite.Framework.Features.Editors.Models;
using Blacklite.Framework.Features.OptionsModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.Editors
{
    public interface IFeatureEditorFactory
    {
        IFeatureEditor GetFeatureEditor();
    }

    public class FeatureEditorFactory<T> : IFeatureEditorFactory
        where T : FeatureDescriberCollection
    {
        private readonly IFeatureDescriberProvider _describerProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IFeatureManager _featureManager;
        private readonly EditorFeatureFactory _featureFactory;
        private readonly IEnumerable<IFeatureDescriber> _describers;

        public FeatureEditorFactory(IFeatureDescriberProvider describerProvider,
            IServiceProvider serviceProvider,
            IFeatureManager featureManager,
            EditorFeatureFactory featureFactory)
        {
            _describers = (IEnumerable<IFeatureDescriber>)Activator.CreateInstance(typeof(T), describerProvider.Describers.Values);
            _describerProvider = describerProvider;
            _serviceProvider = serviceProvider;
            _featureManager = featureManager;
            _featureFactory = featureFactory;
        }

        private static string GroupId(IFeatureDescriber describer, string group)
        {
            return string.Join(":", describer.Groups.TakeWhile(z => z != group).Concat(new[] { group }));
        }

        public IFeatureEditor GetFeatureEditor()
        {
            var groups = _describers
                .OrderBy(x => x.Type.Name)
                .SelectMany(x => x.Groups, (Describer, Group) => GroupId(Describer, Group))
                .Distinct();

            var groupingContainer = new Dictionary<string, EditorGroupOrModel>();
            foreach (var group in groups)
            {
                EditorGroupOrModel result;
                var subgroups = group.Split(':').Where(z => !string.IsNullOrWhiteSpace(z));
                string previous = null;
                EditorGroup previousContainer = null;
                foreach (var subgroup in subgroups)
                {
                    if (!groupingContainer.TryGetValue(group, out result))
                    {
                        var groupName = group.Split(':');
                        result = new EditorGroup(groupName[groupName.Length - 1]);
                        groupingContainer.Add(group, result);
                    }

                    if (previous != null)
                    {
                        previousContainer = (EditorGroup)groupingContainer[previous];
                        previousContainer.Items.Add(result);
                    }

                    if (previous == null)
                        previous = subgroup;
                    else
                        previous += ":" + subgroup;
                }
            }

            var rootGroupings = groupingContainer
                .Where(x => !x.Key.Contains(":"))
                .Select(x => x.Value)
                .OfType<EditorGroup>()
                .Cast<EditorGroupOrModel>();

            var optionEditorModels = new List<EditorModel>();

            var models = _describers.Select(z => new EditorModel(z)).ToArray();
            var optionFeatures = models.Join(models, x => x.FeatureType, x => x.OptionsType, (a, b) => new { Option = a, Feature = b }).ToArray();
            foreach (var item in optionFeatures)
            {
                item.Feature.OptionsFeature = item.Option;
            }
            var modelsDictionary = models
                .Where(x => x.Describer.Parent == null)
                .Except(optionFeatures.Select(z => z.Option))
                .ToDictionary(x => x.FeatureType.Name);

            var groupedModels = _describers
                .OrderBy(x => x.Type.Name)
                .GroupBy(x => string.Join(":", x.Groups))
                .Select(x => new
                {
                    x.Key,
                    Models = x
                        .Where(z => modelsDictionary.ContainsKey(z.Type.Name))
                        .Select(z => modelsDictionary[z.Type.Name])
                });

            if (groupedModels.Where(group => !string.IsNullOrWhiteSpace(group.Key)).Any())
            {
                foreach (var group in groupedModels.Where(group => !string.IsNullOrWhiteSpace(group.Key)))
                {
                    var grouping = (EditorGroup)groupingContainer[group.Key];
                    foreach (var model in group.Models)
                    {
                        grouping.Items.Add(model);
                    }
                }
            }
            else
            {
                rootGroupings = groupedModels.SelectMany(z => z.Models);
            }

            return new FeatureEditor(_featureManager, models, rootGroupings, GetFeature, GetFeatureOptions);
        }

        private IFeature GetFeature(Type type)
        {
            return _featureFactory.GetFeature(type);
        }

        private object GetFeatureOptions(Type type)
        {
            var describer = _describerProvider.Describers[type];
            if (!describer.HasOptions)
                return null;

            var service = _serviceProvider.GetService(typeof(IFeatureOptions<>).MakeGenericType(describer.Options.Type)) as IFeatureOptions<object>;
            return service.Options;
        }
    }
}
