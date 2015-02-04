// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.Common
{
    /// <summary>Describes a data set to be returned to the client</summary>
    public class PortalDataSet
    {
        private const int DataSetNormalPollingIntervalInSeconds = 60;
        private const int DataSetFastPollingIntervalInSeconds = 10;
        private const int DataSetSlowPollingIntervalInSeconds = 300;

        /// <summary>Backing field for eTag property</summary>
        private string entityTag;

        /// <summary>Backing field for data property</summary>
        private object dataObj;

        /// <summary>Provides backward compatibility with client side existing code</summary>
        // TODO: 285292 Remove when everyone is on the V2 data story
        public string namePropertyName 
        {
            get { return "id"; }
        }

        /// <summary>Provides backward compatibility with client side existing code</summary>
        // TODO: 285292 Remove when everyone is on the V2 data story
        public string displayNamePropertyName
        {
            get { return "displayName"; }
        }

        /// <summary>Provides backward compatibility with client side existing code</summary>
        // TODO: 285292 Remove when everyone is on the V2 data story
        public string collectionNamePropertyName
        {
            get { return "data"; }
        }

        /// <summary>Gets or sets the interval (in ms) at which the client should poll when an extension that uses this data set is active</summary>
        public int pollingInterval { get; set; }

        /// <summary>Gets or sets the interval (in ms) at which the client should poll when an extension that uses this data set is active and has pending changes</summary>
        /// <remarks>May be rounded up to a whole multiple of FastPollingInterval by the client</remarks>
        public int fastPollingInterval { get; set; }

        /// <summary>Gets or sets the interval (in ms) at which the client should poll when no extension is active</summary>
        /// <remarks>May be rounded up to a whole multiple of FastPollingInterval by the client</remarks>
        public int slowPollingInterval { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the data set is complete.
        /// </summary>
        /// <remarks>
        /// When set to false the returned data set will be buffered on the client and the data set's source Uri will
        /// be repeatedly called until the data set returns isComplete = true. At which point, the buffered data
        /// set will be used for the merge.
        /// </remarks>
        public bool isComplete { get; set; }

        /// <summary>
        /// Gets or sets a collection of partial data load errors
        /// </summary>
        public IEnumerable<PartialDataError> partialErrors { get; set; }

        /// <summary>The data</summary>
        public object data
        {
            get { return this.dataObj; }
            set
            {
                this.dataObj = value;
                this.entityTag = (value == null) ? string.Empty : CalculateETag(JavaScriptSerializerExtensions.CreateJavaScriptSerializer().Serialize(value));
            }
        }

        /// <summary>The eTag</summary>
        public string eTag { get { return this.entityTag; } }

        /// <summary>The merge method for the data set</summary>
        public string mergeMethod { get; set; }

        /// <summary>Creates a new instance of the PortalDataSetV2 class</summary>
        /// <param name="data">Array of data to be returned</param>
        /// <param name="namePropertyName">Name of the name property used for data merging</param>
        /// <param name="pollingInterval">The duration for which the item can persist on the client before it is considered to have expired when polling normally</param>
        /// <param name="fastPollingInterval">The duration for which the item can persist on the client before it is considered to have expired when polling fast</param>
        /// <param name="mergeMethod">The merge method</param>
        /// <param name="isComplete">Indicates whether the data set is complete or partial</param>
        public PortalDataSet(
            object data,             
            TimeSpan? pollingInterval = null, 
            TimeSpan? fastPollingInterval = null, 
            TimeSpan? slowPollingInterval = null,
            PortalDataSetMergeMethod mergeMethod = PortalDataSetMergeMethod.Cascade, 
            bool isComplete = true,
            IEnumerable<PartialDataError> partialErrors = null )
        {
            this.data = data ?? new object[] { };            
            this.pollingInterval = (int)(pollingInterval.HasValue ? pollingInterval.Value : TimeSpan.FromSeconds(DataSetNormalPollingIntervalInSeconds)).TotalMilliseconds;
            this.fastPollingInterval = (int)(fastPollingInterval.HasValue ? fastPollingInterval.Value : TimeSpan.FromSeconds(DataSetFastPollingIntervalInSeconds)).TotalMilliseconds;
            this.slowPollingInterval = (int)(slowPollingInterval.HasValue ? slowPollingInterval.Value : TimeSpan.FromSeconds(DataSetSlowPollingIntervalInSeconds)).TotalMilliseconds;
            this.mergeMethod = mergeMethod.ToString();
            this.isComplete = isComplete;
            this.partialErrors = partialErrors;
        }

        
        /// <summary>Calcualtes an ETag for a string</summary>
        /// <param name="representation">String for which the ETag is to be generated</param>
        /// <returns>The ETag</returns>
        public static string CalculateETag(string representation)
        {
            return CalculateETag(Encoding.UTF8.GetBytes(representation));
        }

        /// <summary>Calculates an ETag for a byte array</summary>
        /// <param name="source">Array for which teh ETag is to generated</param>
        /// <returns>The ETag</returns>
        public static string CalculateETag(byte[] source)
        {
            using (HashAlgorithm hash = SHA256.Create())
            {
                byte[] data = hash.ComputeHash(source);
                StringBuilder sb = new StringBuilder(data.Length * 2);
                foreach (byte b in data)
                {
                    sb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
                }

                return sb.ToString();
            }
        }
    }
}