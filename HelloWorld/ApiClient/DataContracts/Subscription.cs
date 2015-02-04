// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.ApiClient.DataContracts
{
    /// <summary>    
    /// This is a data contract class between extensions and resource provider
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public class Subscription : IExtensibleDataObject
    {
        [DataMember(Order = 1)]
        public string SubscriptionId { get; set; }

        [DataMember(Order = 2)]
        public string AdminId { get; set; }

        [DataMember(Order = 3)]
        public string SubscriptionName { get; set; }

        [DataMember(Order = 4)]
        public string CoAdminIds { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
