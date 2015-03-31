using System;

namespace Blacklite.Framework.Features.Editors.Models
{
    public class EditorOptionPropertyModel
    {
        public EditorOptionPropertyModel(Type type, string name, string title, string description, Func<object, object> getValue, Action<object, object> setValue, bool isReadOnly = false)
        {
            Type = type;
            Name = name.CamelCase();
            Title = title;
            Description = description;
            GetValue = getValue;
            SetValue = setValue;
            IsReadOnly = isReadOnly;
        }
        public Type Type { get; }
        public string Name { get; }
        public string Title { get; }
        public string Description { get; }
        public Func<object, object> GetValue { get; }
        public Action<object, object> SetValue { get; }
        public bool IsReadOnly { get; }
    }
}
