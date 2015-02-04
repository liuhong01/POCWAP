//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using Microsoft.Azure.Portal.Configuration;
using Microsoft.WindowsAzurePack.Samples.HelloWorld.ApiClient;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.TenantExtension
{
    public static class ClientFactory
    {
        //Get Service Management API endpoint
        private static Uri tenantApiUri;

        private static BearerMessageProcessingHandler messageHandler;

        //This client is used to communicate with the Hello World resource provider
        private static Lazy<HelloWorldClient> helloWorldRestClient = new Lazy<HelloWorldClient>(
           () => new HelloWorldClient(tenantApiUri, messageHandler),
           LazyThreadSafetyMode.ExecutionAndPublication);

        static ClientFactory()
        {
            tenantApiUri = new Uri(AppManagementConfiguration.Instance.RdfeUnifiedManagementServiceUri);
            messageHandler = new BearerMessageProcessingHandler(new WebRequestHandler());
        }

        public static HelloWorldClient HelloWorldClient
        {
            get
            {
                return helloWorldRestClient.Value;
            }
        }
    }
}
