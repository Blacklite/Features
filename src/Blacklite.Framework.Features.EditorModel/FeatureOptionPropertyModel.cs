using System;

namespace Blacklite.Framework.Features.EditorModel
{
    public class FeatureOptionPropertyModel
    {
        public FeatureOptionPropertyModel(Type type, string name, string title, string description, Func<object, object> getValue, Action<object, object> setValue, bool isReadOnly = false, bool optionsHasIsEnabled = false)
        {
            Type = type;
            Name = name.CamelCase();
            Title = title;
            Description = description;
            GetValue = getValue;
            SetValue = setValue;
            IsReadOnly = isReadOnly;
            OptionsHasIsEnabled = optionsHasIsEnabled;
        }
        public Type Type { get; }
        public string Name { get; }
        public string Title { get; }
        public string Description { get; }
        public Func<object, object> GetValue { get; }
        public Action<object, object> SetValue { get; }
        public bool IsReadOnly { get; }
        public bool OptionsHasIsEnabled { get; }
    }
}
