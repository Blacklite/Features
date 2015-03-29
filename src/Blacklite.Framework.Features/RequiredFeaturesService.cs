using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using Blacklite.Framework.Features.Observables;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features
{
    class RequiredFeaturesService : IRequiredFeaturesService
    {
        private readonly IFeatureDescriberProvider _provider;
        private readonly IFeatureFactory _factory;
        private readonly IObservableFeatureFactory _observableFeatureFactory;

        public RequiredFeaturesService(IFeatureFactory factory,
            IFeatureDescriberProvider provider,
            IObservableFeatureFactory observableFeatureFactory)
        {
            _provider = provider;
            _factory = factory;
            _observableFeatureFactory = observableFeatureFactory;
        }

        private object GetRequiredFeature(IFeatureDescriber describer)
        {
            if (describer.IsObservable)
                return _observableFeatureFactory.GetObservableFeature(describer.Type);

            return _factory.GetFeature(describer.Type);
        }

        private IEnumerable<FeatureDependency> GetFeatureDependencies(IFeatureDescriber describer)
        {
            return describer.DependsOn
                .Where(x => x.Key.HasEnabled)
                .Select(x =>
                new FeatureDependency()
                {
                    Service = GetRequiredFeature(x.Key),
                    FeatureType = x.Key.Type,
                    IsObservable = x.Key.IsObservable,
                    ShouldBeEnabled = x.Value
                });
        }

        private IEnumerable<SimpleFeatureWrapper> GetSimpleFeatures(IEnumerable<FeatureDependency> featureDependencies)
        {
            return featureDependencies
                .Where(x => !x.IsObservable)
                .Select(SimpleFeatureWrapper.CreateWrapper);
        }

        private IEnumerable<ObservableFeatureWrapper> GetObservableFeatures(IEnumerable<FeatureDependency> featureDependencies)
        {
            return featureDependencies
                .Where(x => x.IsObservable)
                .Select(ObservableFeatureWrapper.CreateWrapper);
        }

        public bool ValidateRequiredFeatures(Type type)
        {
            var featureDependencies = GetFeatureDependencies(_provider.Describers[type]);
            var simpleFeatures = GetSimpleFeatures(featureDependencies);
            var observableFeatures = GetObservableFeatures(featureDependencies);

            if (observableFeatures.Any())
            {
                var result = simpleFeatures.All(z => z.IsEnabled == z.ShouldBeEnabled);
                if (result)
                {
                    result = observableFeatures.All(z => z.IsEnabledMethod() == z.ShouldBeEnabled);
                }

                foreach (var disposable in observableFeatures.Select(x => x.Disposable))
                    disposable.Dispose();

                return result;
            }

            if (simpleFeatures.Any())
            {
                return simpleFeatures.All(z => z.IsEnabled == z.ShouldBeEnabled);
            }

            return true;
        }

        public IObservable<bool> GetObservableRequiredFeatures(Type type)
        {
            var featureDependencies = GetFeatureDependencies(_provider.Describers[type]);
            var simpleFeatures = GetSimpleFeatures(featureDependencies);
            var observableFeatures = featureDependencies
                .Where(x => x.IsObservable)
                .Select(EnabledObservableWrapper.CreateWrapper);
            if (observableFeatures.Any())
            {
                var allNonObservableFeaturesAreValid = simpleFeatures
                    .All(z => z.IsEnabled == z.ShouldBeEnabled);
                if (allNonObservableFeaturesAreValid)
                {
                    return Observable.Merge(observableFeatures);
                }
            }

            return Observable.Empty<bool>();
        }

        private class FeatureDependency
        {
            public object Service { get; set; }
            public Type FeatureType { get; set; }
            public bool IsObservable { get; set; }
            public bool ShouldBeEnabled { get; set; }
        }

        private class SimpleFeatureWrapper
        {
            public static SimpleFeatureWrapper CreateWrapper(FeatureDependency dependency)
            {
                return new SimpleFeatureWrapper()
                {
                    IsEnabled = GetTrait(dependency).IsEnabled,
                    ShouldBeEnabled = dependency.ShouldBeEnabled
                };
            }

            private static ISwitch GetTrait(FeatureDependency dependency)
            {
                return (ISwitch)dependency.Service;
            }

            public bool IsEnabled { get; set; }
            public bool ShouldBeEnabled { get; set; }
        }

        private class ObservableFeatureWrapper
        {
            public static ObservableFeatureWrapper CreateWrapper(FeatureDependency dependency)
            {
                var tuple = InvokeSubscribeToObservableMethod(dependency);
                return new ObservableFeatureWrapper()
                {
                    ShouldBeEnabled = dependency.ShouldBeEnabled,
                    IsEnabledMethod = tuple.Item1,
                    IsEnabledObservable = tuple.Item3,
                    Disposable = tuple.Item2
                };
            }

            private static MethodInfo SubscribeToObservableMethod = typeof(ObservableFeatureWrapper).GetTypeInfo()
                .GetDeclaredMethod(nameof(SubscribeToObservable));

            private static ConcurrentDictionary<Type, MethodInfo> _methods = new ConcurrentDictionary<Type, MethodInfo>();

            private static Tuple<Func<bool>, IDisposable, IObservable<bool>> InvokeSubscribeToObservableMethod(FeatureDependency dependency)
            {
                return (Tuple<Func<bool>, IDisposable, IObservable<bool>>)_methods.GetOrAdd(dependency.FeatureType, x =>
                {
                    var observableFeatureType = typeof(ObservableFeature<>).MakeGenericType(dependency.FeatureType);
                    return SubscribeToObservableMethod.MakeGenericMethod(observableFeatureType, dependency.FeatureType);
                }).Invoke(null, new object[] { dependency.Service });
            }

            private static Tuple<Func<bool>, IDisposable, IObservable<bool>> SubscribeToObservable<TObservable, TFeature>(TObservable observable)
                where TObservable : ObservableFeature<TFeature>
                where TFeature : IObservableSwitch
            {
                var enabled = false;
                // We can do this and be comfortable that our enabled flag will be populated because ObservableFeatures are powered by a behavior subject.
                var disposable = observable.Subscribe(x => enabled = x.IsEnabled);
                var boolObservable = observable.Select(x => x.IsEnabled);
                return new Tuple<Func<bool>, IDisposable, IObservable<bool>>(() => enabled, disposable, boolObservable);
            }

            public bool ShouldBeEnabled { get; set; }
            public Func<bool> IsEnabledMethod { get; set; }
            public IObservable<bool> IsEnabledObservable { get; set; }
            public IDisposable Disposable { get; set; }
        }

        private class EnabledObservableWrapper
        {
            public static IObservable<bool> CreateWrapper(FeatureDependency dependency)
            {
                return InvokeSubscribeToObservableMethod(dependency);
            }

            private static MethodInfo SubscribeToObservableMethod = typeof(EnabledObservableWrapper).GetTypeInfo()
                .GetDeclaredMethod(nameof(SubscribeToObservable));

            private static ConcurrentDictionary<Type, MethodInfo> _methods = new ConcurrentDictionary<Type, MethodInfo>();

            private static IObservable<bool> InvokeSubscribeToObservableMethod(FeatureDependency dependency)
            {
                return (IObservable<bool>)_methods.GetOrAdd(dependency.FeatureType, x =>
                {
                    var observableFeatureType = typeof(ObservableFeature<>).MakeGenericType(dependency.FeatureType);
                    return SubscribeToObservableMethod.MakeGenericMethod(observableFeatureType, dependency.FeatureType);
                }).Invoke(null, new object[] { dependency.Service });
            }

            private static IObservable<bool> SubscribeToObservable<TObservable, TFeature>(TObservable observable)
                where TObservable : ObservableFeature<TFeature>
                where TFeature : IObservableSwitch
            {
                var boolObservable = observable.Select(x => x.IsEnabled);
                return boolObservable;
            }

            public IObservable<bool> Observable { get; set; }
        }
    }
}
