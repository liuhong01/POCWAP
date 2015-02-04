// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Configuration;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.Api
{
    public class HelloWorldApi : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            string apiUserName = ConfigurationManager.AppSettings["Username"];
            string apiPassword = ConfigurationManager.AppSettings["Password"];

            string authorizationHeader = Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                throw new UnauthorizedAccessException("Access Denied :: Auth header empty");
            }

            string[] authorizationHeaderParts = authorizationHeader.Split(new char[] { ' ' }, 2);

            if (authorizationHeaderParts.Length != 2 || !string.Equals(authorizationHeaderParts[0], "Basic", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new UnauthorizedAccessException("Access Denied :: Invalid Header");
            }

            authorizationHeader = (authorizationHeader.Split(' '))[1];

            string[] credentials = Encoding.ASCII.GetString(Convert.FromBase64String(authorizationHeader)).Split(new char[] { ':' });

            if (credentials == null)
            {
                throw new UnauthorizedAccessException("Access Denied :: No credentials found");
            }

            string username = credentials[0];
            string password = credentials[1];

            //Only for this sample sake username and password in plain text
            //Please check recommendation for encrypting username and password
            if (!string.Equals(apiUserName, username, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(apiPassword, password, StringComparison.Ordinal))
            {
                throw new UnauthorizedAccessException("Access Denied - Username password didn't match");
            }

            //NOTE: RECOMMENDATION FOR SECURE PASSWORD
            //1. Change the web.config with a value from [Microsoft.WindowsAzure.Server.Common.SecurityHelper]::ComputeHash('pass@word1')
            //2. Add Microsoft.WindowsAzure.Server.Common.dll to c:\inetpub\MgmtSvc-HelloWorld\bin
            //3. And replace this code with !SecurityHelper.ValidateHash(password, apiPassword)
        }
    }
}