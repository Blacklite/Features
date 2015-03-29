using System;

namespace Blacklite.Framework.Features.OptionsModel
{
    public interface IFeatureOptionsProvider
    {
        object GetOptions(Type optionsType);
    }
}
