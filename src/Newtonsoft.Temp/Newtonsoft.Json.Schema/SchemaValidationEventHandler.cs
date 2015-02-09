#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Temp.Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

namespace Temp.Newtonsoft.Json.Schema
{
    /// <summary>
    /// Represents the callback method that will handle JSON schema validation events and the <see cref="SchemaValidationEventArgs"/>.
    /// </summary>
    public delegate void SchemaValidationEventHandler(object sender, SchemaValidationEventArgs e);
}
