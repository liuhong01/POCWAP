//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.WindowsAzurePack.Samples.HelloWorld.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.TenantExtension.Models
{
    /// <summary>
    /// Data model for domain name tenant view
    /// </summary>    
    public class FileShareModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="FileShareModel" /> class.
        /// </summary>
        public FileShareModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileServerModel" /> class.
        /// </summary>
        /// <param name="ProductModel">The domain name from API.</param>
        public FileShareModel(FileShare fileShareFromApi)
        {
            this.Name = fileShareFromApi.Name;
            this.SubscriptionId = fileShareFromApi.SubscriptionId;
            this.FileServerName = fileShareFromApi.FileServerName;
            this.Size = fileShareFromApi.Size;            
        }

        /// <summary>
        /// Covert to the API object.
        /// </summary>
        /// <returns>The API DomainName data contract.</returns>
        public FileShare ToApiObject()
        {
            return new FileShare()
            {
                Name = this.Name,
                FileServerName = this.FileServerName,
                Size = this.Size,
                SubscriptionId = this.SubscriptionId
            };
        }

        /// <summary>
        /// Gets or sets the name.
        // </summary>
        public string Name { get; set; }              

        /// <summary>
        /// Gets or sets the value of the display name of the file server 
        /// </summary>
        public string FileServerName { get; set; }

        /// <summary>
        /// Gets or sets the value of the subscription id
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the value of the file share size
        /// </summary>
        public int Size { get; set; }

       
    }
}