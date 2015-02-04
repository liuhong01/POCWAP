//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using Microsoft.Azure.Portal.Configuration;
using Microsoft.WindowsAzurePack.Samples.HelloWorld.ApiClient;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.AdminExtension
{
    public static class ClientFactory
    {
        //Get Service Management API endpoint
        private static Uri adminApiUri;

        private static BearerMessageProcessingHandler messageHandler;

        //This client is used to communicate with the Hello World resource provider
        private static Lazy<HelloWorldClient> helloWorldRestClient = new Lazy<HelloWorldClient>(
           () => new HelloWorldClient(adminApiUri, messageHandler),
           LazyThreadSafetyMode.ExecutionAndPublication);

        //This client is used to communicate with the Admin API
        private static Lazy<AdminManagementClient> adminApiRestClient = new Lazy<AdminManagementClient>(
            () => new AdminManagementClient(adminApiUri, messageHandler),
            LazyThreadSafetyMode.ExecutionAndPublication);

        static ClientFactory()
        {
            adminApiUri = new Uri(OnPremPortalConfiguration.Instance.RdfeAdminUri);
            messageHandler = new BearerMessageProcessingHandler(new WebRequestHandler());
        }

        public static HelloWorldClient HelloWorldClient
        {
            get
            {
                return helloWorldRestClient.Value;
            }
        }

        public static AdminManagementClient AdminManagementClient
        {
            get
            {
                return adminApiRestClient.Value;
            }
        }
    }
}
