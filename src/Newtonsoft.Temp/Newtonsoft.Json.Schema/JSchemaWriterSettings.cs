﻿#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Temp.Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;

namespace Temp.Newtonsoft.Json.Schema
{
    /// <summary>
    /// Specifies the settings used when writing a <see cref="JSchema"/>.
    /// </summary>
    public class JSchemaWriterSettings
    {
        /// <summary>
        /// Gets or sets the <see cref="ExternalSchema"/>s used when resolving the schema reference to write.
        /// </summary>
        /// <value>The <see cref="ExternalSchema"/>s used when resolving the schema reference to write.</value>
        public IList<ExternalSchema> ExternalSchemas { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaWriterSettings"/> class.
        /// </summary>
        public JSchemaWriterSettings()
        {
            ExternalSchemas = new List<ExternalSchema>();
        }
    }
}
