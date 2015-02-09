﻿#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Temp.Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Temp.Newtonsoft.Json.Utilities;

namespace Temp.Newtonsoft.Json.Schema.Infrastructure
{
    internal static class Constants
    {
        public static readonly List<JsonToken> NumberTokens = new List<JsonToken> { JsonToken.Integer, JsonToken.Float };
        public static readonly List<JsonToken> DependencyTokens = new List<JsonToken> { JsonToken.StartObject, JsonToken.StartArray, JsonToken.String };

        public static class PropertyNames
        {
            public const string Type = "type";
            public const string Properties = "properties";
            public const string Items = "items";
            public const string AdditionalItems = "additionalItems";
            public const string Required = "required";
            public const string PatternProperties = "patternProperties";
            public const string AdditionalProperties = "additionalProperties";
            public const string Requires = "requires";
            public const string Dependencies = "dependencies";
            public const string Minimum = "minimum";
            public const string Maximum = "maximum";
            public const string ExclusiveMinimum = "exclusiveMinimum";
            public const string ExclusiveMaximum = "exclusiveMaximum";
            public const string MinimumItems = "minItems";
            public const string MaximumItems = "maxItems";
            public const string Pattern = "pattern";
            public const string MaximumLength = "maxLength";
            public const string MinimumLength = "minLength";
            public const string Enum = "enum";
            public const string ReadOnly = "readonly";
            public const string Title = "title";
            public const string Description = "description";
            public const string Format = "format";
            public const string Default = "default";
            public const string Transient = "transient";
            public const string DivisibleBy = "divisibleBy";
            public const string MultipleOf = "multipleOf";
            public const string Hidden = "hidden";
            public const string Disallow = "disallow";
            public const string Extends = "extends";
            public const string Id = "id";
            public const string UniqueItems = "uniqueItems";
            public const string MinimumProperties = "minProperties";
            public const string MaximumProperties = "maxProperties";

            public const string AnyOf = "anyOf";
            public const string AllOf = "allOf";
            public const string OneOf = "oneOf";
            public const string Not = "not";

            public const string Ref = "$ref";
        }

        public static class Formats
        {
            public const string Draft3Hostname = "host-name";
            public const string Draft3IPv4 = "ip-address";
            public const string Hostname = "hostname";
            public const string DateTime = "date-time";
            public const string Date = "date";
            public const string Time = "time";
            public const string UtcMilliseconds = "utc-millisec";
            public const string Regex = "regex";
            public const string Color = "color";
            public const string Style = "style";
            public const string Phone = "phone";
            public const string Uri = "uri";
            public const string IPv6 = "ipv6";
            public const string IPv4 = "ipv4";
            public const string Email = "email";
        }

        public static readonly IDictionary<string, JSchemaType> JSchemaTypeMapping = new Dictionary<string, JSchemaType>
        {
            { "string", JSchemaType.String },
            { "object", JSchemaType.Object },
            { "integer", JSchemaType.Integer },
            { "number", JSchemaType.Float },
            { "null", JSchemaType.Null },
            { "boolean", JSchemaType.Boolean },
            { "array", JSchemaType.Array },
            { "any", JSchemaType.Any }
        };
    }
}
