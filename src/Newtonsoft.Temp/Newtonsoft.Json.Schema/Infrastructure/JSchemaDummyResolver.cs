#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Temp.Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Temp.Newtonsoft.Json.Schema.Infrastructure
{
    internal class JSchemaDummyResolver : JSchemaResolver
    {
        public static readonly JSchemaDummyResolver Instance = new JSchemaDummyResolver();

        public override JSchema GetSchema(ResolveSchemaContext context)
        {
            return null;
        }
    }
}
