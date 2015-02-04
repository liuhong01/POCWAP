//-----------------------------------------------------------------------
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.WindowsAzurePack.Samples.HelloWorld.Common;
using Microsoft.WindowsAzurePack.Samples.HelloWorld.TenantExtension.Models;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.TenantExtension.Controllers
{
    [RequireHttps]
    [OutputCache(Location = OutputCacheLocation.None)]
    [PortalExceptionHandler]
    public sealed class HelloWorldTenantController : ExtensionController
    {   
        /// <summary>
        /// List file shares belong to subscription
        /// NOTE: For this sample dummy entries will be displayed
        /// </summary>
        /// <param name="subscriptionIds"></param>
        /// <returns></returns>
        [HttpPost]        
        public async Task<JsonResult> ListFileShares(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var fileShares = new List<FileShareModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var fileSharesFromApi = await ClientFactory.HelloWorldClient.ListFileSharesAsync(subId);
                fileShares.AddRange(fileSharesFromApi.Select(d => new FileShareModel(d)));
            }

            return this.JsonDataSet(fileShares);
        }

        /// <summary>
        /// Create new file share for subscription
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="fileShareToCreate"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult>  CreateFileShare(string subscriptionId, FileShareModel fileShareToCreate)
        {
            await ClientFactory.HelloWorldClient.CreateFileShareAsync(subscriptionId, fileShareToCreate.ToApiObject());

            return this.Json(fileShareToCreate);
        }       
    }
}