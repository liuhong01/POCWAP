// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.Samples.HelloWorld.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.ApiClient
{
    /// <summary>
    /// This is client of Hello World Resource Provider 
    /// This client is used by admin and tenant extensions to make call to Hello World Resource Provider
    /// In real world you should have seperate clients of admin and tenant extensions    
    /// </summary>
    public class HelloWorldClient
    {        
        public const string RegisteredServiceName = "helloworld";
        public const string RegisteredPath = "services/" + RegisteredServiceName;
        public const string AdminSettings = RegisteredPath + "/settings";
        public const string AdminProducts = RegisteredPath + "/products";
        public const string AdminFileServers = RegisteredPath + "/fileservers";

        public const string FileShares = "{0}/" + RegisteredPath + "/fileshares";

        public Uri BaseEndpoint { get; set; }
        public HttpClient httpClient;

        /// <summary>
        /// This constructor takes BearerMessageProcessingHandler which reads token as attach to each request
        /// </summary>
        /// <param name="baseEndpoint"></param>
        /// <param name="handler"></param>
        public HelloWorldClient(Uri baseEndpoint, MessageProcessingHandler handler)
        {
            if (baseEndpoint == null) 
            {
                throw new ArgumentNullException("baseEndpoint"); 
            }

            this.BaseEndpoint = baseEndpoint;

            this.httpClient = new HttpClient(handler);
        }

        public HelloWorldClient(Uri baseEndpoint, string bearerToken, TimeSpan? timeout = null)
        {
            if (baseEndpoint == null) 
            { 
                throw new ArgumentNullException("baseEndpoint"); 
            }

            this.BaseEndpoint = baseEndpoint;

            this.httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            if (timeout.HasValue)
            {
                this.httpClient.Timeout = timeout.Value;
            }
        }
       
        #region Admin APIs
        /// <summary>
        /// GetAdminSettings returns Hello World Resource Provider endpoint information if its registered with Admin API
        /// </summary>
        /// <returns></returns>
        public async Task<AdminSettings> GetAdminSettingsAsync()
        {
            var requestUrl = this.CreateRequestUri(HelloWorldClient.AdminSettings);

            // For simplicity, we make a request synchronously.
            var response = await this.httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AdminSettings>();
        }

        /// <summary>
        /// UpdateAdminSettings registers Hello World Resource Provider endpoint information with Admin API
        /// </summary>
        /// <returns></returns>
        public async Task UpdateAdminSettingsAsync(AdminSettings newSettings)
        {
            var requestUrl = this.CreateRequestUri(HelloWorldClient.AdminSettings);
            var response = await this.httpClient.PutAsJsonAsync<AdminSettings>(requestUrl.ToString(), newSettings);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// GetFileServerList return list of file servers hosted in Hello World Resource Provider
        /// </summary>
        /// <returns></returns>
        public async Task<List<FileServer>> GetFileServerListAsync()
        {
            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, HelloWorldClient.AdminFileServers));

            var response = await this.httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<FileServer>>();
        }

        /// <summary>
        /// UpdateFileServer updates existing file server information in Hello World Resource Provider
        /// </summary>        
        public async Task UpdateFileServerAsync(FileServer fileServer)
        {
            var requestUrl = this.CreateRequestUri(HelloWorldClient.AdminFileServers);
            var response = await this.httpClient.PutAsJsonAsync<FileServer>(requestUrl.ToString(), fileServer);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// AddFileServer adds new file server in Hello World Resource Provider
        /// </summary>        
        public async Task AddFileServerAsync(FileServer fileServer)
        {
            var requestUrl = this.CreateRequestUri(HelloWorldClient.AdminFileServers);
            var response = await this.httpClient.PutAsJsonAsync<FileServer>(requestUrl.ToString(), fileServer);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// GetProductList return list of products stored in Hello World Resource Provider
        /// </summary>        
        public async Task<List<Product>> GetProductListAsync()
        {
            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, HelloWorldClient.AdminProducts));

            var response = await this.httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<Product>>();
        }

        /// <summary>
        /// UpdateProduct updates existing product information in Hello World Resource Provider
        /// </summary>        
        public async Task UpdateProductAsync(Product product)
        {
            var requestUrl = this.CreateRequestUri(HelloWorldClient.AdminProducts);
            var response = await this.httpClient.PutAsJsonAsync<Product>(requestUrl.ToString(), product);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// AddProduct adds new product in Hello World Resource Provider
        /// </summary>        
        public async Task AddProductAsync(Product product)
        {
            var requestUrl = this.CreateRequestUri(HelloWorldClient.AdminProducts);
            var response = await this.httpClient.PostAsXmlAsync<Product>(requestUrl.ToString(), product);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Tenant APIs
        /// <summary>
        /// ListFileShares supposed to return list of file shares per subscription stored in Hello World Resource Provider 
        /// Per subscription shares not implemented for this sample so its returning static common file shares for all subscriptions
        /// </summary> 
        public async Task<List<FileShare>> ListFileSharesAsync(string subscriptionId = null)
        {
            var requestUrl = this.CreateRequestUri(HelloWorldClient.CreateUri(subscriptionId));
            return await this.GetAsync<List<FileShare>>(requestUrl);            
        }
        
        /// <summary>
        /// CreateFileShare allows to create new file share for given subscription 
        /// </summary>        
        public async Task CreateFileShareAsync(string subscriptionId, FileShare fileShareNameToCreate)
        {
            var requestUrl = this.CreateRequestUri(HelloWorldClient.CreateUri(subscriptionId));
            await this.PostAsync<FileShare>(requestUrl, fileShareNameToCreate);            
        }        

        /// <summary>
        /// UpdateFileShare allows to update existing file share for given subscription 
        /// </summary>        
        public async Task UpdateFileShareAsync(string subscriptionId, FileShare fileShareNameToUpdate)
        {
            var requestUrl = this.CreateRequestUri(HelloWorldClient.CreateUri(subscriptionId));
            await this.PutAsync<FileShare>(requestUrl, fileShareNameToUpdate);            
        }        
        #endregion

        #region Private Methods
        /// <summary>
        /// Common method for making GET calls
        /// </summary>        
        private async Task<T> GetAsync<T>(Uri requestUrl)
        {         
            var response = await this.httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        /// <summary>
        /// Common method for making POST calls
        /// </summary>        
        private async Task PostAsync<T>(Uri requestUrl, T content)
        {            
            var response = await this.httpClient.PostAsXmlAsync<T>(requestUrl.ToString(), content);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Common method for making PUT calls
        /// </summary>        
        private async Task PutAsync<T>(Uri requestUrl, T content)
        {            
            var response = await this.httpClient.PutAsJsonAsync<T>(requestUrl.ToString(), content);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Common method for making Request Uri's
        /// </summary>        
        private Uri CreateRequestUri(string relativePath, string queryString = "")
        {
            var endpoint = new Uri(this.BaseEndpoint, relativePath);
            var uriBuilder = new UriBuilder(endpoint);
            uriBuilder.Query = queryString;
            return uriBuilder.Uri;
        }

        private static string CreateUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, HelloWorldClient.FileShares, subscriptionId);
        }
        #endregion
    }
}
