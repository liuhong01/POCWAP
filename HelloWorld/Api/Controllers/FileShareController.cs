//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.WindowsAzurePack.Samples.HelloWorld.ApiClient.DataContracts;
using IO = System.IO;
using System.Globalization;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.Api.Controllers
{
    public class FileShareController : ApiController
    {
        public static List<FileShare> fileShares = new List<FileShare>();

        [HttpGet]
        public List<FileShare> ListFileShares(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw new ArgumentNullException(subscriptionId);
            }

            var shares = from share in fileShares
                         where string.Equals(share.SubscriptionId, subscriptionId, StringComparison.OrdinalIgnoreCase)
                         select share;

            return shares.ToList();
        }

        [HttpPut]
        public void UpdateFileShare(string subscriptionId, FileShare fileShareToUpdate)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, ErrorMessages.EmptySubscription);
            }

            if (fileShareToUpdate == null)
            {
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, ErrorMessages.FileShareEmpty);
            }

            var fileShare = (from share in fileShares
                             where share.Id == fileShareToUpdate.Id && string.Equals(share.SubscriptionId, fileShareToUpdate.SubscriptionId, StringComparison.OrdinalIgnoreCase)
                             select share).FirstOrDefault();

            if (fileShare != null)
            {
                string message = string.Format(CultureInfo.CurrentCulture, ErrorMessages.FileShareNotFound, fileShare.FileServerName);
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, message);
            }

            fileShare.Name = fileShareToUpdate.Name;
            fileShare.FileServerName = fileShareToUpdate.FileServerName;
            fileShare.Size = fileShareToUpdate.Size;
        }

        [HttpPost]
        public void CreateFileShare(FileShare fileShare)
        {
            if (fileShare == null)
            {
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, ErrorMessages.FileShareEmpty);
            }

            fileShares.Add(new FileShare
            {
                Id = fileShares.Count,
                FileServerName = fileShare.FileServerName,
                Name = fileShare.Name,
                SubscriptionId = fileShare.SubscriptionId,
                Size = fileShare.Size
            });
        }

        internal static void PopulateFileShareForSubscription(string subscriptionId)
        {
            double random;
            for (int count = 1; count < 10; count++)
            {
                random = new Random().NextDouble() * (99 - 10) + 10;

                fileShares.Add(
                    new FileShare
                    {
                        Id = fileShares.Count,
                        Name = "Share " + IO.Path.GetFileNameWithoutExtension(IO.Path.GetRandomFileName()),
                        FileServerName = "Server " + count,
                        Size = Convert.ToInt32(10 * random * count + random),
                        SubscriptionId = subscriptionId
                    }
                             );
            }

        }
    }
}
