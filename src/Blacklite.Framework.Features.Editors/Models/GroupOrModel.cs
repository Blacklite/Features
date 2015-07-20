using System;

namespace Blacklite.Framework.Features.Editors.Models
{
    public abstract class GroupOrModel
    {
        public GroupOrModel(string name)
        {
            Name = name.CamelCase();
        }
        
        public string Name { get; }
    }
}
