#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Temp.Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Temp.Newtonsoft.Json.Schema.Generation
{
    /// <summary>
    /// Customizes <see cref="JSchema"/> generation for a <see cref="Type"/>.
    /// </summary>
    public abstract class JSchemaGenerationProvider
    {
        /// <summary>
        /// Gets a <see cref="JSchema"/> for a <see cref="Type"/>.
        /// </summary>
        /// <param name="context">The <see cref="Type"/> and associated information used to generate a <see cref="JSchema"/>.</param>
        /// <returns>The generated <see cref="JSchema"/>.</returns>
        public abstract JSchema GetSchema(JSchemaTypeGenerationContext context);
    }
}
