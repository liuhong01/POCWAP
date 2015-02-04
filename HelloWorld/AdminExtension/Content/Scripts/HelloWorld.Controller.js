/*globals window,jQuery,cdm, HelloWorldAdminExtension*/
(function ($, global, undefined) {
    "use strict";

    var baseUrl = "/HelloWorldAdmin",
        adminSettingsUrl = baseUrl + "/AdminSettings",
        adminProductsUrl = baseUrl + "/Products",
        adminFileServersUrl = baseUrl + "/FileServers";

    function makeAjaxCall(url, data) {
        return Shell.Net.ajaxPost({
            url: url,
            data: data
        });
    }

    function updateAdminSettings(newSettings) {
        return makeAjaxCall(baseUrl + "/UpdateAdminSettings", newSettings);
    }

    function invalidateAdminSettingsCache() {
        return global.Exp.Data.getData({
            url: global.HelloWorldAdminExtension.Controller.adminSettingsUrl,
            dataSetName: HelloWorldAdminExtension.Controller.adminSettingsUrl,
            forceCacheRefresh: true
        });
    }

    function getCurrentAdminSettings() {
        return makeAjaxCall(global.HelloWorldAdminExtension.Controller.adminSettingsUrl);
    }

    function isResourceProviderRegistered() {
        global.Shell.UI.Spinner.show();
        global.HelloWorldAdminExtension.Controller.getCurrentAdminSettings()
        .done(function (response) {
            if (response && response.data.EndpointAddress) {
                return true;
            }
            else {
                return false;
            }
        })
         .always(function () {
             global.Shell.UI.Spinner.hide();
         });
    }

    // Public
    global.HelloWorldAdminExtension = global.HelloWorldAdminExtension || {};
    global.HelloWorldAdminExtension.Controller = {
        adminSettingsUrl: adminSettingsUrl,
        adminProductsUrl: adminProductsUrl,
        adminFileServersUrl: adminFileServersUrl,
        updateAdminSettings: updateAdminSettings,
        getCurrentAdminSettings: getCurrentAdminSettings,
        invalidateAdminSettingsCache: invalidateAdminSettingsCache,
        isResourceProviderRegistered: isResourceProviderRegistered
    };
})(jQuery, this);
