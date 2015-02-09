#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Temp.Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using Temp.Newtonsoft.Json.Converters;

namespace Temp.Newtonsoft.Json.Schema.Infrastructure.Licensing
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum LicenseType
    {
        Test,
        JsonSchemaIndie,
        JsonSchemaBusiness
    }
}
