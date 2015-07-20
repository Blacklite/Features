using System;
using System.Collections.Generic;

namespace Blacklite.Framework.Features.Editors.Models
{
    public class Group : GroupOrModel
    {
        public Group(string name) : base(name)
        {
            Title = name;
            Items = new List<GroupOrModel>();
        }

        public string Title { get; }
        public IList<GroupOrModel> Items { get; }
    }
}
