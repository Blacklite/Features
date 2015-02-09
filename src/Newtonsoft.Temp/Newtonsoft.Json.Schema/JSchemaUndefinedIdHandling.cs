#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Temp.Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

namespace Temp.Newtonsoft.Json.Schema
{
    /// <summary>
    /// Specifies undefined schema Id handling options for the <see cref="JSchemaGenerator"/>.
    /// </summary>
    public enum JSchemaUndefinedIdHandling
    {
        /// <summary>
        /// Do not infer a schema Id.
        /// </summary>
        None = 0,

        /// <summary>
        /// Use the .NET type name as the schema Id.
        /// </summary>
        UseTypeName = 1,

        /// <summary>
        /// Use the assembly qualified .NET type name as the schema Id.
        /// </summary>
        UseAssemblyQualifiedName = 2,
    }
}
