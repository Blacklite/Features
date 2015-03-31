using Blacklite.Framework.Features.OptionsModel;
using System;
using System.Reactive.Subjects;

namespace Blacklite.Framework.Features
{
    public abstract class ObservableSwitch : Switch, IObservableSwitch
    {
    }

    public abstract class ObservableSwitch<TOptions> : ObservableSwitch, ISwitch<TOptions>
        where TOptions : class, new()
    {
        public TOptions Options { get; private set; }

        void IFeatureOptions.SetOptions(object options)
        {
            Options = (TOptions)options;
        }
    }
}
