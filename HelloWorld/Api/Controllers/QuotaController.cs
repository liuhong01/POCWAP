//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.WindowsAzurePack.Samples.DataContracts;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.Api.Controllers
{
    /// <summary>
    /// Controller class with all methods pertaining to Quota
    /// </summary>
    public class QuotaController : ApiController
    {
        /// <summary>
        /// Gets the default quota for Hello World Resource Provider.
        /// </summary>
        [HttpGet]
        public List<string> GetDefaultQuota()
        {
            return new List<string>();
        }

        /// <summary>
        /// Handles Put request to validate quotas.
        /// </summary>
        /// <param name="quotaUpdateBatch">The quota update batch.</param>
        /// <param name="validateOnly">if set to <c>true</c> validates the batch</param>
        [HttpGet]
        public HttpResponseMessage ValidateQuota(QuotaUpdateBatch quotaUpdateBatch, bool validateOnly)
        {
            // For this sample sake we are just returning status code Ok.
            // For your service do validate incoming quota information and return appropriate status code            
            return this.Request.CreateResponse<QuotaUpdateBatch>(HttpStatusCode.OK, quotaUpdateBatch);                        
        }

        /// <summary>
        /// Handles Put request to validate or update quotas.
        /// </summary>
        /// <param name="quotaUpdateBatch">The quota update batch.</param>
        [HttpPut]
        public QuotaUpdateResultBatch UpdateQuota(QuotaUpdateBatch quotaUpdateBatch)
        {
            var subscriptionList = quotaUpdateBatch.SubscriptionIdsToUpdate;
            if (subscriptionList == null || subscriptionList.Count == 0)
            {
                //Throw exception since no subscription is send to update quota for
                throw Utility.ThrowResponseException(this.Request, HttpStatusCode.BadRequest, ErrorMessages.NullOrEmptySubscriptionList);
            }
            
            // For this sample sake we are just returning QuotaUpdateResultBatch since no quota value is exposed from hello world RP
            // For your service do perform quota update using base quota and addon quota            
            return new QuotaUpdateResultBatch { UpdatedSubscriptionIds = subscriptionList };
        }
    }
}