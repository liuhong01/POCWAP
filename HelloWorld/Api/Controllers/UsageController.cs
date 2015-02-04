//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.WindowsAzurePack.Samples.DataContracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Web.Http;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.Api.Controllers
{
    /// <summary>
    /// Controller class with all methods pertaining to Quota
    /// </summary>
    public class UsageController : ApiController
    {
        //This is static because it get incremented with each fake record created during GetUsageData call
        private static int EventId = 0;

        /// <summary>
        /// Collects the usage data
        /// </summary>
        [HttpGet]
        public IEnumerable<UsageRecord> GetUsageData(string lastId = null, int batchSize = 0)
        {            
            var usageRecords = new List<UsageRecord>();            
            
            int id;

            if (string.IsNullOrWhiteSpace(lastId))
            {
                id = 0;
            }
            else if (!int.TryParse(lastId, out id))
            {
                string errorMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.InvalidLastId, lastId);
                throw Utility.ThrowResponseException(this.Request, HttpStatusCode.BadRequest, errorMessage);
            }

            if (batchSize < 0)
            {
                throw Utility.ThrowResponseException(this.Request, HttpStatusCode.BadRequest, ErrorMessages.InvalidBatchSize);
            }

            //If batchsize is 0, no need to fetch usage records. just return a empty collection.            
            if (batchSize == 0)
            {
                return usageRecords;
            }


            string subscriptionId = string.Empty;
            if (SubscriptionsController.subscriptions != null && SubscriptionsController.subscriptions.Count > 0)
            {
                subscriptionId = SubscriptionsController.subscriptions[0].SubscriptionId;
            }
            else
            {
                subscriptionId = Guid.NewGuid().ToString();
            }

            //At this stage one should read actual usage records and send back data
            //For sample purpose fake data is getting returned
            int fakeRecordCount = new Random().Next(10);
            for (int count = 0; count < fakeRecordCount; count++)
            {
                usageRecords.Add(new UsageRecord()
                {
                    EventId = UsageController.EventId++,
                    StartTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(10)),
                    EndTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(9)),
                    SubscriptionId = subscriptionId,
                    ServiceType = "Sample record or asset name",
                    Properties = null, 
                    Resources = new Dictionary<string, string>() 
                    {
                        {"QuotaPropertyName1" , "QuotaPropertyValue1"},
                        {"QuotaPropertyName2" , "QuotaPropertyValue2"},
                        {"QuotaPropertyName3" , "QuotaPropertyValue3"}
                    }
                });
            }

            return usageRecords;
        }
    }
}