#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Temp.Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;

namespace Temp.Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class ValidatorContext : ContextBase
    {
        private bool _hasErrors;

        public ValidatorContext(Validator validator)
            : base(validator)
        {
        }

        public override void RaiseError(string message, ErrorType errorType, JSchema schema, object value, IList<ValidationError> childErrors)
        {
            _hasErrors = true;
            Validator.RaiseError(message, errorType, schema, value, childErrors);
        }

        public override bool HasErrors
        {
            get { return _hasErrors; }
        }
    }
}
