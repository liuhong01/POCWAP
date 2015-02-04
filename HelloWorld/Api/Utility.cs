//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.WindowsAzurePack.Samples.HelloWorld.ApiClient.DataContracts;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.Api
{
    /// <summary>
    /// Generic Utitlities
    /// </summary>
    internal class Utility
    {
        /// <summary>
        /// This is getting used to return error response. This can be used for methods not returning any specific object
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <param name="errorResourceCode"></param>
        /// <returns></returns>
        internal static HttpResponseMessage SendResponseException(HttpRequestMessage request, HttpStatusCode statusCode, string message, string errorResourceCode = null)
        {
            return request.CreateResponse<HelloWorldErrorResource>(statusCode, new HelloWorldErrorResource()
            {
                Code = errorResourceCode,
                Message = message
            });  
        }

        /// <summary>
        /// This method is used to throw exceptions
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <param name="errorResourceCode"></param>
        /// <returns></returns>
        internal static HttpResponseException ThrowResponseException(HttpRequestMessage request, HttpStatusCode statusCode, string message, string errorResourceCode = null)
        {
            return new HttpResponseException(
             new HttpResponseMessage(statusCode)
             {
                 Content = new ObjectContent<HelloWorldErrorResource>(
                             new HelloWorldErrorResource()
                             {
                                 Code = errorResourceCode,
                                 Message = message,
                             },
                             new XmlMediaTypeFormatter())
             });                   
        }
       
    }
}