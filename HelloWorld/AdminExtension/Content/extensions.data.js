(function (global, undefined) {
    "use strict";

    var extensions = [{
        name: "HelloWorldAdminExtension",
        displayName: "Hello World",
        iconUri: "/Content/HelloWorldAdmin/TestTeam.png",
        iconShowCount: false,
        iconTextOffset: 11,
        iconInvertTextColor: true,
        displayOrderHint: 51
    }];

    global.Shell.Internal.ExtensionProviders.addLocal(extensions);
})(this);