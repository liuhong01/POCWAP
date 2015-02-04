(function (global, undefined) {
    "use strict";

    var extensions = [{
        name: "HelloWorldTenantExtension",
        displayName: "Hello World",
        iconUri: "/Content/HelloWorldTenant/HelloWorldTenant.png",
        iconShowCount: false,
        iconTextOffset: 11,
        iconInvertTextColor: true,
        displayOrderHint: 2 // Display it right after WebSites extension (order 1)
    }];

    global.Shell.Internal.ExtensionProviders.addLocal(extensions);
})(this);