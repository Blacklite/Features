using Blacklite.Framework.Features.Traits;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features
{
    class RequiredFeaturesService : IRequiredFeaturesService
    {
        private readonly IFeatureDescriberProvider _provider;
        private readonly IServiceProvider _serviceProvider;

        public RequiredFeaturesService(IServiceProvider serviceProvider, IFeatureDescriberProvider provider)
        {
            _provider = provider;
            _serviceProvider = serviceProvider;
        }

        private object GetRequiredFeature(IFeatureDescriber describer)
        {
            if (describer.IsObservable)
                return _serviceProvider.GetRequiredService(typeof(IObservableFeature<>).MakeGenericType(describer.FeatureType));

            return _serviceProvider.GetRequiredService(describer.FeatureType);
        }

        public IValidateFeatureService ValidateFeaturesAreInTheCorrectState(Type type)
        {
            var describer = _provider.Describers[type];
            var allFeatures = describer.DependsOn.Select(x => new
            {
                Service = GetRequiredFeature(x.Key),
                FeatureType = x.Key.FeatureType,
                IsObservable = x.Key.IsObservable,
                IsEnabled = x.Value
            });

            var simpleFeatures = allFeatures
                .Where(x => !x.IsObservable)
                .Select(x => new
                {
                    Service = (ITrait)x.Service,
                    x.IsEnabled
                })
                .Where(x => x.Service != null)
                .ToArray();

            var observableFeatures = allFeatures
                .Where(x => x.IsObservable)
                .ToArray() ;

            if (observableFeatures.Any())
            {
                var allNonObservableFeaturesAreEnabled = simpleFeatures
                    .All(z => z.Service.IsEnabled == z.IsEnabled);
                if (allNonObservableFeaturesAreEnabled)
                {
                    var observableSubscriptions = observableFeatures.Select(x =>
                    {
                        var tuple = InvokeSubscribeToObservableMethod(x.FeatureType, x.Service);

                        return new
                        {
                            x.IsEnabled,
                            Method = tuple.Item1,
                            Disposable = tuple.Item2
                        };
                    }).ToArray();

                    var disposables = observableSubscriptions.Select(x => x.Disposable).ToArray();
                    return new ValidateFeatureService(() => observableSubscriptions.All(z => z.Method() == z.IsEnabled), disposables);
                }
            }

            if (simpleFeatures.Any())
            {
                var value = simpleFeatures.All(z => z.Service.IsEnabled == z.IsEnabled);
                return new ValidateFeatureService(() => value);
            }

            return new ValidateFeatureService(() => true);
        }

        private static MethodInfo SubscribeToObservableMethod = typeof(RequiredFeaturesService).GetTypeInfo()
            .GetDeclaredMethod(nameof(SubscribeToObservable));

        private static ConcurrentDictionary<Type, MethodInfo> _methods = new ConcurrentDictionary<Type, MethodInfo>();

        private static Tuple<Func<bool>, IDisposable> InvokeSubscribeToObservableMethod(Type featureType, object service)
        {
            return (Tuple<Func<bool>, IDisposable>)_methods.GetOrAdd(featureType, x =>
            {
                var observableFeatureType = typeof(IObservableFeature<>).MakeGenericType(featureType);
                return SubscribeToObservableMethod.MakeGenericMethod(observableFeatureType, featureType);
            }).Invoke(null, new object[] { service });
        }

        private static Tuple<Func<bool>, IDisposable> SubscribeToObservable<TObservable, TFeature>(TObservable observable)
            where TObservable : IObservableFeature<TFeature>
            where TFeature : IObservableTrait
        {
            var enabled = false;
            var disposable = observable.Subscribe(x => enabled = x.IsEnabled);
            return new Tuple<Func<bool>, IDisposable>(() => enabled, disposable);
        }
    }
}
