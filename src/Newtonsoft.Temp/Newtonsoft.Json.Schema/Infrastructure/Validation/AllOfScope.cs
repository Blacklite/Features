﻿#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Temp.Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Temp.Newtonsoft.Json.Utilities;

namespace Temp.Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class AllOfScope : ConditionalScope
    {
        public AllOfScope(SchemaScope parent, ContextBase context, int depth)
            : base(context, parent, depth)
        {
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            if (depth == InitialDepth && JsonTokenHelpers.IsPrimitiveOrEndToken(token))
            {
                if (!GetChildren().All(IsValidPredicate))
                {
                    List<int> invalidIndexes = new List<int>();
                    int index = 0;
                    foreach (SchemaScope schemaScope in GetChildren())
                    {
                        if (!schemaScope.IsValid)
                            invalidIndexes.Add(index);

                        index++;
                    }

                    string message = "JSON does not match all schemas from 'allOf'. Invalid schema indexes: {0}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", invalidIndexes));
                    RaiseError(message, ErrorType.AllOf, ParentSchemaScope.Schema, null, ConditionalContext.Errors);
                }

                return true;
            }

            return false;
        }
    }
}
