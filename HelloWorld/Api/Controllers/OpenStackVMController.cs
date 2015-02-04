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
    public class OpenStackVMController : ApiController
    {
        public static List<OpenStackVM> openstackvms = new List<OpenStackVM>();

        [HttpGet]
        public List<OpenStackVM> ListOpenStackVMs(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw new ArgumentNullException(subscriptionId);
            }

            var vms = from vm in openstackvms
                         where string.Equals(vm.SubscriptionId, subscriptionId, StringComparison.OrdinalIgnoreCase)
                         select vm;

            return vms.ToList();
        }

        [HttpPut]
        public void UpdateOpenStackVM(string subscriptionId, OpenStackVM openstackvmToUpdate)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, ErrorMessages.EmptySubscription);
            }

            if (openstackvmToUpdate == null)
            {
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, ErrorMessages.FileShareEmpty);
            }

            var openstackvm = (from vm in openstackvms
                               where vm.Id == openstackvmToUpdate.Id && string.Equals(vm.SubscriptionId, openstackvmToUpdate.SubscriptionId, StringComparison.OrdinalIgnoreCase)
                             select vm).FirstOrDefault();

            if (openstackvm != null)
            {
                string message = string.Format(CultureInfo.CurrentCulture, ErrorMessages.OpenStackVMNotFound, openstackvm.Location);
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, message);
            }

            openstackvm.Name = openstackvmToUpdate.Name;
            openstackvm.Location = openstackvmToUpdate.Location;
            openstackvm.Size = openstackvmToUpdate.Size;
        }

        [HttpPost]
        public void CreateOpenStackVM(OpenStackVM OpenStackVM)
        {
            if (OpenStackVM == null)
            {
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, ErrorMessages.OpenStackVMEmpty);
            }

            openstackvms.Add(new OpenStackVM
            {
                Id = openstackvms.Count,
                Location = OpenStackVM.Location,
                Name = OpenStackVM.Name,
                SubscriptionId = OpenStackVM.SubscriptionId,
                Size = OpenStackVM.Size
            });
        }

        internal static void PopulateOpenStackVMForSubscription(string subscriptionId)
        {
            double random;
            for (int count = 1; count < 10; count++)
            {
                random = new Random().NextDouble() * (99 - 10) + 10;

                openstackvms.Add(
                    new OpenStackVM
                    {
                        Id = openstackvms.Count,
                        Name = "Share " + IO.Path.GetFileNameWithoutExtension(IO.Path.GetRandomFileName()),
                        Location = "Server " + count,
                        Size = Convert.ToInt32(10 * random * count + random),
                        SubscriptionId = subscriptionId
                    }
                             );
            }

        }
    }
}
