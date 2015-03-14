using Blacklite.Framework.Features.OptionsModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.EditorModel
{
    public interface IFeatureEditorFactory
    {
        IFeatureEditor GetFeatureEditor();
    }

    public class FeatureEditorFactory<T> : IFeatureEditorFactory
        where T : IFeatureDescriberEnumerable
    {
        private readonly IFeatureDescriberProvider _describerProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly T _describers;

        public FeatureEditorFactory(IFeatureDescriberProvider describerProvider, T describers, IServiceProvider serviceProvider)
        {
            _describers = describers;
            _describerProvider = describerProvider;
            _serviceProvider = serviceProvider;
        }

        private static string GroupId(IFeatureDescriber describer, string group)
        {
            return string.Join(":", describer.Groups.TakeWhile(z => z != group).Concat(new[] { group }));
        }

        public IFeatureEditor GetFeatureEditor()
        {
            var groups = _describers
                .OrderBy(x => x.FeatureType.Name)
                .SelectMany(x => x.Groups, (Describer, Group) => GroupId(Describer, Group))
                .Distinct();

            var groupingContainer = new Dictionary<string, FeatureGroupOrModel>();
            foreach (var group in groups)
            {
                FeatureGroupOrModel result;
                var subgroups = group.Split(':');
                string previous = null;
                FeatureGroup previousContainer = null;
                foreach (var subgroup in subgroups)
                {
                    if (!groupingContainer.TryGetValue(group, out result))
                    {
                        var groupName = group.Split(':');
                        result = new FeatureGroup(groupName[groupName.Length - 1]);
                        groupingContainer.Add(group, result);
                    }

                    if (previous != null)
                    {
                        previousContainer = (FeatureGroup)groupingContainer[previous];
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
                .Cast<FeatureGroup>();

            var groupedModels = _describers
                .OrderBy(x => x.FeatureType.Name)
                .GroupBy(x => string.Join(":", x.Groups))
                .Select(x => new { x.Key, Models = x.Select(z => new FeatureModel(z)) });

            var groupings = new List<FeatureGroupOrModel>();
            foreach (var group in groupedModels)
            {
                if (!string.IsNullOrWhiteSpace(group.Key))
                {
                    var grouping = (FeatureGroup)groupingContainer[group.Key];
                    foreach (var model in group.Models)
                    {
                        grouping.Items.Add(model);
                    }
                }
            }

            var models = groupedModels.SelectMany(x => x.Models);

            return new FeatureEditor(models, rootGroupings, GetFeature, GetFeatureOptions);
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

            var service = _serviceProvider.GetService(typeof(IFeatureOptions<>).MakeGenericType(describer.OptionsType)) as IFeatureOptions<object>;
            return service.Options;
        }
    }

    public class DefaultFeatureEditorFactory : FeatureEditorFactory<DefaultFeatureDescriberEnumerable>
    {
        public DefaultFeatureEditorFactory(IFeatureDescriberProvider describerProvider, DefaultFeatureDescriberEnumerable describers, IServiceProvider serviceProvider)
            : base(describerProvider, describers, serviceProvider)
        {
        }
    }
}
