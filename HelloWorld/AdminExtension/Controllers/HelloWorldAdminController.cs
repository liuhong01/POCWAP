// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.Azure.Portal.Configuration;
using Microsoft.WindowsAzurePack.Samples.DataContracts;
using Microsoft.WindowsAzurePack.Samples.HelloWorld.AdminExtension.Models;
using Microsoft.WindowsAzurePack.Samples.HelloWorld.ApiClient;
using Microsoft.WindowsAzurePack.Samples.HelloWorld.ApiClient.DataContracts;
using Microsoft.WindowsAzurePack.Samples.HelloWorld.Common;


namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.AdminExtension.Controllers
{
    [RequireHttps]
    [OutputCache(Location = OutputCacheLocation.None)]
    [PortalExceptionHandler]
    public sealed class HelloWorldAdminController : ExtensionController
    {
        private static readonly string adminAPIUri = OnPremPortalConfiguration.Instance.RdfeAdminUri;
        //This model is used to show registered resource provider information
        public EndpointModel HelloWorldServiceEndPoint { get; set; }

        /// <summary>
        /// Gets the admin settings.
        /// </summary>
        [HttpPost]
        [ActionName("AdminSettings")]
        public async Task<JsonResult> GetAdminSettings()
        {
            try
            {
                var resourceProvider = await ClientFactory.AdminManagementClient.GetResourceProviderAsync
                                                            (HelloWorldClient.RegisteredServiceName, Guid.Empty.ToString());

                this.HelloWorldServiceEndPoint = EndpointModel.FromResourceProviderEndpoint(resourceProvider.AdminEndpoint);
                return this.JsonDataSet(this.HelloWorldServiceEndPoint);
            }
            catch (ManagementClientException managementException)
            {
                // 404 means the Hello World resource provider is not yet configured, return an empty record.
                if (managementException.StatusCode == HttpStatusCode.NotFound)
                {
                    return this.JsonDataSet(new EndpointModel());
                }

                //Just throw if there is any other type of exception is encountered
                throw;
            }
        }

        /// <summary>
        /// Update admin settings => Register Resource Provider
        /// </summary>
        /// <param name="newSettings">The new settings.</param>
        [HttpPost]
        [ActionName("UpdateAdminSettings")]
        public async Task<JsonResult> UpdateAdminSettings(EndpointModel newSettings)
        {
            this.ValidateInput(newSettings);

            ResourceProvider helloWorldResourceProvider;
            string errorMessage = string.Empty;

            try
            {
                //Check if resource provider is already registered or not
                helloWorldResourceProvider = await ClientFactory.AdminManagementClient.GetResourceProviderAsync(HelloWorldClient.RegisteredServiceName, Guid.Empty.ToString());
            }
            catch (ManagementClientException exception)
            {
                // 404 means the Hello World resource provider is not yet configured, return an empty record.
                if (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    helloWorldResourceProvider = null;
                }
                else
                {
                    //Just throw if there is any other type of exception is encountered
                    throw;
                }
            }

            if (helloWorldResourceProvider != null)
            {
                //Resource provider already registered so lets update endpoint
                helloWorldResourceProvider.AdminEndpoint = newSettings.ToAdminEndpoint();
                helloWorldResourceProvider.TenantEndpoint = newSettings.ToTenantEndpoint();
                helloWorldResourceProvider.NotificationEndpoint = newSettings.ToNotificationEndpoint();
                helloWorldResourceProvider.UsageEndpoint = newSettings.ToUsageEndpoint();
            }
            else
            {
                //Resource provider not registered yet so lets register new one now
                helloWorldResourceProvider = new ResourceProvider()
                {
                    Name = HelloWorldClient.RegisteredServiceName,
                    DisplayName = "Hello World",
                    InstanceDisplayName = HelloWorldClient.RegisteredServiceName + " Instance",
                    Enabled = true,
                    PassThroughEnabled = true,
                    AllowAnonymousAccess = false,
                    AdminEndpoint = newSettings.ToAdminEndpoint(),
                    TenantEndpoint = newSettings.ToTenantEndpoint(),
                    NotificationEndpoint = newSettings.ToNotificationEndpoint(),
                    UsageEndpoint = newSettings.ToUsageEndpoint(),
                    MaxQuotaUpdateBatchSize = 3 // Check link http://technet.microsoft.com/en-us/library/dn520926(v=sc.20).aspx
                };
            }

            var testList = new ResourceProviderVerificationTestList()
                               {
                                   new ResourceProviderVerificationTest()
                                   {
                                       TestUri = new Uri(HelloWorldAdminController.adminAPIUri + HelloWorldClient.AdminSettings),
                                       IsAdmin = true
                                   }
                               };
            try
            {
                // Resource Provider Verification to ensure given endpoint and username/password is correct
                // Only validate the admin RP since we don't have a tenant subscription to do it.
                var result = await ClientFactory.AdminManagementClient.VerifyResourceProviderAsync(helloWorldResourceProvider, testList);
                if (result.HasFailures)
                {
                    throw new HttpException("Invalid endpoint or bad username/password");
                }
            }
            catch (ManagementClientException ex)
            {
                throw new HttpException("Invalid endpoint or bad username/password " + ex.Message.ToString());
            }

            //Finally Create Or Update resource provider
            Task<ResourceProvider> rpTask = (string.IsNullOrEmpty(helloWorldResourceProvider.Name) || String.IsNullOrEmpty(helloWorldResourceProvider.InstanceId))
                                                ? ClientFactory.AdminManagementClient.CreateResourceProviderAsync(helloWorldResourceProvider)
                                                : ClientFactory.AdminManagementClient.UpdateResourceProviderAsync(helloWorldResourceProvider.Name, helloWorldResourceProvider.InstanceId, helloWorldResourceProvider);

            try
            {
                await rpTask;
            }
            catch (ManagementClientException e)
            {
                throw e;
            }

            return this.Json(newSettings);
        }

        /// <summary>
        /// Gets all File Servers.
        /// </summary>
        [HttpPost]
        [ActionName("FileServers")]
        public async Task<JsonResult> GetAllFileServers()
        {
            try
            {
                var fileServers = await ClientFactory.HelloWorldClient.GetFileServerListAsync();
                var fileServerModel = fileServers.Select(d => new FileServerModel(d)).ToList();
                return this.JsonDataSet(fileServerModel);
            }
            catch (HttpRequestException)
            {
                // Returns an empty collection if the HTTP request to the API fails
                return this.JsonDataSet(new FileServerList());
            }
        }

        /// <summary>
        /// Gets all Products.
        /// </summary>
        [HttpPost]
        [ActionName("Products")]
        public async Task<JsonResult> GetAllProducts()
        {
            try
            {
                var productNames = await ClientFactory.HelloWorldClient.GetProductListAsync();
                var productModels = productNames.Select(d => new ProductModel(d)).ToList();
                return this.JsonDataSet(productModels);
            }
            catch (HttpRequestException)
            {
                // Returns an empty collection if the HTTP request to the API fails 
                return this.JsonDataSet(new ProductList());
            }
        }        

        private void ValidateInput(EndpointModel newSettings)
        {
            if (newSettings == null)
            {
                throw new ArgumentNullException("newSettings");
            }

            if (String.IsNullOrEmpty(newSettings.EndpointAddress))
            {
                throw new ArgumentNullException("EndpointAddress");
            }

            if (String.IsNullOrEmpty(newSettings.Username))
            {
                throw new ArgumentNullException("Username");
            }

            if (String.IsNullOrEmpty(newSettings.Password))
            {
                throw new ArgumentNullException("Password");
            }
        }
    }
}