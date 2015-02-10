using System;

namespace Blacklite.Framework.Features.EditorModel
{
    public class FeatureOptionPropertyModel
    {
        public FeatureOptionPropertyModel(Type type, string name, string description, Func<object, object> getValue, bool isReadOnly = false, bool optionsHasIsEnabled = false)
        {
            Type = type;
            Name = name;
            Description = description;
            GetValue = getValue;
            IsReadOnly = isReadOnly;
            OptionsHasIsEnabled = optionsHasIsEnabled;
        }
        public Type Type { get; }
        public string Name { get; }
        public string Description { get; }
        public Func<object, object> GetValue { get; }
        public bool IsReadOnly { get; }
        public bool OptionsHasIsEnabled { get; }
    }
}
