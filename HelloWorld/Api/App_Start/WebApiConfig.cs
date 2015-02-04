// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Web.Http;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
               name: "AdminSettings",
               routeTemplate: "admin/settings",
               defaults: new { controller = "AdminSettings" });

            config.Routes.MapHttpRoute(
                name: "AdminProducts",
                routeTemplate: "admin/products",
                defaults: new { controller = "Products" });

            config.Routes.MapHttpRoute(
                name: "AdminFileServers",
                routeTemplate: "admin/fileservers",
                defaults: new { controller = "FileServers" });

            config.Routes.MapHttpRoute(
               name: "HelloWorldQuota",
               routeTemplate: "admin/quota",
               defaults: new { controller = "Quota" });

            config.Routes.MapHttpRoute(
               name: "HelloWorldDefaultQuota",
               routeTemplate: "admin/defaultquota",
               defaults: new { controller = "Quota" });

            config.Routes.MapHttpRoute(
               name: "Subscription",
               routeTemplate: "admin/subscriptions",
               defaults: new { controller = "Subscriptions" });

            config.Routes.MapHttpRoute(
               name: "FileShares",
               routeTemplate: "subscriptions/{subscriptionId}/fileshares",
               defaults: new { controller = "FileShare" });

            config.Routes.MapHttpRoute(
               name: "Usage",
               routeTemplate: "usage",
               defaults: new { controller = "Usage" });
        }
    }
}
