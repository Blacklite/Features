using System;

namespace Blacklite.Framework.Features.Editors.Models
{
    public abstract class EditorGroupOrModel
    {
        public EditorGroupOrModel(string name)
        {
            Name = name.CamelCase();
        }
        public string Name { get; }
    }
}
