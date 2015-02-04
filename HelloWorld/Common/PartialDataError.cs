// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.Common
{
    /// <summary>Describes an error encountered while developing a data set to be returned to the client</summary>
    public class PartialDataError
    {
        /// <summary>Gets or sets the localized error string</summary>
        public string errorString { get; set; }

        /// <summary>Gets or sets exception details if in development mode</summary>
        public string exceptionDetails { get; set; }

        /// <summary>Initializes a new instance of the <see cref="PartialDataError"/> class.
        /// Creates a new instance of the PartialDataError class</summary>
        /// <param name="errorString">A localized error string</param>
        /// <param name="exception">An exception handled as a partial error</param>
        public PartialDataError(string errorString, Exception exception)
        {
            this.errorString = errorString;
            this.exceptionDetails = exception.Message; // Check if you want to show this one.
        }
    }
}
