using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features.Editors.Models
{
    public class EditorGroup : EditorGroupOrModel
    {
        public EditorGroup(string name) : base(name)
        {
            Title = name;
            Items = new List<EditorGroupOrModel>();
        }

        public string Title { get; }
        public IList<EditorGroupOrModel> Items { get; }
    }
}
