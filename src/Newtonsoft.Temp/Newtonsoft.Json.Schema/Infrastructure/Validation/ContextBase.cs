#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Temp.Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using System.Diagnostics;
using Temp.Newtonsoft.Json.Linq;

namespace Temp.Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal abstract class ContextBase
    {
        public Validator Validator;

        public List<Scope> Scopes
        {
            [DebuggerStepThrough]
            get { return Validator.Scopes; }
        }

        public JTokenWriter TokenWriter
        {
            get { return Validator.TokenWriter; }
            set { Validator.TokenWriter = value; }
        }

        protected ContextBase(Validator validator)
        {
            Validator = validator;
        }

        public abstract void RaiseError(string message, ErrorType errorType, JSchema schema, object value, IList<ValidationError> childErrors);
        public abstract bool HasErrors { get; }
    }
}
